namespace RoomSystemTest
{
    public class Student
    {
        public int Id { get; set;}

    };

    public delegate RoomV2<Student> RoomFactoryForStudent(int maxRoomSize);

    public static class RoomFactory
    {
        public static RoomV2<Student> CreateStudentRoom(int maxRoomSize)
        {
            return new RoomV2<Student>(maxRoomSize);
        }
    }


    [TestClass, TestCategory("RoomsV2")]
    public class RoomsV2Test
    {
        [TestMethod]
        public void RoomsTest()
        {
            RoomManagerV2<RoomV2<Student>, Student> rm = new(
                2,
                RoomFactory.CreateStudentRoom
            );
            Student st = new();
            st.Id = 2;
            rm.AddClient(st);

            Assert.IsTrue(rm.rooms.Count == 1 && rm.rooms[0].clientsList.Count == 1);
        }
    }
}