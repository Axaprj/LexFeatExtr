using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.Processors;
using Takenet.Textc.Types;

namespace Axaprj.LexFeatExtr.Detectors
{
    /// <summary>
    /// Tokens matched detector, Takenet.Textc syntax patterns based
    /// </summary>
    public class DetectorTextC : IOutputProcessor, IDetectorTextC
    {
        readonly ISyntaxParser _syntaxParser;
        protected readonly Syntax _syntax;

        /// <summary>Initializes a new instance</summary>
        public DetectorTextC(string syntaxPattern, CultureInfo culture)
            : this(syntaxPattern, new SyntaxParser(), culture) { }
        /// <summary>Initializes a new instance</summary>
        public DetectorTextC(string syntaxPattern, ISyntaxParser syntaxParser, CultureInfo culture)
        {
            _syntaxParser = syntaxParser ?? throw new ArgumentNullException(nameof(syntaxParser));
            var syntax = CsdlParser.Parse(syntaxPattern, culture);
            var fn = (Func<Task<string>>)(() => Task.FromResult(string.Empty));
            // Now create the command processors, to bind the methods to the syntaxes
            var cmd_proc = new DelegateCommandProcessor(
                fn,
                true,
                this,
                syntax
                );
            var syntaxes = cmd_proc.Syntaxes
                .Where(s => s.Culture.Equals(culture) || s.Culture.Equals(CultureInfo.InvariantCulture));
            if (syntaxes.Count() > 1)
                throw new InvalidOperationException(
                    $"Multiple syntax patterns '{syntaxPattern}' are not allowed here");
            _syntax = syntaxes.FirstOrDefault();
            if (_syntax == null)
                throw new InvalidOperationException(
                    $"Unable to build syntax pattern '{syntaxPattern}'");
        }

        Task IOutputProcessor.ProcessOutputAsync(object output, IRequestContext context, CancellationToken cancellationToken)
            => Task.Run(() => { });

        public bool IsMatched(ITextCursor textCursor, out Expression parsedExpr, CancellationToken cancellationToken)
        {
            if (textCursor == null)
                throw new ArgumentNullException(nameof(textCursor));
            parsedExpr = ParseInput(textCursor, cancellationToken);
            return parsedExpr != null;
        }

        Expression ParseInput(ITextCursor textCursor, CancellationToken cancellationToken)
        {
            IRequestContext context = textCursor.Context;
            // Gets all the syntaxes that are of the same culture of the context or are culture invariant
            cancellationToken.ThrowIfCancellationRequested();

            textCursor.RightToLeftParsing = _syntax.RightToLeftParsing;
            textCursor.Reset();

            if (_syntaxParser.TryParse(textCursor, _syntax, context, out Expression expression))
                return expression;
            return null;
        }
    }
}