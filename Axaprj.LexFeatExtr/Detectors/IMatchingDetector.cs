using Axaprj.LexFeatExtr.Grammar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Detectors
{
    /// <summary> Tokens matched detector </summary>
    public interface IMatchingDetector
    {
        bool IsMatched(IVGrammarTextCursor textCursor, out Dictionary<string, object> valDict);
    }
}
