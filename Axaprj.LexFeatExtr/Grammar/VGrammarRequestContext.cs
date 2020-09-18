using Axaprj.LexFeatExtr.Grammar.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Takenet.Textc;

namespace Axaprj.LexFeatExtr.Grammar
{
    /// <summary> Grammar Request Context (Textc compatible)</summary>
    public interface IVGrammarRequestContext : IRequestContext
    {
        /// <summary> Debug Logger handler (optional) </summary>
        Action<string> DbgLog { get; set; }
        /// <summary> Recursion Stack (infrastructure) </summary>
        ProductionStack ExecStack { get; }
        /// <summary> Products Detector and Factory </summary>
        IVGrammarProdFactory Factory { get; set; }
    }

    public class VGrammarRequestContext : RequestContext, IVGrammarRequestContext
    {
        ProductionStack _execStack;
        public ProductionStack ExecStack =>
            _execStack = _execStack ?? new ProductionStack();

        public VGrammarRequestContext():this(CultureInfo.InvariantCulture)
        {            
        }

        public VGrammarRequestContext(CultureInfo culture):base(culture)
        {
            DbgLog = (msg) => { };
        }
        
        public Action<string> DbgLog { get; set; }

        IVGrammarProdFactory _factory;
        public virtual IVGrammarProdFactory Factory
        {
            get
            {
                if (_factory == null)
                    _factory = new VGrammarProdFactory();
                return _factory;
            }
            set => _factory = value;
        }
    }
}
