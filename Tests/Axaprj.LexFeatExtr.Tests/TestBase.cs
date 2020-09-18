using Axaprj.LexFeatExtr.Grammar;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Axaprj.LexFeatExtr.Tests
{
    public abstract class TestBase
    {
        readonly ITestOutputHelper _output;
        protected abstract Productions Prod { get; }
        protected abstract Productions.Grammar Grammar { get; }

        public TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected void Log(string msg)
        {
            msg = DateTime.Now.ToShortTimeString() + ": " + msg;
            _output.WriteLine(msg);
        }

        protected IProd Produce(string txt)
        {
            Log($"Src: {txt}");
            var ctx = new VGrammarRequestContext();
            ctx.DbgLog = (msg) => Log(msg);
            var cursor = new VGrammarTextCursor(txt, ctx);
            return Prod.Produce(Grammar, cursor, CancellationToken.None);
        }
    }
}







