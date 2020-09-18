using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar
{
    /// <summary> Garammar Product </summary>
    public interface IProd
    {
        /// <summary> Add to result collection flag </summary>
        bool IsAddToResult { get; }
        /// <summary> Grammar Symbol </summary>
        object Symbol { get; }

        bool IsEqualBySymbol(IProd prod);

        bool IsEqualBySymbol(object symbol);
    }
    /// <summary> Non Terminal Garammar Product (product collection) </summary>
    public interface INonTermProd : IProd
    {
        /// <summary> 
        /// Merge equal by symbol components instead of adding as component 
        /// (build result option)
        /// </summary>
        bool MergeComponents { get; }

        IEnumerable<IProd> Components { get; }

        void ClearComponent();

        void AddComponent(IProd prod);

        int ComponentsCount { get; }
    }
    /// <summary>Terminal Garammar Product</summary>
    public interface ITermProd : IProd
    {
    }
    /// <summary>Terminal Garammar Product (generic value type)</summary>
    public interface ITermValue<TVal> : ITermProd
    {
        /// <summary>Terminal Product Value</summary>
        TVal Value { get; }
    }

}
