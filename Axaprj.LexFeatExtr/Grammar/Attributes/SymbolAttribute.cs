using System;
using System.Globalization;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>
    /// Grammar Symbol Attribute
    /// <see cref="System.Attribute">custom attribute</see> based.
    /// </summary>
    public abstract class SymbolAttribute : Attribute, IGAttrSymbol
    {
        public readonly bool IsAddToResult;
        CultureInfo _lng = CultureInfo.InvariantCulture;
        string _lngName = string.Empty;

        public CultureInfo Lng {
            get => _lng;
            set {
                _lng = value;
                _lngName = _lng.Name;
            }
        }
        public string LngName {
            get => _lngName;
            set => Lng = CultureInfo.GetCultureInfo(value);
        }

        public SymbolAttribute(bool addToResult = true)
        {
            IsAddToResult = addToResult;
            Lng = CultureInfo.InvariantCulture;
        }
    }
}
