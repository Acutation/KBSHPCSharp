using System;
using KBShapeSharp.Type;

namespace KBShapeSharp.Struct
{
    /// <summary>
    /// SHPObject - represents on shape (without attributes) read from the .shp file.
    /// </summary>
    public class DBFAttribute
    {
        DBFFieldType m_FieldType;
        byte[] m_Data;
        public DBFFieldType FieldType { get => m_FieldType; }

        public DBFAttribute()
        {
            m_FieldType = DBFFieldType.FTInvalid;
            m_Data = null;
        }

        public DBFAttribute( DBFFieldType fieldType, byte[] data )
        {
            m_FieldType = fieldType;
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
            return BitConverter.ToString(m_Data, 0);
        }

        private DateTime DBFReadDateAttribute()
        {
            string sDate = BitConverter.ToString(m_Data, 0);

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
            return m_Data;
        }
        public object DBFReadAttribute( ref DBFFieldType fieldType )
        {
            fieldType = m_FieldType;

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
