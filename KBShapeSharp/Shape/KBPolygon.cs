
using KBShapeSharp.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Shape
{
    public class KBPolygon : KBMultiPoints
    {
        public KBPolygon()
        {
            m_ShpType = SHPType.PolygonZ;
            m_Points = new List<KBPoint>();
        }

        public KBPolygon( int cnt )
        {
            Init( cnt );
        }
    }
}
