using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBShapeSharp.Type;

namespace KBShapeSharp.Shape
{

    public class KBMultiPoints : KBShapeBase
    {
        public List<KBPoint> m_Points;

        public KBMultiPoints(int cnt)
        {
            m_ShpType = SHPType.MultiPointZ;
            Init( cnt );

        }

        public KBMultiPoints()
        {
            m_Points = new List<KBPoint>();
        }

        public void Init(int cnt)
        {
            m_Points = new List<KBPoint>();

            for (int idx = 0; idx < cnt; ++idx)
            {
                m_Points.Add(new KBPoint());
            }
        }

    }
}
