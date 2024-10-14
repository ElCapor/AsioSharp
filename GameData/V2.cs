namespace GameData
{
    /// <summary>
    /// Sever <=> Client messages
    /// </summary>
    public enum V2Messages : ushort
    {
        RoomAssign, // room reassignement
    }

    /// <summary>
    /// A player only has access to these messages if
    /// he is inside a room
    /// Room <=> Player
    /// </summary>
    public enum R2Messages : ushort
    {
        PlayerJoined, // a player joined the room
        PlayerLeft, // a player left the room
        SendRoomState, // A client can request a room state
    }
}