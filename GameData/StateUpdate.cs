using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    public enum GameState : int
    {
        WaitingForAnotherPlayer,
        Started,
        Starting,
        Ended
    }
}
