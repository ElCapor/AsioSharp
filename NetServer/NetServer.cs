using GameData;
using Riptide;

public class NetServer : Server
{
    public RoomManager roomManager;
    public Dictionary<int, Player> playerMap;
    public NetServer() : base()
    {
        Console.WriteLine("Created server");
        roomManager = new(2, OnRoomPlayerAdded);
        playerMap = new();
        
    }

    public void OnConnect(object? sender, ServerConnectedEventArgs args)
    {
        Console.WriteLine($"OnConnect {args.Client.Id}");
        Player p = new();
        p.id = args.Client.Id;
        playerMap[args.Client.Id] = p;
        roomManager.AddPlayer(p);
    }

    public void OnDisconnect(object? sender, ServerDisconnectedEventArgs args)
    {
        Console.WriteLine($"OnDisconnect {args.Client.Id}");
        roomManager.RemovePlayer(playerMap[args.Client.Id]);
        playerMap.Remove(args.Client.Id);
    }

    public Player? GetOtherPlayerInRoom(Room room, ushort playerId)
    {
        Player? otherPlayer = room.playerList.Where(player => player.id != playerId).FirstOrDefault();
        return otherPlayer;
    }

    public void OnRoomPlayerAdded(object? sender , RoomPlayerAddedArgs args)
    {
        Console.WriteLine($"Player {args.AddedPlayer.id} added to room {args.RoomId}");
        int clientID = playerMap.FirstOrDefault(p => p.Value.id == args.AddedPlayer.id).Key;
        Message msg = Message.Create(MessageSendMode.Reliable, ServerMessage.JoinedRoom);
        msg.AddUShort(args.RoomId);
        Send(msg, (ushort)clientID);

        Room r = roomManager.rooms[roomManager.GetRoomIndexByID(args.RoomId)];
        if (r.isFull())
        {
            Message full = Message.Create(MessageSendMode.Reliable, ServerMessage.SendRoomFull);
            Player? other = GetOtherPlayerInRoom(r, args.AddedPlayer.id);
            if (other != null)
            {
                full.AddUShort(other.id);
                Send(full, (ushort)clientID);

                Message full2 = Message.Create(MessageSendMode.Reliable, ServerMessage.SendRoomFull);
                int clientID2 = playerMap.FirstOrDefault(p => p.Value.id == other.id).Key;
                full2.AddUShort((ushort)args.AddedPlayer.id);
                Send(full2, (ushort)clientID2);

            }
            else
            {
                Console.WriteLine("what the fuck");
            }
        }
    }
    

    public void CreateRoom()
    {
        roomManager.CreateNewRoom(); // new room isnt subject to gc
    }

    public void ClearRoom(ushort roomId)
    {
        for (int i = roomManager.rooms[roomManager.GetRoomIndexByID(roomId)].playerList.Count - 1; i >= 0; i--)
        {
            ushort player_id = roomManager.rooms[roomManager.GetRoomIndexByID(roomId)].playerList[i].id;
            Player p = playerMap.FirstOrDefault(x => x.Value.id == player_id).Value;
            int clientID = playerMap.FirstOrDefault(p => p.Value.id == player_id).Key;
            Connection? connection = this.Clients.FirstOrDefault(p => p.Id == clientID);
            roomManager.rooms[roomManager.GetRoomIndexByID(roomId)].RemovePlayer(p);

            DisconnectClient(connection);
        }
    }
    
    public void DeleteRoom(ushort roomId)
    {
        if (roomManager.rooms[roomManager.GetRoomIndexByID(roomId)].isEmpty())
        {
            roomManager.rooms.RemoveAt(roomManager.GetRoomIndexByID(roomId));
            roomManager.maxRoomIndex = roomManager.rooms.Count;
        }
        else
        {
            ClearRoom(roomId); // will get gc'd
        }
    }
}
