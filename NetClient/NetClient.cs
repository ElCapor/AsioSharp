using Riptide;
using GameData;

public class NetClient : Client
{

    public ushort roomID;
    public void ConnectToGameServer()
    {
        this.Connect("127.0.0.1:7777");
        while (this.IsConnecting)
        {
            this.Update();
        }
    }

    public void DisconnectFromGameServer() {  this.Disconnect(); }

    

}
