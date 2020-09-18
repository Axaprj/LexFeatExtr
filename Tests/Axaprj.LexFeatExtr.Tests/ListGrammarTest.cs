using Axaprj.LexFeatExtr.Grammar;
using Axaprj.LexFeatExtr.Grammar.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.LexFeatExtr.Tests
{
    public class ListGrammarTest : TestBase
    {
        protected override Productions Prod
            => new ProdsList();

        protected override Productions.Grammar Grammar
            => new ProdsList.ListGrammar();

        public ListGrammarTest(ITestOutputHelper output) : base(output) { }

        public enum SYM
        {
            [NonTerm]
            Text,
            [AnyToken(addToResult: false)]
            ignore,
            [NonTerm]
            List,
            [NonTerm]
            Item,
            [SpecCharToken(CHSP.Letters | CHSP.Digits)]
            itemTerm,
            [Empty]
            eof
        }
                    
        public partial class ProdsList : Productions
        {
            internal class ListGrammar : Grammar
            {
                public ListGrammar() : base(SYM.Text)
                {
                    AddRULE(SYM.Text,
                        EXPR("(", SYM.List, ")", SYM.Text),
                        EXPR(SYM.ignore, SYM.Text),
                        EXPR(SYM.eof)
                    );
                    AddRULE(SYM.List,
                        EXPR(SYM.Item, ",", SYM.List),
                        EXPR(SYM.Item)
                    );
                    AddRULE(SYM.Item,
                        EXPR(SYM.itemTerm),
                        EXPR("(", SYM.List, ")")
                    );
                }
            }
        }

        [Fact]
        public void DevGListTest1()
        {
            var prod = Produce("( one , two )");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(SYM.List, out List<IProd> items_prod));
            Assert.Equal(2, ((INonTermProd)items_prod[0]).ComponentsCount);
        }

        [Fact]
        public void DevGListTest2()
        {
            var prod = Produce("list ( one , two ) test");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(SYM.List, out List<IProd> items_prod));
            Assert.Equal(2, ((INonTermProd)items_prod[0]).ComponentsCount);
        }

        [Fact]
        public void DevGListTest3()
        {
            var prod = Produce("list bad ( one , two , three test ( four , five ) ");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(SYM.List, out List<IProd> prod_items));
            Assert.Single(prod_items);
            Assert.Equal(2, ((INonTermProd)prod_items[0]).ComponentsCount);
        }

        [Fact]
        public void DevGListTest4()
        {
            var prod = Produce("list ( one , two , three , ( four , five ) ) ");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(SYM.List, out List<IProd> prod_items));
            Assert.Equal(2, prod_items.Count);
            int fn(IProd p) => ((INonTermProd)p).ComponentsCount;
            Assert.Equal(4, prod_items.Max(p => fn(p)));
            Assert.Equal(2, prod_items.Min(p => fn(p)));
        }
    }
}
