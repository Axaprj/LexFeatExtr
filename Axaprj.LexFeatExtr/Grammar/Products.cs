using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar
{
    public abstract class Prod : IProd
    {
        public bool IsAddToResult { get; }

        public object Symbol { get; }

        public bool IsEqualBySymbol(IProd prod) =>
            this.Symbol.ToString() == prod.Symbol.ToString();

        public bool IsEqualBySymbol(object symbol) =>
            this.Symbol.ToString() == symbol.ToString();

        public override string ToString() =>
            Symbol.ToString();

        protected Prod(object symbol, bool is_add_to_result = true)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            IsAddToResult = is_add_to_result;
        }
    }

    public class NonTermProd : Prod, INonTermProd
    {
        static readonly IEnumerable<IProd> Empty = new IProd[] { };
        List<IProd> _components;

        public bool MergeComponents { get; }

        public NonTermProd(object symbol, bool mergeComponents, bool addToResult)
            : base(symbol, addToResult)
        {
            MergeComponents = mergeComponents;
        }

        public IEnumerable<IProd> Components =>
            _components ?? Empty;

        public int ComponentsCount =>
            _components == null ? 0 : _components.Count;

        protected virtual bool IsAddComponents(INonTermProd prod) =>
            IsEqualBySymbol(prod) || prod.MergeComponents;

        public void ClearComponent()
        {
            if (_components != null)
                _components.Clear();
        }

        public void AddComponent(IProd prod)
        {
            if (this == prod)
                throw new InvalidOperationException(
                    $"Attempt to add your components to itself {this}");
            if (prod is INonTermProd nt_prod)
            {
                if (IsAddComponents(nt_prod))
                {
                    var add_comps = nt_prod.Components;
                    if (add_comps != null)
                    {
                        _components = _components
                            ?? new List<IProd>(nt_prod.ComponentsCount);
                        _components.AddRange(add_comps);
                    }
                    return;
                }
            }
            _components = _components ?? new List<IProd>();
            _components.Add(prod);
        }

        public override string ToString()
        {
            var lst = string.Empty;
            if (_components != null)
            {
                foreach (var c in Components)
                {
                    if (!string.IsNullOrEmpty(lst))
                        lst += " ";
                    lst += c == null ? "null" : c.ToString();
                }
            }
            return base.ToString() + "[" + lst + "]";
        }
    }

    public class TermProd : Prod, ITermProd
    {
        public string DisplayName { get; set; }
        public TermProd(object symbol, bool is_add_to_result = true)
            : base(symbol, is_add_to_result: is_add_to_result) { }
    }

    public class TermValue<TVal> : TermProd, ITermValue<TVal>
    {
        public TermValue(object symbol, TVal value, bool is_add_to_result = true)
            : base(symbol, is_add_to_result: is_add_to_result)
        {
            Value = value;
        }

        public TVal Value { get; }

        public override string ToString()
        {
            var msg = Value == null ? "null" : Value.ToString();
            if (DisplayName != null)
            {
                msg = $"{DisplayName}:" + msg;
            }
            return msg;
        }
    }

    public class TermMultiValue : TermValue<Dictionary<string, ITermProd>>
    {
        public TermMultiValue(object symbol, Dictionary<string, ITermProd> value, bool is_add_to_result = true)
            : base(symbol, value, is_add_to_result) { }

        public override string ToString()
        {
            string msg = null;
            if (Value == null || Value.Count == 0)
                msg = "empty";
            else
            {
                foreach (var v in Value)
                {
                    msg = msg == null ? "[" : (msg + ", ");
                    msg += $"{v.Key}: {v.Value}";
                }
                msg += "]";
            }
            if (DisplayName != null)
            {
                msg = $"{DisplayName}:" + msg;
            }
            return msg;
        }
    }
}
