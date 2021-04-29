using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Struct
{
    public class DBFHeaderInfo
    {
        public byte[] test1;
        public byte[] test2;
        public int nRecords;
        public int nFields;
        public short nHeaderLength;
        public short nRecordLength;
        public bool bIncompleteTransaction;
        public bool bEncryption;
        public bool bMDFFileExist;
        public byte languageDriverID;

        public DBFFieldInfo[] m_FieldInfo;

    }
}
