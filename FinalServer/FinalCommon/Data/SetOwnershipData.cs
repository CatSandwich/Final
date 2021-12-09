using DarkRift;

namespace FinalCommon.Data
{
    // Stores an object, Id, for the client to assume ownership of
    public class SetOwnershipData : IDarkRiftSerializable, IServerToClient
    {
        public ServerToClient ServerToClientTag => ServerToClient.SetOwnership;

        public ObjectIds Id;

        public SetOwnershipData() { }

        public SetOwnershipData(ObjectIds id)
        {
            Id = id;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = (ObjectIds)e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Id);
        }
    }
}
