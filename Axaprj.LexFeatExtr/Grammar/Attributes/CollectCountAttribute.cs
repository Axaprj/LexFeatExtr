using System;
using System.Collections.Generic;
using System.Linq;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>
    /// Check current NotTerm collection filling
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CollectCountAttribute : TermAttribute
    {
        readonly int ReqiredQty;
        public CollectCountAttribute(int reqiredQty = 1, bool addToResult = false) : base(addToResult)
        {
            ReqiredQty = reqiredQty;
        }

        public override bool IsMatched(object symbol, IVGrammarTextCursor cursor)
        {
            bool is_matched = false;
            object check_symb = null;
            int cnt = 0;
            var stack = cursor.VGContext.ExecStack;
            stack.InspectStack((cur_prod) =>
            {
                if (check_symb == null)
                    check_symb = cur_prod.Symbol;
                else if (check_symb == cur_prod.Symbol)
                {
                    cnt++;
                    is_matched = cnt >= ReqiredQty;
                }
                else
                    return false;
                return !is_matched;
            });
            return is_matched;
        }

        public override bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        {
            prod = null;
            if (IsMatched(symbol, cursor))
                prod = new TermProd(symbol, IsAddToResult);
            return prod != null;
        }
        
        //public override bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        //{
        //    prod = null;
        //    var stack = cursor.VGContext.ExecStack;
        //    var stack_prod = stack.StackProd.Reverse();
        //    int cnt = 0;
        //    object cur_symb = null;
        //    foreach (var sprod in stack_prod)
        //    {
        //        if (cur_symb == null)
        //            cur_symb = sprod.Symbol;
        //        else if(cur_symb == sprod.Symbol)
        //        {
        //            cnt++;
        //            if (cnt >= ReqiredQty)
        //            {
        //                prod = new TermProd(symbol, IsAddToResult);
        //                break;
        //            }
        //        }
        //    }
        //    return prod != null;
        //}
    }
}
