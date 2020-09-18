using Axaprj.LexFeatExtr.Grammar;
using Axaprj.LexFeatExtr.Grammar.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.LexFeatExtr.Tests
{
    public class ConsumptionDemo : TestBase
    {
        protected override Productions Prod
            => new ProdsConsumption();

        protected override Productions.Grammar Grammar
            => new ProdsConsumption.ConsumptionGrammar();

        public ConsumptionDemo(ITestOutputHelper output) : base(output) { }

        enum SYM
        {
            [NonTerm]
            Text,
            [AnyToken(addToResult: false)]
            token,
            [CollectCount]
            finished,
            [Empty]
            eof,
            [NonTerm]
            Consumption,
            [NonTerm]
            Val,
            [NonTerm]
            Vals,
            [NonTerm]
            CO2Val,
            [SpecCharToken(CHSP.Punctuations)]
            separator
        }

        public enum SymUoM
        {
            [TextC(
               ":Word(liters) :Word(per) :Word(100) :Word(kilometers)",
               ":Word(L,l) :Word(/,per) :Word(100) :Word(km)")]
            l100km,
            [TextC(
               ":Word(grams) :Word(per) :Word(kilometer)",
               ":Word(g) :Word(/) :Word(km)")]
            gkm,
            [TextC(
               ":Word(mpg) :Word?(US)")]
            mpg,
            [TextC("val:Decimal")]
            val,
            [TextC("vfrom:Decimal :Word(-) vto:Decimal")]
            valrange
        }

        public enum SymTxt
        {
            [TextC(
               ":Word(fuel) :Word(consumption)")]
            consumption,
            [TextC(
               ":Word(combined)")]
            combined,
            [TextC(
               ":Word(urban)")]
            urban,
            [TextC(":Word(extra) :Word?(-) :Word(urban)", ":Word(extraurban)")]
            exurban
        }

        public class ProdsConsumption : Productions
        {

            internal class ConsumptionGrammar : Grammar
            {
                public ConsumptionGrammar() : base(SYM.Text)
                {
                    AddRULE(SYM.Text,
                        EXPR(SYM.Consumption, SYM.Text),
                        EXPR(SYM.CO2Val, SYM.Text),
                        EXPR(SYM.token, SYM.Text),
                        EXPR(SYM.eof)
                    );
                    AddRULE(SYM.Consumption,
                        EXPR(RANGE(1, 0, SymTxt.combined), SymTxt.consumption, RANGE(0, 6, SYM.token), SYM.Vals),
                        EXPR(RANGE(1, 0, SymTxt.exurban), SymTxt.consumption, RANGE(0, 6, SYM.token), SYM.Vals),
                        EXPR(RANGE(1, 0, SymTxt.urban), SymTxt.consumption, RANGE(0, 6, SYM.token), SYM.Vals)
                    );
                    AddRULE(SYM.Vals,
                        EXPR(SYM.Val, RANGE(1, 0, SYM.separator), SYM.Vals),
                        EXPR(SYM.Val),
                        EXPR(SYM.finished)
                    );
                    AddRULE(SYM.Val,
                        EXPR(RANGE(1, 0, SymTxt.combined), SymUoM.valrange, SymUoM.l100km),
                        EXPR(RANGE(1, 0, SymTxt.exurban), SymUoM.valrange, SymUoM.l100km),
                        EXPR(RANGE(1, 0, SymTxt.urban), SymUoM.valrange, SymUoM.l100km),
                        EXPR(RANGE(1, 0, SymTxt.combined), SymUoM.val, SymUoM.l100km),
                        EXPR(RANGE(1, 0, SymTxt.exurban), SymUoM.val, SymUoM.l100km),
                        EXPR(RANGE(1, 0, SymTxt.urban), SymUoM.val, SymUoM.l100km),
                        EXPR(SymUoM.valrange, SymUoM.mpg),
                        EXPR(SymUoM.val, SymUoM.mpg)
                    );
                    AddRULE(SYM.CO2Val,
                        EXPR(SymUoM.val, SymUoM.gkm),
                        EXPR(SymUoM.valrange, SymUoM.gkm)
                    );
                }
            }
        }

        string Txt1 = "its combined fuel consumption is reduced to up to 2.4 liters per 100 kilometers ( 98 mpg US ) ,"
            + " with CO 2 emissions of up to 56 grams per kilometer .";
        string Txt2 = "The lowest combined fuel consumption for the range is 7.9 L / 100 km for LS-U and LS-T 4 x 4 autos " +
            "( 209 CO 2 emissions ) , and peaks at 8.1 L / 100 km ( for all LS-T and LS-U 4 x 2 and LS-M 4 x 4 ) .";
        string Txt3 = "Its combined fuel consumption is 141.2 - 122.8 mpg , while CO 2 emissions are at least 49 g / km .";
        string Txt4 = "NEDC combined fuel consumption is 4.2 - 4.1 l / 100 km " +
            "( preliminary fuel consumption NEDC 1 : urban 4.9 - 4.8 l / 100 km ; extra - urban 3.8 - 3.6 l / 100 km ;" +
            " combined 4.2 - 4.1 l / 100 km , 95 - 93 g / km CO 2 ;";

        void ProduceConsumption(string txt)
        {
            var prod = Produce(txt);
            Assert.NotNull(prod);
            Log($"Prod: '{prod}'");
        }

        [Fact]
        public void ConsumptionTest()
        {
            ProduceConsumption(Txt1);
            ProduceConsumption(Txt2);
            ProduceConsumption(Txt3);
            ProduceConsumption(Txt4);
        }

    }
}
