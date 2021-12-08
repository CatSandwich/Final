using DarkRift;

namespace FinalCommon.Data
{
    // Stores an object, Id, and a size, Size, to resize it to
    public class ResizeObjectData : IDarkRiftSerializable
    {
        public ObjectIds Id;
        public Vector2 Size;

        public ResizeObjectData() { }

        public ResizeObjectData(ObjectIds id, Vector2 size)
        {
            Id = id;
            Size = size;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = (ObjectIds) e.Reader.ReadUInt16();
            Size = e.Reader.ReadSerializable<Vector2>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Id);
            e.Writer.Write(Size);
        }
    }
}
