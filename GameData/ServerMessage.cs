using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    // server to client messae enum
    public enum ServerMessage : ushort
    {
        JoinedRoom, // player was sent to a room
        SendRoomFull, // the room is now full

    }
}
