using GameData;

public class RoomManager
{
    public List<Room> rooms = new();
    int maxRoomIndex = -1;

    int maxRoomSize;

    public EventHandler<RoomPlayerAddedArgs>? onRoomPlayerAdded;
    public EventHandler<RoomPlayerRemoveArgs>? onRoomPlayerRemoved;


    public RoomManager(int MaxRoomSize = 2, EventHandler<RoomPlayerAddedArgs>? eventHandler = null, EventHandler<RoomPlayerRemoveArgs>? removeHandler = null)
    {
        this.maxRoomSize = MaxRoomSize;
        if (eventHandler != null)
        {
            this.onRoomPlayerAdded = eventHandler;
        }
        if (removeHandler != null)
        {
            this.onRoomPlayerRemoved = removeHandler;
        }
        CreateNewRoom(); // we want to always have one free room
    }
    public int GetAvaliableRoomIndex()
    {
        for (int i=0; i<rooms.Count; i++)
        {
            if (!rooms[i].isFull())
            {
                return i;
            }
        }
        return -1; // not found
    }

    public void CreateNewRoom()
    {
        maxRoomIndex++;
        Room r = new(maxRoomSize);
        r.SetID((ushort)maxRoomIndex);
        r.onRoomPlayerAdded += onRoomPlayerAdded;
        rooms.Add(r);
    }

    public void AddPlayer(Player p)
    {
        int idx = GetAvaliableRoomIndex();
        if (idx == -1)
        {
            CreateNewRoom();
            AddPlayer(p); // retry adding player
        }
        else
        {
            rooms[idx].AddPlayer(p);
        }
    }

    public void RemovePlayer(Player p)
    {
        Room? r = rooms.Where(room => room.playerList.Contains(p)).FirstOrDefault();
        r?.RemovePlayer(p);
        GC();
    }

    // removes empty rooms
    public void GC()
    {
        for (int i = rooms.Count - 1;  i>-1; i--)
        {
            if (rooms[i].isEmpty())
            {
                Console.WriteLine($"Deleting room {rooms[i].GetID()}");
                rooms.RemoveAt(i);
                Console.WriteLine($"Deleted room , new count {rooms.Count}");

            }
            else
            {
                Console.WriteLine($"Room {rooms[i].GetID()} is not empty");
            }
        }
        maxRoomIndex = rooms.Count;
        Console.WriteLine($"End GC, max room index : {maxRoomIndex}");
    }

    public int GetRoomIndexByID(ushort roomID)
    {
        int roomIndex = -1;
        for (int i =0; i < rooms.Count; i++)
        {
            if (rooms[i].GetID() ==  roomID)
            {
                roomIndex = i; break;
            }
        }
        return roomIndex;
    }
}
