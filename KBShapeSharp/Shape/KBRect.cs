using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Shape
{
    public class KBRect
    {
        public KBPoint point_LT;
        public KBPoint point_RB;

        public KBRect()
        {
            point_LT = new KBPoint();
            point_RB = new KBPoint();
        }

        public KBPoint GetCenteroid()
        {
            return new KBPoint( ( point_LT.x + point_RB.x ) / 2, ( point_LT.y + point_RB.y ) / 2, ( point_LT.z + point_RB.z ) / 2 );
        }
    }
}
