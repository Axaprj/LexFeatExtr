using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Axaprj.LexFeatExtr.Grammar.Infrastructure
{
    public class ProductionStack
    {
        readonly LinkedList<ProductionItem> _stack;

        public ProductionStack()
        {
            _stack = new LinkedList<ProductionItem>();
        }

        public ProductionItem Take() => _stack.Last?.Value;

        public void RemoveLast() => _stack.RemoveLast();

        public int Size => _stack.Count;

        public bool IsEmpty => _stack.Count == 0; 

        public void Push(INonTermProd prod, List<Productions.Expression> rules, IVGrammarTextCursor cursor)
        {
            var itm = new ProductionItem(prod, rules, cursor);
            _stack.AddLast(itm);
        }

        public IEnumerable<INonTermProd> StackProd => 
            _stack.Select(itm => itm.Prod);
        /// <summary>
        /// Enumerates stack products from Last while predicate true
        /// </summary>
        /// <param name="fnCheckProd">enumeration predicate</param>
        public void InspectStack(Func<INonTermProd, bool> fnCheckProd)
        {
            var cur = _stack.Last;
            while (cur != null)
            {
                if (!fnCheckProd(cur.Value.Prod))
                    break;
                cur = cur.Previous;
            }
        }

        public override string ToString() =>
            string.Join("\n", _stack);
    }
}
