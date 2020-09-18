using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Takenet.Textc;

namespace Axaprj.LexFeatExtr.Detectors
{
    /// <summary> Tokens matched <see cref="Takenet.Textc">Takenet.Textc</see> detector </summary>
    public interface IDetectorTextC
    {
        bool IsMatched(ITextCursor textCursor, out Expression parsedExpr, CancellationToken cancellationToken);
    }
}
