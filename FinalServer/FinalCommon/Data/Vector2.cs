using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;

namespace FinalCommon.Data
{
    public class Vector2 : IDarkRiftSerializable
    {
        public float X;
        public float Y;

        public void Deserialize(DeserializeEvent e)
        {
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y);
        }
    }
}
