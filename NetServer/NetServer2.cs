using GameData;
using Riptide;
using Riptide.Utils;
using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using rlImGui_cs;
using ImGuiNET;


/// <summary>
/// Event triggered when a client is added to a group
/// </summary>
/// <typeparam name="T">Client Type</typeparam>
public class RoomV2ClientAddedArgs<T> : EventArgs where T:class
{
    public ushort RoomId { get; }
    public T AddedClient { get; }

    public RoomV2ClientAddedArgs(ushort roomId, T addedClient)
    {
        RoomId = roomId;
        AddedClient = addedClient;
    }
}

/// <summary>
/// Event triggered when a client is removed to a group
/// </summary>
/// <typeparam name="T">Client Type</typeparam>
public class RoomV2ClientRemovedArgs<T> : EventArgs where T:class
{
    public ushort RoomId { get; }
    public T RemovedClient { get; }

    public RoomV2ClientRemovedArgs(ushort roomId, T removedClient)
    {
        RoomId = roomId;
        RemovedClient = removedClient;
    }
}

/// <summary>
/// A Room is just a group of clients
/// </summary>
/// <typeparam name="T">The type of clients in the room</typeparam>
public class RoomV2<T> where T:class
{
    int maxClients;

    public List<T> clientsList;

    ushort id;
    
    public EventHandler<RoomV2ClientAddedArgs<T>>? onRoomClientAdded;
    public EventHandler<RoomV2ClientRemovedArgs<T>>? onRoomClientRemoved;

    public RoomV2(int maxClients)
    {
        this.maxClients = maxClients;
        this.clientsList = new List<T>();
    }

    public bool AddClient(T client)
    {
        if (clientsList.Count <= maxClients)
        {
            clientsList.Add(client);
            this.onRoomClientAdded?.Invoke(this, new RoomV2ClientAddedArgs<T>(id, client));
            return true;
        } else {
            return false; // room is full
        }
    }

    public bool RemoveClient(T client)
    {
        T? cl = clientsList.Where(c => c == client).FirstOrDefault();
        if (cl != null)
        {
            return clientsList.Remove(cl);
        } else
        {
            return false;
        }
    }

    public bool IsFull()
    {
        return this.clientsList.Count == maxClients;
    }

    public bool IsEmpty()
    {
        return this.clientsList.Count == 0;
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

public delegate R RoomFactory<R, T>(int maxRoomSize) where R : RoomV2<T> where T : class;

public class RoomManagerV2<R, T> where R:RoomV2<T> where T: class
{
    public List<R> rooms = new();
    public int maxRoomIndex = -1;
    int maxRoomSize; // generic max room size

    public EventHandler<RoomV2ClientAddedArgs<T>>? onRoomClientAdded; // generic event
    public EventHandler<RoomV2ClientRemovedArgs<T>>? onRoomClientRemoved;

    private RoomFactory<R, T> roomFactory; // Delegate for creating rooms
    public RoomManagerV2(int maxRoomSize, RoomFactory<R, T> factory, EventHandler<RoomV2ClientAddedArgs<T>>? addHandler = null, EventHandler<RoomV2ClientRemovedArgs<T>>? removeHandler = null)
    {
        this.maxRoomSize = maxRoomSize;
        roomFactory = factory;
        if (addHandler != null)
        {
            this.onRoomClientAdded = addHandler;
        }
        if (removeHandler != null)
        {
            this.onRoomClientRemoved = removeHandler;
        }
        CreateNewRoom(); // we want to always have one free room
    }

    public int GetAvaliableRoomIndex()
    {
        for (int i=0; i<rooms.Count; i++)
        {
            if (!rooms[i].IsFull())
            {
                return i;
            }
        }
        return -1; // not found
    }

    public void CreateNewRoom()
    {
        maxRoomIndex++;
        R r = roomFactory(maxRoomSize);
        r.SetID((ushort)maxRoomIndex);
        r.onRoomClientAdded += onRoomClientAdded;
        rooms.Add(r);
    }

    public void AddClient(T client)
    {
        int idx = GetAvaliableRoomIndex();
        if (idx == -1)
        {
            CreateNewRoom();
            AddClient(client); // retry adding player
        }
        else
        {
            rooms[idx].AddClient(client);
        }
    }

    public void RemoveClient(T p)
    {
        R? r = rooms.Where(room => room.clientsList.Contains(p)).FirstOrDefault();
        r?.RemoveClient(p);
        GC();
    }

    // removes empty rooms
    public void GC()
    {
        for (int i = rooms.Count - 1;  i>-1; i--)
        {
            if (rooms[i].IsEmpty())
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

public class NtServer : Server
{
    /// <summary>
    /// Create a new server
    /// </summary>
    public NtServer() : base("NtServer")
    {
        Console.WriteLine("NtServer::NtServer()");
    }

    /// <summary>
    /// Triggered when a client is attempting a connection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnConnect(object? sender, ServerConnectedEventArgs e)
    {
        Console.WriteLine($"NtServer::OnConnect({e.Client.ToString()})");
    }

    /// <summary>
    /// Triggered when a client is disconnecting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnDisconnect(object? sender,ServerDisconnectedEventArgs e)
    {
        Console.WriteLine($"NtServer::OnDisconnect({e.Client.ToString()})");
    }


    /// <summary>
    /// Triggered when the server recieves a message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMsgRecieved(object? sender, MessageReceivedEventArgs e)
    {
        Console.WriteLine($"NtServer::OnMsgRecieved({e.MessageId})");
    }

}

public class AppV2
{
    /// <summary>
    /// The server instance
    /// </summary>
    static NtServer ntsrv = new NtServer();

    public static void CreateNtServer()
    {
        RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);
        ntsrv.ClientConnected += ntsrv.OnConnect;
        ntsrv.ClientDisconnected += ntsrv.OnDisconnect;
        ntsrv.MessageReceived += ntsrv.OnMsgRecieved;
        ntsrv.Start(7777,10);
    }

    public static void CreateUi(ref NtServer srv)
    {
        InitWindow(800, 600, "Server");
        SetTargetFPS(60);
        rlImGui.Setup();
        LoadConfig();
        LoadStyle();
    }

    public static void LoadConfig()
    {
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        ImGui.GetStyle().WindowRounding = 5;
        ImGui.GetStyle().ChildRounding = 5;
        ImGui.GetStyle().FrameRounding = 5;
        ImGui.GetStyle().PopupRounding = 5;
        ImGui.GetStyle().ScrollbarRounding = 5;
        ImGui.GetStyle().GrabRounding = 5;
        ImGui.GetStyle().TabRounding = 5;

        ImGui.GetStyle().TabBorderSize = 1;

        ImGui.GetStyle().WindowTitleAlign = new Vector2(0.5f);
        ImGui.GetStyle().SeparatorTextAlign = new Vector2(0.5f);
        ImGui.GetStyle().SeparatorTextPadding = new Vector2(20, 5);

    }

    public static void LoadStyle()
    {
        var colors = ImGui.GetStyle().Colors;
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.105f, 0.11f, 1.0f);

        // Headers
        colors[(int)ImGuiCol.Header] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);

        // Buttons
        colors[(int)ImGuiCol.Button] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);

        // Frame BG
        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.3f, 0.305f, 0.31f, 1.0f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);

        // Tabs
        colors[(int)ImGuiCol.Tab] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.38f, 0.3805f, 0.381f, 1.0f);
        colors[(int)ImGuiCol.TabActive] = new Vector4(0.28f, 0.2805f, 0.281f, 1.0f);
        colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.2f, 0.205f, 0.21f, 1.0f);

        // Title
        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.15f, 0.1505f, 0.151f, 1.0f);
    }

    /// <summary>
    /// Entry point for NetServer
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        CreateNtServer();
        CreateUi(ref ntsrv);

        while (!WindowShouldClose())
        {
            ntsrv.Update();
            BeginDrawing();
            ClearBackground(Color.WHITE);
            DrawText("Server", 275, 20, 20, Color.RED);
            rlImGui.Begin();
            if (ImGui.Begin("Server"))
            {
                if (ImGui.BeginTabBar("#tab"))
                {
                    if (ImGui.BeginTabItem("Rooms"))
                    {
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();

                }
                ImGui.End();
            }
            rlImGui.End();
            EndDrawing();
        }
        rlImGui.Shutdown();
        CloseWindow();
    }
}