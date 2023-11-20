using GameData;

public class RoomPlayerRemoveArgs : EventArgs
{
    ushort RoomId { get; }  
    Player RemovedPlayer { get; }

    public RoomPlayerRemoveArgs(ushort roomId, Player removedPlayer)
    {
        RoomId = roomId;
        RemovedPlayer = removedPlayer;
    }
}
