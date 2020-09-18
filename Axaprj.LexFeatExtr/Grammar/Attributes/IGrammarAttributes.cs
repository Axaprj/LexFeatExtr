using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary> Grammar Symbol Attribute </summary>
    public interface IGAttrSymbol
    {
        CultureInfo Lng { get; }
    }
    /// <summary> Grammar Non Terminal Symbol Attribute (Factory)</summary>
    public interface IGAttrNonTerm : IGAttrSymbol
    {
        INonTermProd CreateProduct(object symbol, IVGrammarTextCursor cursor);
    }
    /// <summary> Grammar Terminal Symbol Attribute (Factory and Detector) </summary>
    public interface IGAttrTerm : IGAttrSymbol
    {
        bool IsMatched(object symbol, IVGrammarTextCursor cursor);
        bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod);
    }

}
