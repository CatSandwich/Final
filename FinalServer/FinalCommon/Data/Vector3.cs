using DarkRift;

namespace FinalCommon.Data
{
    public class Vector3 : IDarkRiftSerializable
    {
        public float X;
        public float Y;
        public float Z;

        public void Deserialize(DeserializeEvent e)
        {
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();  
            Z = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y); 
            e.Writer.Write(Z);
        }

        public static implicit operator Vector3(Vector2 v2) => new Vector3 { X = v2.X, Y = v2.Y, Z = 0 };
    }
}
