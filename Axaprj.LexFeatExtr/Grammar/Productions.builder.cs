using System;
using System.Linq;
using System.Collections.Generic;

namespace Axaprj.LexFeatExtr.Grammar
{
    partial class Productions
    {
        protected class RangeGenerator
        {
            public readonly object Symbol;
            public readonly int FromQty;
            public readonly int ToQty;
            public RangeGenerator(object symbol, int from_qty, int to_qty)
            {
                Symbol = symbol;
                FromQty = from_qty;
                ToQty = to_qty;
            }
        }

        /// <summary>Factory: Expression</summary>
        protected static Expression EXPR(params object[] elems) => new Expression(elems);
        /// <summary>Factory: Expression Ranges generator (macro)</summary>
        /// <remarks>
        /// A := RANGE(0, 2, B) C 
        /// is replaced by
        /// A := C | B C | B B C
        /// </remarks>
        protected static RangeGenerator RANGE(int from_qty, int to_qty, object symbol) =>
            new RangeGenerator(symbol, from_qty, to_qty);

        public class Grammar : Dictionary<object, List<Expression>>
        {
            public readonly object RootSymbol;
            public Grammar(object root_symbol) =>
                RootSymbol = root_symbol;

            public void AssertRulesFound(object symbol)
            {
                if (!ContainsKey(symbol))
                    throw new InvalidOperationException($"Symbol '{symbol}' rules not found");
            }

            /// <summary>Add Production Rule</summary>
            public void AddRULE(object symbol, params Expression[] expressions)
            {
                List<Expression> rules = this.ContainsKey(symbol) ? this[symbol] : null;
                if (rules == null)
                {
                    rules = new List<Expression>();
                    this.Add(symbol, rules);
                }
                expressions = PreProcess(expressions);
                if (expressions == null || expressions.Length == 0)
                    throw new InvalidOperationException(
                        $"Empty expression of non term {nameof(symbol)} {symbol}");
                rules.AddRange(expressions);
            }

            protected virtual Expression[] PreProcess(Expression[] expressions)
            {
                var res_exp = new List<Expression>(expressions.Length);
                for (int expr_inx = 0; expr_inx < expressions.Length; expr_inx++)
                {
                    var expr = expressions[expr_inx];
                    var syms = expr.Symbols;
                    for (int sinx = 0; sinx < syms.Length; sinx++)
                    {
                        if (syms[sinx] is RangeGenerator rng)
                        {
                            var expr_begin = new object[sinx];
                            Array.Copy(syms, expr_begin, expr_begin.Length);
                            var expr_end = new object[syms.Length - sinx - 1];
                            Array.Copy(syms, sinx + 1, expr_end, 0, expr_end.Length);
                            var rng_list = new List<Expression>();
                            int delta = rng.FromQty < rng.ToQty ? 1 : -1;
                            for (int qty = rng.FromQty;
                                    delta > 0 ? qty <= rng.ToQty : qty >= rng.ToQty; qty += delta)
                            {
                                var rng_syms = new List<object>(expr_begin);
                                for (int cur_qty = 0; cur_qty < qty; cur_qty++)
                                    rng_syms.Add(rng.Symbol);
                                rng_syms.AddRange(expr_end);
                                res_exp.Add(new Expression(rng_syms.ToArray()));
                            }
                            for (int rest_inx = expr_inx + 1; rest_inx < expressions.Length; rest_inx++)
                                res_exp.Add(expressions[rest_inx]);
                            return PreProcess(res_exp.ToArray());
                        }
                    }
                    res_exp.Add(expr);
                }
                return res_exp.ToArray();
            }
        }
    }
}
