using DarkRift;

namespace FinalServer.Data
{
    public class TestData : IDarkRiftSerializable
    {
        public string FirstName;
        public string LastName;
        public int Age = 10;

        public TestData()
        {

        }

        public void Deserialize(DeserializeEvent e)
        {
            FirstName = e.Reader.ReadString();
            LastName = e.Reader.ReadString();
            Age = e.Reader.ReadInt32();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(FirstName);
            e.Writer.Write(LastName);
            e.Writer.Write(Age);
        }
    }
}
