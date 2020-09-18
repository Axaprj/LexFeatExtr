using System;
using System.Collections.Generic;
using System.Text;

namespace Axaprj.LexFeatExtr.Grammar.Attributes
{
    /// <summary>
    /// Character kinds (Chars Specification)
    /// </summary>
    [Flags]
    public enum CHSP
    {
        /// <summary> <see cref="Char.IsLetter(char)"/> </summary>
        Letters = 0,
        /// <summary> <see cref="Char.IsDigit(char)"/> </summary>
        Digits = 1,
        /// <summary> <see cref="Char.IsPunctuation(char)"/> </summary>
        Punctuations = 2,
        /// <summary> <see cref="Char.IsControl(char)"/> </summary>
        Controls = 4,
        /// <summary> <see cref="Char.IsSymbol(char)"/> </summary>
        Symbols = 8,
        /// <summary> <see cref="Char.IsSeparator(char)"/> </summary>
        Separators = 16
    }

    /// <summary>
    /// Input token with specified character set
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SpecCharTokenAttribute : TermAttribute
    {
        readonly CHSP CharSet;

        public SpecCharTokenAttribute(CHSP chars, bool addToResult = true) : base(addToResult)
        {
            CharSet = chars;
        }

        bool IsMatchedChars(string token)
        {
            for (int ch_inx = 0; ch_inx < token.Length; ch_inx++)
            {
                char ch = token[ch_inx];
                bool is_matched = (CharSet.HasFlag(CHSP.Letters) && Char.IsLetter(ch))
                    || (CharSet.HasFlag(CHSP.Digits) && Char.IsDigit(ch))
                    || (CharSet.HasFlag(CHSP.Punctuations) && Char.IsPunctuation(ch))
                    || (CharSet.HasFlag(CHSP.Controls) && Char.IsControl(ch))
                    || (CharSet.HasFlag(CHSP.Symbols) && Char.IsSymbol(ch))
                    || (CharSet.HasFlag(CHSP.Separators) && Char.IsSeparator(ch))
                    ;
                if (!is_matched)
                    return false;
            }
            return true;
        }

        public override bool TryCreateProduct(object symbol, IVGrammarTextCursor cursor, out ITermProd prod)
        {
            prod = null;
            if (IsMatched(symbol, cursor))
            {
                prod = new TermValue<string>(symbol, cursor.Peek(), IsAddToResult);
                cursor.StartFromNextToken();
            }
            return prod != null;
        }

        public override bool IsMatched(object symbol, IVGrammarTextCursor cursor) =>
            !cursor.IsEmpty && IsMatchedChars(cursor.Peek());

    }
}
