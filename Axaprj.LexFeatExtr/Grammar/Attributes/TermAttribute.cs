using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>Terminal symbol attribute</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class TermAttribute: SymbolAttribute, IGAttrTerm
    {
        public TermAttribute(bool addToResult = true) : base(addToResult) { }

        public abstract bool IsMatched(object symbol, IVGrammarTextCursor cursor);

        public abstract bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod);
    }
}
