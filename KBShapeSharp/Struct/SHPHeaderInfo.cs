using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Struct
{
    public class SHPHeaderInfo
    {
        public int fileCode;
        public int shxLength;
        public int version;
        public int shpType;

        public double[] mbr;
        public double[] zRange;
        public double[] mRange;

        public override bool Equals( object obj )
        {
            return obj is SHPHeaderInfo info &&
                     fileCode == info.fileCode &&
                     version == info.version &&
                     shpType == info.shpType;
        }

        public override int GetHashCode()
        {
            int hashCode = -1471589882;
            hashCode = hashCode * -1521134295 + fileCode.GetHashCode();
            hashCode = hashCode * -1521134295 + version.GetHashCode();
            hashCode = hashCode * -1521134295 + shpType.GetHashCode();
            return hashCode;
        }
    }
}
