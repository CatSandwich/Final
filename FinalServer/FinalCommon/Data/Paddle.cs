using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;

namespace FinalCommon.Data
{
    public enum PaddleSide : ushort
    {
        Left,
        Right
    }

    public class Paddle : Box
    {
        public PaddleSide Side;

        public override void Deserialize(DeserializeEvent e)
        {
            base.Deserialize(e);
            Side = (PaddleSide)e.Reader.ReadUInt16();
        }

        public override void Serialize(SerializeEvent e)
        {
            base.Serialize(e);
            e.Writer.Write((ushort)Side);
        }
    }
}
