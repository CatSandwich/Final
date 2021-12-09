using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;

namespace FinalCommon.Data
{
    public interface IClientToServer
    {
        ClientToServer ClientToServerTag { get; }
    }

    public interface IServerToClient
    {
        ServerToClient ServerToClientTag { get; }
    }
}
