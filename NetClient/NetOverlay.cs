using Raylib_cs;
using static Raylib_cs.Raylib;
using GameData;

public static class NetOverlay
{
    public static void DrawNetDebug(NetClient client)
    {
        DrawRectangle(0, 0, 150, 100, Color.GRAY);
        DrawText($"RTT : {client.SmoothRTT}", 0, 20, 20, Color.BLACK);
        DrawText($"Id : {client.Id}", 0, 40, 20, Color.BLACK);
        DrawText($"{(client.IsConnected ? "Connected" : "Disconnected")}", 0, 60, 20, client.IsConnected ? Color.GREEN : Color.RED);
    }

    public static void DrawNetPlayer(Player player)
    {
        DrawRectangle(250, 0, 250, 140, Color.GRAY);
        DrawText($"Position : {player.Position}", 250, 20, 20, Color.BLUE);
        DrawText($"Velocity : {player.Velocity}", 250, 40, 20, Color.BLUE);
        DrawText($"Previous Vel : {player.previousVelocity}", 250, 60, 20, Color.BLUE);
        DrawText($"Previous Pos : {player.previousPostition}", 250, 80, 20, Color.BLUE);
        DrawText($"IsMoving : {player.IsMoving()}", 250, 100 ,20, Color.BLUE);
        DrawText($"HasMoved : {player.HasMoved()}", 250, 120, 20, Color.BLUE);
    }
}
