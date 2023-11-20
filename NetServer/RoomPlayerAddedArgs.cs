using GameData;

public class RoomPlayerAddedArgs : EventArgs
{
    public ushort RoomId { get; }
    public Player AddedPlayer { get; }

    public RoomPlayerAddedArgs(ushort roomId, Player addedPlayer)
    {
        RoomId = roomId;
        AddedPlayer = addedPlayer;
    }
}
