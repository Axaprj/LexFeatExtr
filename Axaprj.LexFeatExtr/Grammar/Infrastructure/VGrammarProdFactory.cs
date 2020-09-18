using Axaprj.LexFeatExtr.Grammar.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Axaprj.LexFeatExtr.Grammar.Infrastructure
{
    /// <summary> Products Detector and Factory </summary>
    public interface IVGrammarProdFactory
    {
        bool TryGetTermProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod);
        bool TryGetNonTermProduct(object symbol, IVGrammarTextCursor cursor, out INonTermProd prod);
    }

    public class VGrammarProdFactory : IVGrammarProdFactory
    {
        readonly ConcurrentDictionary<object, object[]> _dictSymbAttrbutes =
            new ConcurrentDictionary<object, object[]>();

        TAttr GetAttrib<TAttr>(object[] attrs) where TAttr : IGAttrSymbol =>
            (TAttr)attrs.FirstOrDefault(a => (a is TAttr));

        protected virtual bool IsLangConform(IVGrammarTextCursor cursor, IGAttrSymbol attr) =>
            CultureInfo.InvariantCulture.Equals(attr.Lng) 
            ? true : attr.Lng.Name == cursor.Context.Culture.Name;

        protected bool TryGetAttribs(object symbol, out object[] attribs)
        {
            attribs = null;
            if(!_dictSymbAttrbutes.TryGetValue(symbol, out attribs))
            {
                var memInfo = symbol.GetType().GetMember(symbol.ToString());
                if (memInfo.Length > 0)
                    attribs = memInfo[0].GetCustomAttributes(false);
                _dictSymbAttrbutes.TryAdd(symbol, attribs);
            }
            return attribs != null && attribs.Length > 0;
        }

        public bool TryGetNonTermProduct(object symbol, IVGrammarTextCursor cursor, out INonTermProd prod)
        {
            prod = null;
            bool is_str = symbol is string;
            if (!is_str)
            {
                if (TryGetAttribs(symbol, out object[] attrs))
                {
                    var nt_attr = GetAttrib<IGAttrNonTerm>(attrs);
                    if (nt_attr != null && IsLangConform(cursor, nt_attr)) // not terminal case
                        prod = nt_attr.CreateProduct(symbol, cursor);
                }
            }
            return prod != null;
        }

        public bool TryGetTermProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        {
            prod = null;
            bool is_str = symbol is string;
            if (is_str)
            { // string terminal case
                if (string.Equals(symbol.ToString(), cursor.Peek()
                    , StringComparison.InvariantCultureIgnoreCase))
                {
                    prod = new TermValue<string>(symbol, cursor.Peek(), is_add_to_result: false);
                    cursor.StartFromNextToken();
                    return true;
                }
                else
                    return false;
            }
            else if (TryGetAttribs(symbol, out object[] attrs))
            { // common terminal case
                IGAttrTerm term_attr = null;
                foreach (var attr in attrs)
                {
                    if (attr is IGAttrTerm)
                    {
                        term_attr = (IGAttrTerm)attr;
                        if (IsLangConform(cursor, term_attr) &&
                            term_attr.TryCreateProduct(symbol, cursor, out prod))
                            return true;
                    }
                }
                if (term_attr == null)
                    throw new InvalidOperationException(
                        $"Not found term attributes of {symbol} symbol {nameof(symbol)}");
                return false;
            }
            throw new InvalidOperationException($"Unknown term {symbol} symbol {nameof(symbol)}");
        }
    }
}
