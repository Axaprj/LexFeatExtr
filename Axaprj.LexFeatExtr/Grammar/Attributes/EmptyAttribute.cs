using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>
    /// No any input token
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EmptyAttribute : TermAttribute
    {
        public EmptyAttribute(bool addToResult = false) : base(addToResult) { }

        public override bool IsMatched(object symbol, IVGrammarTextCursor cursor) =>
            cursor.IsEmpty;

        public override bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        {
            prod = null;
            if (IsMatched(symbol, cursor))
                prod = new TermProd(symbol, IsAddToResult);
            return prod != null;
        }
    }
}
