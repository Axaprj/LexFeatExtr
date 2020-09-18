using Axaprj.LexFeatExtr.Detectors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>
    /// Takenet.Textc syntax patterns based Grammar Symbol Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TextCAttribute : TermAttribute
    {
        string[] _patternsList;
        IMatchingDetector _detector;
        readonly object DetectorLock = new object();

        public TextCAttribute() { }

        public TextCAttribute(params string[] vals) : this() =>
            _patternsList = vals;

        public string[] SyntaxPatterns
        { get => _patternsList; set => _patternsList = value; }

        protected virtual IMatchingDetector GetCreateDetector(IVGrammarTextCursor cursor)
        {
            lock (DetectorLock)
            {
                if (_detector == null)
                {
                    var culture = cursor.VGContext;
                    _detector = _detector ?? new TextCMatchingDetector(
                       this.SyntaxPatterns, culture);
                }
                return _detector;
            }
        }

        public override bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        {
            prod = null;
            cursor.Reset();
            if (!cursor.IsEmpty)
            {
                var detector = GetCreateDetector(cursor);
                if (detector.IsMatched(cursor, out Dictionary<string, object> valDict))
                {
                    cursor.StartFromRemainingTokens();
                    Dictionary<string, ITermProd> dict_prod = null;
                    if (valDict != null)
                    {
                        foreach (var name_val in valDict)
                        {
                            var val = name_val.Value;
                            var name = name_val.Key;
                            if (valDict.Count == 1)
                            {
                                prod = CreateTermProd(symbol, val);
                                ((TermProd)prod).DisplayName = name;
                                break;
                            }
                            else
                            {
                                dict_prod = dict_prod ?? new Dictionary<string, ITermProd>();
                                dict_prod.Add(name, CreateTermProd(name, val));
                            }
                        }
                    }
                    if (dict_prod != null)
                        prod = new TermMultiValue(symbol, dict_prod, IsAddToResult);
                    else
                        prod = prod ?? new TermProd(symbol, IsAddToResult);
                }
            }
            cursor.Reset();
            return prod != null;
        }

        TermProd CreateTermProd(object symbol, object val)
        {
            switch (val)
            {
                case int v:
                    return new TermValue<int>(symbol, v, IsAddToResult);
                case decimal v:
                    return new TermValue<decimal>(symbol, v, IsAddToResult);
                case DateTime v:
                    return new TermValue<DateTime>(symbol, v, IsAddToResult);
                case string v:
                    return new TermValue<string>(symbol, v, IsAddToResult);
                default:
                    throw new InvalidOperationException(
                        $" Unknown/NotImplemented TermProd {val} {nameof(val)} type");
            }
        }

        public override bool IsMatched(object symbol, IVGrammarTextCursor cursor)
        {
            bool res = false;
            cursor.Reset();
            if (!cursor.IsEmpty)
            {
                var detector = GetCreateDetector(cursor);
                res = detector.IsMatched(cursor, out _);
                cursor.Reset();
            }
            return res;
        }
    }
}
