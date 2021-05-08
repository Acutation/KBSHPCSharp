using System;
using System.Text;
using KBShapeSharp.Type;

namespace KBShapeSharp.Struct
{
    /// <summary>
    /// SHPObject - represents on shape (without attributes) read from the .shp file.
    /// </summary>
    public class DBFAttribute
    {
        DBFFieldType m_FieldType;
        int m_nDeciaml;
        int m_nWidth;
        byte[] m_Data;

        public DBFFieldType FieldType { get => m_FieldType; }
        public int nDecimal { get => m_nDeciaml; }
        public int nWidth { get => m_nWidth; }
        public byte[] Data { get => m_Data; }

        public DBFAttribute()
        {
            m_FieldType = DBFFieldType.FTInvalid;
            m_Data = null;
        }

        public DBFAttribute( ref DBFFieldInfo fieldInfo, byte[] data )
        {
            m_FieldType = fieldInfo.m_FieldType;
            m_nDeciaml = fieldInfo.m_NDecimal;
            m_nWidth = fieldInfo.m_NWidth;
            m_Data = data;
        }

        private double DBFReadDoubleAttribute()
        {
            return BitConverter.ToDouble(m_Data, 0);
        }

        private int DBFReadIntegerAttribute()
        {
            return BitConverter.ToInt32(m_Data, 0);
        }

        private string DBFReadStringAttribute()
        {
            return Encoding.Default.GetString( m_Data );
        }

        private DateTime DBFReadDateAttribute()
        {
            string sDate = DBFReadStringAttribute();

            try
            {
                DateTime d = new DateTime(int.Parse(sDate.Substring(0, 4)), int.Parse(sDate.Substring(4, 2)),
                    int.Parse(sDate.Substring(6, 2)));
                return d;
            }
            catch
            {
                return new DateTime(0);
            }
        }

        public object DBFReadAttribute()
        {
            switch( m_FieldType )
            {
                case DBFFieldType.FTDouble:
                    return DBFReadDoubleAttribute();

                case DBFFieldType.FTInteger:
                    return DBFReadIntegerAttribute();

                case DBFFieldType.FTDate:
                    return DBFReadDateAttribute();

                case DBFFieldType.FTLogical:
                case DBFFieldType.FTString:
                default:
                    return DBFReadStringAttribute();
            }
        }
    }
}
