using GameData;

public class Room
{
    int maxPlayers;
    public List<Player> playerList;
    ushort id;
    Ball ball;
    public EventHandler<RoomPlayerAddedArgs>? onRoomPlayerAdded;
    public EventHandler<RoomPlayerRemoveArgs>? onRoomPlayerRemoved;

    public Room(int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
        this.playerList = new();
    }

    public void AddPlayer(Player player)
    {
        if (playerList.Count <= maxPlayers)
        {
            playerList.Add(player);
            this.onRoomPlayerAdded?.Invoke(this, new RoomPlayerAddedArgs(id, player));
        }
        else
        {
            Console.WriteLine($"Room full {id} , can't add anyone ");
        }
    }

    public void RemovePlayer(Player p)
    {
        Player? player = playerList.Where(player => player.id == p.id).FirstOrDefault();

        if (player != null)
        {
            playerList.Remove(player);
        }
        else
        {
            Console.WriteLine("No players to be removed");
        }
    }

    public bool isFull()
    {
        return this.playerList.Count == maxPlayers;
    }

    public bool isEmpty()
    {
        return this.playerList.Count == 0;
    }

    public ushort GetID()
    {
        return id;
    }

    public void SetID(ushort id)
    {
        this.id = id;
    }
}
