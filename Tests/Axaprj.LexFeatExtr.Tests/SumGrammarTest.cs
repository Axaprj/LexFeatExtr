using Axaprj.LexFeatExtr.Grammar;
using Axaprj.LexFeatExtr.Grammar.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.LexFeatExtr.Tests
{
    public class SumGrammarTest : TestBase
    {

        public SumGrammarTest(ITestOutputHelper output) : base(output) { }

        protected override Productions Prod
            => new ProdsSum();

        protected override Productions.Grammar Grammar
            => new ProdsSum.SumGrammar();


        public enum SYM
        {
            [NonTerm]
            Text,
            [AnyToken(addToResult: false)]
            ignore,
            [Empty]
            eof,
            [CollectCount]
            finished
        }

        public enum TcSYM
        {
            [TextC(":Word(sums)")]
            sum,
            [TextC(":Word(and) num:Integer)", "num:Integer")]
            num,
            [NonTerm]
            Args
        }

        public partial class ProdsSum : Productions
        {
            internal class SumGrammar : Grammar
            {
                public SumGrammar() : base(SYM.Text)
                {
                    AddRULE(SYM.Text,
                        EXPR(TcSYM.sum, TcSYM.Args, SYM.Text),
                        EXPR(TcSYM.Args, TcSYM.sum, SYM.Text),
                        EXPR(SYM.ignore, SYM.Text),
                        EXPR(SYM.eof)
                    );
                    AddRULE(TcSYM.Args,
                        EXPR(TcSYM.num, TcSYM.Args),
                        EXPR(SYM.finished)
                    );
                }
            }
        }

        [Fact]
        public void DevSumTest1()
        {
            var prod = Produce("sums 5 6 ");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(TcSYM.sum, out List<IProd> prod_sum));
            Assert.Single(prod_sum);
            Assert.True(prod.TryFind(TcSYM.Args, out List<IProd> prod_args));
            Assert.Single(prod_args);
            var args = ((INonTermProd)prod_args[0]);
            Assert.Equal(2, args.ComponentsCount);
        }

        [Fact]
        public void DevSumTest2()
        {
            var prod = Produce("sums 5 6 and 7");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(TcSYM.sum, out List<IProd> prod_sum));
            Assert.Single(prod_sum);
            Assert.True(prod.TryFind(TcSYM.Args, out List<IProd> prod_args));
            Assert.Single(prod_args);
            var args = ((INonTermProd)prod_args[0]).Components;
            Assert.NotNull(args);
            Assert.Equal(18, args.Sum(iterm => ((ITermValue<int>)iterm).Value));
        }

        [Fact]
        public void DevSumTest3()
        {
            var prod = Produce("5 6 and 7 sums");
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
            Assert.True(prod.TryFind(TcSYM.sum, out List<IProd> prod_sum));
            Assert.Single(prod_sum);
            Assert.True(prod.TryFind(TcSYM.Args, out List<IProd> prod_args));
            Assert.Single(prod_args);
            var args = ((INonTermProd)prod_args[0]).Components;
            Assert.NotNull(args);
            Assert.Equal(18, args.Sum(iterm => ((ITermValue<int>)iterm).Value));
        }
    }
}
