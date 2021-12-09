using DarkRift;

namespace FinalCommon.Data
{
    // Stores a position, Position, for the server to move the paddle to
    public class PaddlePositionData : IDarkRiftSerializable, IClientToServer
    {
        public ClientToServer ClientToServerTag => ClientToServer.PaddlePosition;

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
