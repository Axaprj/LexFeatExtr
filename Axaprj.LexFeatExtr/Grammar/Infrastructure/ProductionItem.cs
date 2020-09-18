using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Infrastructure
{
    public class ProductionItem
    {
        public readonly INonTermProd Prod;
        readonly List<Productions.Expression> Rules;
        readonly IVGrammarTextCursor Cursor;
        readonly int posCursorStart;
        readonly int posCursorStop;

        int inxRule;
        int inxSym;
        public bool IsOk = true;

        public ProductionItem(INonTermProd prod, List<Productions.Expression> rules, IVGrammarTextCursor cursor)
        {
            Prod = prod;
            Rules = rules;
            Cursor = cursor;
            cursor.GetRange(out posCursorStart, out posCursorStop);
            inxRule = 0;
            inxSym = 0;
        }

        public bool TryNextRule()
        {
            Prod.ClearComponent();
            Cursor.SetRange(posCursorStart, posCursorStop);
            inxRule++;
            inxSym = 0;
            IsOk = true;
            return inxRule < Rules.Count;
        }

        public bool IsRuleFinished => 
            inxSym >= Rules[inxRule].Symbols.Length; 

        public object ExtractRuleSymbol()
        {
            var rule = Rules[inxRule];
            var sym = IsRuleFinished ? null : rule.Symbols[inxSym];
            inxSym++;
            return sym;
        }

        public override string ToString()
        {
            var rule_msg = "NA";
            if (inxRule < Rules.Count)
            {
                rule_msg = $"Rule[{Rules[inxRule]}]";
                if (IsRuleFinished)
                    rule_msg = $"Finished {rule_msg}";
                else
                {
                    var rsymbs = Rules[inxRule].Symbols;
                    rule_msg = $"Symb[{rsymbs[inxSym]}] {rule_msg}";
                }
            }
            return (IsOk ? "T" : "F") + $" {rule_msg} Prod:{Prod}";
        }
    }
}
