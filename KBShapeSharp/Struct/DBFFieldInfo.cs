using KBShapeSharp.Type;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBShapeSharp.Struct
{
    public class DBFFieldInfo
    {
        public string m_Name;
        public DBFFieldType m_FieldType;
        public int m_NWidth;
        public int m_NDecimal;

        // 써본적이 없지만 적혀있는 것들
        public int nWorkAreadID;
        public byte example;
        public byte mdxFlag;

        public DBFFieldInfo()
        {
            m_Name = null;
            m_FieldType = DBFFieldType.FTInteger;
            m_NWidth = 0;
            m_NDecimal = 0;
        }

        public DBFFieldInfo( string name, DBFFieldType fieldType, int iWidth, int iDecimal )
        {
            m_Name = name;
            m_FieldType = fieldType;
            m_NWidth = iWidth;
            m_NDecimal = iDecimal;
        }
    }
}
