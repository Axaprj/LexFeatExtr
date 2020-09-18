using Axaprj.LexFeatExtr.Grammar.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Axaprj.LexFeatExtr.Grammar
{
    /// <summary>
    /// Grammar Productions definition and processing
    /// (TAG:thread-safe)
    /// </summary>
    /// <remarks>
    /// See Regular Grammars https://en.wikipedia.org/wiki/Formal_grammar
    /// </remarks>
    public abstract partial class Productions
    {
        public class Expression
        {
            public readonly object[] Symbols;
            public Expression(object[] expr) => Symbols = expr;
            public override string ToString()
                => string.Join(" ", Symbols);
        }

        #region debug
        public bool IsDbgOutStack = false;
        public bool IsDbgOutSteps = false;

        void DbgOutWhenDone(IVGrammarTextCursor cursor, ProductionStack stack, bool is_ok)
        {
            if (IsDbgOutStack)
            {
                var prod_item = stack.Take();
                if (prod_item != null && prod_item.Prod != null)
                {
                    var msg = (is_ok ? "T" : "F") + $" [{stack.Size}] '{prod_item.Prod}'";
                    if (prod_item.Prod.ComponentsCount == 0)
                    {
                        if(!is_ok)
                            cursor.VGContext.DbgLog(msg);
                    }
                    else
                        cursor.VGContext.DbgLog(msg);
                }
            }
        }
        #endregion
        
        public IProd Produce(Grammar grammar, IVGrammarTextCursor cursor, CancellationToken ct)
        {
            // initializing
            ProductionStack stack = cursor.VGContext.ExecStack;
            IVGrammarProdFactory factory = cursor.VGContext.Factory;

            if (factory.TryGetNonTermProduct(grammar.RootSymbol, cursor, out INonTermProd root_nt_prod))
            {
                grammar.AssertRulesFound(grammar.RootSymbol);
                stack.Push(root_nt_prod, grammar[grammar.RootSymbol], cursor);
            }
            else
                throw new InvalidOperationException($"Unable create product for '{grammar.RootSymbol}' root symbol");
            // processing
            try
            {
                while (!stack.IsEmpty)
                {
                    ct.ThrowIfCancellationRequested();
                    var cur_exec = stack.Take();
                    if(IsDbgOutSteps)
                        cursor.VGContext.DbgLog($"* {cur_exec}");
                    if (cur_exec.IsOk)
                    {
                        if (cur_exec.IsRuleFinished)
                        { // rule is done
                            DbgOutWhenDone(cursor, stack, is_ok:true);
                            stack.RemoveLast();
                            if (stack.IsEmpty)
                                return cur_exec.Prod;
                            var new_exec = stack.Take();
                            if (cur_exec.Prod.IsAddToResult)
                                new_exec.Prod.AddComponent(cur_exec.Prod);
                            continue;
                        }
                    }
                    else
                    { // rule failed
                        if (!cur_exec.TryNextRule())
                        { // no more rules - reject symbol
                            DbgOutWhenDone(cursor, stack, is_ok: false);
                            stack.RemoveLast();
                            if (stack.IsEmpty)
                                return null;
                            cur_exec = stack.Take();
                            cur_exec.IsOk = false;
                        }
                        continue;
                    }
                    // checks
                    var cur_symbol = cur_exec.ExtractRuleSymbol();
                    bool is_ok = factory.TryGetNonTermProduct(cur_symbol, cursor, out INonTermProd nt_prod);
                    if (is_ok)
                        stack.Push(nt_prod, grammar[cur_symbol], cursor);
                    else
                    {
                        is_ok = factory.TryGetTermProduct(cur_symbol, cursor, out ITermProd term_prod);
                        if (is_ok)
                        {
                            if (term_prod.IsAddToResult)
                                cur_exec.Prod.AddComponent(term_prod);
                        }
                    }
                    cur_exec.IsOk = is_ok;
                }
            }
            catch (Exception ex)
            {
                cursor.VGContext.DbgLog($"Produce '{grammar.RootSymbol}' error:{ex.Message}");
                cursor.VGContext.DbgLog($"Cursor '{cursor}'");
                cursor.VGContext.DbgLog($"Stack:\n {stack}");
                throw;
            }
            return null;
        }
    }
}
