/*using GameData;
using Riptide;
using Riptide.Utils;
using Raylib_cs;
using static Raylib_cs.Raylib;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;

public static class ServerImguiOverlay
{
    public static void RenderRooms(ref NetServer server)
    {
        ref RoomManager rm = ref server.roomManager;
        if (ImGui.Button("Create Room"))
        {
            server.CreateRoom();
        }
        for (int i = 0; i < rm.rooms.Count; i++)
        {
            
            if (ImGui.TreeNode($"Room {i}"))
            {
                ImGui.SameLine();
                if (ImGui.SmallButton("Clear"))
                {
                    server.ClearRoom(rm.rooms[i].GetID());
                    continue;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("Delete"))
                {
                    server.DeleteRoom(rm.rooms[i].GetID());
                    continue;
                }
                ImGui.Text($"Room ID : {rm.rooms[i].GetID()}");
                

                for (int j = rm.rooms[i].playerList.Count -1; j >=0 ; j--)
                {
                    if (ImGui.TreeNode($"Player {j}"))
                    {
                        ImGui.SameLine();
                        if (ImGui.SmallButton("kick"))
                        {
                            server.KickPlayer(rm.rooms[i].playerList[j].id);
                            continue;
                        }
                        ImGui.Text($"Player id {rm.rooms[i].playerList[j].id}");
                        ImGui.TreePop();

                    }
                }
                ImGui.TreePop();
            }
        }
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
}

public class App
{
    static NetServer _serv = new NetServer();

    [MessageHandler((ushort)ClientMessage.ReadyToStart)]
    public static void HandleReadyToStart(ushort clientID, Message msg)
    {
        _serv.playerMap[clientID].isReady = true;
        Room? r = _serv.roomManager.rooms.Where(room => room.playerList.Exists(p => p.id == _serv.playerMap[clientID].id)).FirstOrDefault();
        Player? p = _serv.GetOtherPlayerInRoom(r, _serv.playerMap[clientID].id);
        if (p != null)
        {
            ushort cID = (ushort)_serv.playerMap.Where(c => c.Value.id == p?.id).FirstOrDefault().Key;
            Connection conn = _serv.Clients.Where(c => c.Id == cID).FirstOrDefault();
            Message ret = Message.Create(MessageSendMode.Reliable, ServerMessage.PlayerIsReady);
            ret.AddUShort(p.id);
            _serv.Send(ret, conn);
        }
        
    }

    [MessageHandler((ushort)ClientMessage.StartMoveDown)]
    public static void HandleClientStartMoveDown(ushort clientID, Message msg)
    {
        Player z = _serv.playerMap[clientID];
        z.StartMoveDown();

        Room? r = _serv.roomManager.rooms.Where(room => room.playerList.Exists(p => p.id == _serv.playerMap[clientID].id)).FirstOrDefault();
        Player? p = _serv.GetOtherPlayerInRoom(r, _serv.playerMap[clientID].id);
        if (p != null)
        {
            ushort cID = (ushort)_serv.playerMap.Where(c => c.Value.id == p?.id).FirstOrDefault().Key;
            Connection conn = _serv.Clients.Where(c => c.Id == cID).FirstOrDefault();
            Message ret = Message.Create(MessageSendMode.Reliable, ServerMessage.PlayerMoveDown);
            ret.AddUShort(p.id);
            _serv.Send(ret, conn);
        }
    }

    [MessageHandler((ushort)ClientMessage.StartMoveUp)]
    public static void HandleClientStartMoveUp(ushort clientID, Message msg)
    {
        Player z = _serv.playerMap[clientID];
        z.StartMoveUp();

        Room? r = _serv.roomManager.rooms.Where(room => room.playerList.Exists(p => p.id == _serv.playerMap[clientID].id)).FirstOrDefault();
        Player? p = _serv.GetOtherPlayerInRoom(r, _serv.playerMap[clientID].id);
        if (p != null)
        {
            ushort cID = (ushort)_serv.playerMap.Where(c => c.Value.id == p?.id).FirstOrDefault().Key;
            Connection conn = _serv.Clients.Where(c => c.Id == cID).FirstOrDefault();
            Message ret = Message.Create(MessageSendMode.Reliable, ServerMessage.PlayerMoveUp);
            ret.AddUShort(p.id);
            _serv.Send(ret, conn);
        }
    }

    [MessageHandler((ushort)ClientMessage.StopMove)]
    public static void HandleClientStopMove(ushort clientID, Message msg)
    {
        Player z = _serv.playerMap[clientID];
        z.StopMove();

        Room? r = _serv.roomManager.rooms.Where(room => room.playerList.Exists(p => p.id == _serv.playerMap[clientID].id)).FirstOrDefault();
        Player? p = _serv.GetOtherPlayerInRoom(r, _serv.playerMap[clientID].id);
        if (p != null)
        {
            ushort cID = (ushort)_serv.playerMap.Where(c => c.Value.id == p?.id).FirstOrDefault().Key;
            Connection conn = _serv.Clients.Where(c => c.Id == cID).FirstOrDefault();
            Message ret = Message.Create(MessageSendMode.Reliable, ServerMessage.PlayerStopMove);
            ret.AddUShort(p.id);
            _serv.Send(ret, conn);
        }
    }

    public static void OldMain(string[] args)
    {
        RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);
        _serv.ClientConnected += _serv.OnConnect;
        _serv.ClientDisconnected += _serv.OnDisconnect;
        _serv.Start(7777, 10);

        InitWindow(800, 600, "Server");
        SetTargetFPS(60);
        rlImGui.Setup();
        ServerImguiOverlay.LoadConfig();
        ServerImguiOverlay.LoadStyle();
        while (!WindowShouldClose())
        {
            _serv.Update();
            BeginDrawing();
            ClearBackground(Color.WHITE);
            DrawText("Server", 275, 20, 20, Color.RED);
            rlImGui.Begin();            // starts the ImGui content mode. Make all ImGui calls after this
            if (ImGui.Begin("Server"))
            {
                if (ImGui.BeginTabBar("#tab"))
                {
                    if (ImGui.BeginTabItem("Rooms"))
                    {
                        ServerImguiOverlay.RenderRooms(ref _serv);
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();

                }
                ImGui.End();
            }
            rlImGui.End();			// ends the ImGui content mode. Make all ImGui calls before this
            EndDrawing();
        }
        rlImGui.Shutdown();
    }

    
}*/