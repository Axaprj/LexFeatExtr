using Takenet.Textc;

namespace Axaprj.LexFeatExtr.Grammar
{
    /// <summary>
    /// Cursor for grammar processing.
    /// Note: special processing of <see cref="VTextCursor.All"/>.
    /// </summary>
    public interface IVGrammarTextCursor : ITextCursor
    {
        /// <summary>
        /// Reset all processing markers
        /// </summary>
        void ReInitialize();
        /// <summary>
        /// Setup start to next token
        /// </summary>
        /// <returns>false on the end of stream</returns>
        bool StartFromNextToken(int token_cnt = 1);
        /// <summary>
        /// Setup start to first remaining token
        /// </summary>
        /// <returns>false on the end of stream</returns>
        bool StartFromRemainingTokens();
        /// <summary>
        /// Get current processing token range
        /// </summary>
        /// <param name="pos_start"></param>
        /// <param name="pos_stop"></param>
        void GetRange(out int pos_start, out int pos_stop);
        /// <summary>
        /// Set processing token range
        /// </summary>
        /// <param name="pos_start"></param>
        /// <param name="pos_stop"></param>
        void SetRange(int pos_start, int pos_stop);
        /// <summary>Grammar Request Context</summary>
        IVGrammarRequestContext VGContext { get; }
    }

    public class VGrammarTextCursor : VTextCursor, IVGrammarTextCursor
    {
        int _leftRemainigTextPos = int.MaxValue;
        int _rightRemainigTextPos = int.MinValue;

        public IVGrammarRequestContext VGContext => 
            (IVGrammarRequestContext) Context;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="inputTokens">The input text tokens.</param>
        /// <param name="context">The context.</param>
        /// <param name="tokenSeparator">The token separator</param>
        /// <exception cref="System.ArgumentNullException">inputText</exception>
        public VGrammarTextCursor(string inputText, IVGrammarRequestContext context, char tokenSeparator = ' ')
            : base(inputText, context, tokenSeparator)
        {
        }
        
        public bool StartFromNextToken(int token_cnt = 1)
        {
            bool isok = false;
            if (isok = Tokens.Length > PosStart)
            {
                IncRangePos(posStartDelta: token_cnt);
                Reset();
            }
            return isok;
        }

        public bool StartFromRemainingTokens()
        {
            SetRangePos(posStart: _leftRemainigTextPos, posStop: _rightRemainigTextPos);
            Reset();
            return Tokens.Length > PosStart;
        }

        /// <summary>
        /// hack: block and refactoring remaining text functionality
        /// </summary>
        /// <returns></returns>
        public override string All()
        {
            _leftRemainigTextPos = LeftPos;
            _rightRemainigTextPos = RightPos;
            SetupEmptyPosition();
            return IsEmpty ? string.Empty : "IVGrammarTextCursor does not support RemainingText function";
        }

        public void GetRange(out int start_pos, out int stop_pos)
        {
            start_pos = this.PosStart;
            stop_pos = this.PosStop;
        }

        public void SetRange(int start_pos, int stop_pos)
        {
            SetRangePos(start_pos, stop_pos);
            Reset();
        }

        public void ReInitialize()
        {
            _leftRemainigTextPos = int.MaxValue;
            _rightRemainigTextPos = int.MinValue;
            SetRangePos();
            Reset();
        }
    }
}
