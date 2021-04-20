using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Struct
{
    public struct SHXData
    {
        public int iOffset;
        public int iLength;
    }

    public class SHXInfo 
    {
        public SHPHeaderInfo m_ShxHeaderInfo;
        public List<SHXData> m_SHXDataList;
        public int nRecords;
    }
}
