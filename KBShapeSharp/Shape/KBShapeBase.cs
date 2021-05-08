using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBShapeSharp.Struct;
using KBShapeSharp.Type;

namespace KBShapeSharp.Shape
{
    public class KBShapeBase
    {
        public SHPType m_ShpType;
        /////<summary>Shape number (-1 is unknown/unassigned)</summary>	
        public int m_ShapeId;
        /////<summary>Number of parts (0 implies single part with no info)</summary>	
        public int m_NumParts;
        public DBFAttribute [] m_Attribute;
        public List<KBPoint> m_Points;
    }
}
