using DarkRift;

namespace FinalCommon.Data
{
    // Stores an object, Id, for the client to assume ownership of
    public class SetOwnershipData : IDarkRiftSerializable
    {
        public ObjectIds Id;

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
