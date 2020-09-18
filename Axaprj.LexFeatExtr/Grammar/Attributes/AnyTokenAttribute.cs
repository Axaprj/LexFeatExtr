using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>
    /// Any input token
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AnyTokenAttribute : TermAttribute
    {
        public AnyTokenAttribute(bool addToResult = true) : base(addToResult) { }

        public override bool IsMatched(object symbol, IVGrammarTextCursor cursor) =>
            !cursor.IsEmpty;

        public override bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        {
            prod = null;
            if (IsMatched(symbol, cursor))
            {
                prod = new TermValue<string>(symbol, cursor.Peek(), IsAddToResult);
                cursor.StartFromNextToken();
            }
            return prod != null;
        }
    }
}
