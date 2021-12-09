using DarkRift;

namespace FinalCommon.Data
{
    // Stores an object, Id, and a position, Position, to move it to
    public class MoveObjectData : IDarkRiftSerializable, IServerToClient
    {
        public ServerToClient ServerToClientTag => ServerToClient.MoveObject;

        public ObjectIds Id;
        public Vector3 Position;

        public MoveObjectData() { }

        public MoveObjectData(ObjectIds id, Vector3 position)
        {
            Id = id;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = (ObjectIds)e.Reader.ReadUInt16();
            Position = e.Reader.ReadSerializable<Vector3>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Id);
            e.Writer.Write(Position);
        }
    }
}
