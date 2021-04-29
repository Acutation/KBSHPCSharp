using KBShapeSharp.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Shape
{
    public class KBPoint
    {
        public double x;
        public double y;
        public double z;

        public KBPoint()
        {
            x = 0.0;
            y = 0.0;
            z = 0.0;
        }

        public KBPoint(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public KBPoint( double x, double y )
        {
            this.x = x;
            this.y = y;
            z = 0.0;
        }
    }

    
}
