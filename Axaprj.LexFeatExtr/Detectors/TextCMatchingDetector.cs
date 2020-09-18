using Axaprj.LexFeatExtr.Grammar;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Takenet.Textc;
using Takenet.Textc.Types;

namespace Axaprj.LexFeatExtr.Detectors
{
    public class TextCMatchingDetector : IMatchingDetector
    {
        readonly object DetectorsLock = new object();
        readonly Dictionary<CultureInfo, IDetectorTextC[]> Detectors;
        string[] _syntaxPatterns;

        public TextCMatchingDetector(string[] syntax_patterns, IVGrammarRequestContext context)
        {
            Detectors = new Dictionary<CultureInfo, IDetectorTextC[]>();
            _syntaxPatterns = syntax_patterns;
        }

        IDetectorTextC[] GetCreateDetectors(CultureInfo culture)
        {
            lock (DetectorsLock)
            {
                if (Detectors.ContainsKey(culture))
                    return Detectors[culture];
                var detectors = new List<IDetectorTextC>();
                foreach (var syntax in _syntaxPatterns)
                {
                    var detector = new DetectorTextC(syntax, culture);
                    detectors.Add(detector);
                }
                Detectors.Add(culture, detectors.ToArray());
                return detectors.ToArray();
            }
        }

        public virtual bool IsMatched(IVGrammarTextCursor textCursor, out Dictionary<string, object> valDict)
        {
            valDict = null;
            var culture = textCursor.Context.Culture;
            var detectors = GetCreateDetectors(culture);
            foreach (var detector in detectors)
            {
                if (detector.IsMatched(textCursor, out Expression expr, CancellationToken.None))
                {
                    foreach (var token in expr.Tokens)
                    {
                        if (token == null)
                            continue;
                        if (IsNamedToken(token.Type.Name))
                        {
                            valDict = valDict ?? new Dictionary<string, object>();
                            valDict.Add(token.Type.Name, token.Value);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        bool IsNamedToken(string name)
        {
            var not_named = string.IsNullOrEmpty(name);
            if (!not_named && (name.Length > 1))
            {
                not_named = name[0] == 't' && 
                    name.Substring(1).All(char.IsDigit);
            }
            return !not_named;
        }
    }
}
