using Riptide.Utils;
using Raylib_cs;
using static Raylib_cs.Raylib;
using GameData;
using Riptide;
using Riptide.Utils;
using Riptide.Transports;
public class App
{
    static NetClient _client = new NetClient();

    [MessageHandler(((ushort)ServerMessage.JoinedRoom))]
    public static void HandleJoinRoomEvent(Message msg)
    {
        ushort roomID = msg.GetUShort();
        Console.WriteLine($"We are now in room : {roomID}");
        _client.roomID = roomID;

        Message msg2 = Message.Create(MessageSendMode.Reliable, ClientMessage.ReadyToStart);
        _client.Send(msg2);
    }


    [MessageHandler(((ushort)ServerMessage.SendRoomFull))]
    public static void HandleFullRoom(Message msg)
    {
        ushort otherPlayerID = msg.GetUShort();
        Console.WriteLine($"Room is full {otherPlayerID}");
    }
   

    public static void OnConnect(object? sender, EventArgs eventArgs)
    {
        Console.WriteLine($"Connection Succeded , launching game...");
        Run();
    }

    public static void OnConnectionFailed(object? sender, Riptide.ConnectionFailedEventArgs args)
    {
        Console.WriteLine($"Connection failed for reason {args.Message}");
        Environment.Exit(-1);
    }

    public static void OnDisconnect(object? sender, Riptide.DisconnectedEventArgs args)
    {
        Console.WriteLine($"Disconnected , reason : {args.Message}");
        Environment.Exit(-1);
    }

    public static void OnJoin(object? sender, Riptide.ClientConnectedEventArgs args)
    {
        Console.WriteLine($"Another player joined , {args.Id}");
        otherPlayer.isReady = true;
    }
    static Player LocalPlayer = new();
    static Player otherPlayer = new();

    public static void Run()
    {
        InitWindow(800, 600, "demo");
        SetTargetFPS(60);
        LocalPlayer.isReady = true;
        while (!WindowShouldClose())
        {
            _client.Update();
            if (LocalPlayer.isReady && otherPlayer.isReady)
            {
                if (GetKeyPressed() == (int)KeyboardKey.KEY_DOWN)
                {
                    Console.WriteLine("Down");
                    LocalPlayer.StartMoveDown();
                }
                else if (IsKeyReleased(KeyboardKey.KEY_DOWN))
                {
                    LocalPlayer.StopMove();
                }
                LocalPlayer.Update();
                BeginDrawing();
                ClearBackground(Color.BLACK);
                NetOverlay.DrawNetDebug(_client);
                NetOverlay.DrawNetPlayer(LocalPlayer);
                LocalPlayer.Draw();
                EndDrawing();
            }
            else
            {
                BeginDrawing();
                ClearBackground(Color.BLACK);
                NetOverlay.DrawNetDebug(_client);
                DrawText("Waiting for another player to come ...", 250, 370, 20, Color.WHITE);
                EndDrawing();
            }
           
        }
        CloseWindow();
    }
    public static void Main(string[] args)
    {
        RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);
        _client.Connected += OnConnect;
        _client.Disconnected += OnDisconnect;
        _client.ConnectionFailed+= OnConnectionFailed;
        _client.ClientConnected += OnJoin;
        _client.ConnectToGameServer();

        
        
    }
}