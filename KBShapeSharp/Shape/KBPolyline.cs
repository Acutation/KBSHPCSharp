using KBShapeSharp.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Shape
{
    public class KBPolyline : KBMultiPoints
    {
        public KBPolyline()
        {
            m_ShpType = SHPType.PolyLineZ;
            m_Points = new List<KBPoint>();
        }

        public KBPolyline( int cnt )
        {
            Init( cnt );
        }
    }
}
