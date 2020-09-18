using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>Non Terminal symbol attribute</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NonTermAttribute : SymbolAttribute, IGAttrNonTerm
    {
        public NonTermAttribute(bool mergeComponents= false, bool addToResult = true) 
            : base(addToResult) {
            MergeComponents = mergeComponents;
        }

        public bool MergeComponents { get; }

        public INonTermProd CreateProduct(object symbol, IVGrammarTextCursor cursor) =>
            new NonTermProd(symbol, MergeComponents, IsAddToResult);

    }
}
