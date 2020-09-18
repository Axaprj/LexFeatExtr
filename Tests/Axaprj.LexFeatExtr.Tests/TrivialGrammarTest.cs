using Axaprj.LexFeatExtr.Grammar;
using Axaprj.LexFeatExtr.Grammar.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.LexFeatExtr.Tests
{
    public class TrivialGrammarTest : TestBase
    {
        protected override Productions Prod 
            => new ProdsTrivial() { IsDbgOutStack = true, IsDbgOutSteps = true };

        protected override Productions.Grammar Grammar 
            => new ProdsTrivial.TrivialGrammar();

        public TrivialGrammarTest(ITestOutputHelper output) : base(output) { }

        enum SYM
        {
            [NonTerm]
            Text,
            [AnyToken]
            token,
            [Empty]
            eof
        }

        public class ProdsTrivial : Productions
        {
            internal class TrivialGrammar : Grammar
            {
                public TrivialGrammar() : base(SYM.Text)
                {
                    AddRULE(SYM.Text,
                        EXPR(SYM.token, SYM.Text),
                        EXPR(SYM.eof)
                    );
                }
            }
        }

        [Fact]
        public void DevGTrivialTest()
        {
            var txt = "one two three";
            var prod = Produce(txt);
            Assert.NotNull(prod);
            Log($"Prod: '{prod.ToString()}'");
        }
        
    }
}
