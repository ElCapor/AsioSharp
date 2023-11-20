using GameData;
using Riptide;
using Riptide.Utils;
namespace NetTest
{
    [TestClass]
    public class ConnectionTest
    {
        [TestMethod]
        public void TestServerClientConnection()
        {
            NetClient nc = new NetClient();
            NetServer ns = new();

            ns.Start(7777, 10);
            nc.Connect("127.0.0.1:7777", 10);

            while (nc.IsConnecting)
            {
                ns.Update();
                nc.Update();
                if (ns.ClientCount == 1 && nc.IsConnected)
                    break;
            }
            Assert.IsTrue(ns.ClientCount == 1 && nc.IsConnected);
            nc.Disconnect();
            ns.Stop();
        }

        [TestMethod]
        public void RoomManagerTest()
        {
            RoomManager rm = new();
            Player p = new();
            p.id = 10;

            rm.AddPlayer(p);
            Assert.IsTrue(rm.GetAvaliableRoomIndex() == 0 && rm.rooms.Count == 1 && rm.rooms[0].playerList.Count == 1);
            rm.RemovePlayer(p);
            Assert.IsTrue(rm.rooms.Count == 1 && rm.rooms[0].playerList.Count == 0);
        }

        [TestMethod]
        public void MultipleRoomsTest()
        {
            RoomManager rm = new();

            for (int i = 0; i < 10; i++)
            {
                Player p = new();
                p.id = (ushort)i;

                rm.AddPlayer(p);
                
            }

            Assert.IsTrue(rm.GetAvaliableRoomIndex() == -1 && rm.rooms.Count == 5);

            for (int i = 0; i < 10; i++)
            {
                Player p = new();
                p.id = (ushort)i;

                rm.RemovePlayer(p);

            }
            // we deleted all rooms
            Assert.IsTrue(rm.GetAvaliableRoomIndex() == -1 && rm.rooms.Count == 0);
        }
    }
}