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

        Message ret = Message.Create(MessageSendMode.Reliable, ClientMessage.ReadyToStart);
        _client.Send(ret);
    }

    [MessageHandler((ushort)ServerMessage.PlayerIsReady)]
    public static void HandleOtherPlayerReady(Message msg)
    {
        ushort pid = msg.GetUShort();
        otherPlayer.isReady = true;
    }

    [MessageHandler((ushort)ServerMessage.PlayerMoveDown)]
    public static void HandlerPlayerMoveDown(Message msg)
    {
        Console.WriteLine("Other player started moving");

        ushort pid = msg.GetUShort();
        otherPlayer.StartMoveDown();
    }

    [MessageHandler((ushort)ServerMessage.PlayerStopMove)]
    public static void HandlerPlayerMoveStop(Message msg)
    {
        Console.WriteLine("Other player stopped moving");
        ushort pid = msg.GetUShort() ;
        otherPlayer.StopMove();
    }

    [MessageHandler((ushort)ServerMessage.PlayerMoveUp)]
    public static void HandlerPlayerMoveUp(Message msg)
    {
        Console.WriteLine("Other player move up");
        ushort pid = msg.GetUShort();
        otherPlayer.StartMoveUp();
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

        LocalPlayer.playerStartMoveDownEvent += (object sender, EventArgs args) =>
        {
            Console.WriteLine("Moved down");
            Message mv = Message.Create(MessageSendMode.Reliable, ClientMessage.StartMoveDown);
            _client.Send(mv);
        };

        LocalPlayer.playerStartMoveUpEvent += (object sender, EventArgs args) =>
        {
            Console.WriteLine("Moved up");
            Message mv = Message.Create(MessageSendMode.Reliable, ClientMessage.StartMoveUp);
            _client.Send(mv);
        };

        LocalPlayer.playerStopMoveEvent += (object sender, EventArgs args) =>
        {
            Console.WriteLine("Move Stop");
            Message mv = Message.Create(MessageSendMode.Reliable, ClientMessage.StopMove);
            _client.Send(mv);
        };

        otherPlayer.Position.X = 790;
        while (!WindowShouldClose())
        {
            _client.Update();
            if (LocalPlayer.isReady && otherPlayer.isReady)
            {
                if (IsKeyPressed(KeyboardKey.KEY_DOWN))
                {
                    Console.WriteLine("Down");
                    LocalPlayer.StartMoveDown();
                }
                else if (IsKeyPressed(KeyboardKey.KEY_UP))
                {
                    Console.WriteLine("Up");
                    LocalPlayer.StartMoveUp();
                }
                else if (IsKeyReleased(KeyboardKey.KEY_DOWN))
                {
                    LocalPlayer.StopMove();
                }
                else if (IsKeyReleased(KeyboardKey.KEY_UP))
                {
                    LocalPlayer.StopMove();
                }
                LocalPlayer.Update();
                otherPlayer.Update();
                BeginDrawing();
                ClearBackground(Color.BLACK);
                NetOverlay.DrawNetDebug(_client);
                NetOverlay.DrawNetPlayer(LocalPlayer);
                LocalPlayer.Draw();
                otherPlayer.Draw();
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