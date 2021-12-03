using DarkRift;

namespace FinalCommon.Data
{
    public class PaddlePositionData : IDarkRiftSerializable
    {
        public Vector3 Position;

        public void Deserialize(DeserializeEvent e)
        {
            Position = e.Reader.ReadSerializable<Vector3>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Position);
        }
    }
}
