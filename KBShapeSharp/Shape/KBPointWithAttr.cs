using KBShapeSharp.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Shape
{
    public class KBPointWithAttr : KBShapeBase
    {
        KBPoint pt = new KBPoint();

        public KBPointWithAttr()
        {
            pt.x = 0.0;
            pt.y = 0.0;
            pt.z = 0.0;
        }

        public KBPointWithAttr( double x, double y, double z )
        {
            pt.x = x;
            pt.y = y;
            pt.z = z;
        }

        public KBPointWithAttr( double x, double y )
        {
            pt.x = x;
            pt.y = y;
            pt.z = 0.0;
        }

        public KBPointWithAttr( DBFAttribute[] attr )
        {
            pt.x = 0.0;
            pt.y = 0.0;
            pt.z = 0.0;
            m_Attribute = attr;
        }
    }
}