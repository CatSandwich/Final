using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalCommon
{
    public enum ServerToClient : ushort
    {
        MoveObject
    }

    public enum ClientToServer : ushort
    {
        PaddlePosition
    }
}
