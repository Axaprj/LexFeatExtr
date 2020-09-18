using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar
{
    public static class ProductsHelper
    {
        /// <summary>Collect Products by Symbol value</summary>
        public static bool TryFind(this IProd prod, object symbol, out List<IProd> found_prods)
            => prod.TryFind(out found_prods, (p) => p.IsEqualBySymbol(symbol));

        /// <summary>Collect Products by predicate</summary>
        public static bool TryFind(this IProd prod, out List<IProd> found_prods, Func<IProd, bool> predicate)
        {
            found_prods = new List<IProd>();
            if (prod is INonTermProd ntp)
            {
                if (ntp.Components != null)
                {
                    foreach (var cprod in ntp.Components)
                    {
                        if (cprod.TryFind(out List<IProd> found_cprods, predicate))
                            found_prods.AddRange(found_cprods);
                    }
                }
            }
            if (predicate(prod))
                found_prods.Add(prod);
            return found_prods.Count > 0;
        }
    }
}
