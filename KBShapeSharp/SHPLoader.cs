
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBShapeSharp.Shape;
using KBShapeSharp.Struct;
using KBShapeSharp.Type;

namespace KBShapeSharp
{


    public class SHPLoader
    {
        public static int SHPHeaderLength   = 100;
        public static int SHXRecordSize     = 8;

        public static int DBFHeaderLength   = 32;
        public static int DBFFieldLength   = 32;

        //SHX Info;
        SHXInfo m_SHXInfo = null;
        SHPHeaderInfo m_SHPHeader = null;
        DBFHeaderInfo m_DBFHeader = null;

        public bool m_ValidDBF;
        public bool m_ValidSHP;
        public bool m_ValidSHX;
        public List<KBShapeBase> m_Shape = null;

        public SHPLoader()
        {
            m_ValidDBF = false;
            m_ValidSHP = false;
        }

        public bool Load( string strPath )
        {
            string strSHX, strSHP, strDBF;

            if ( !File.Exists( strPath ) )
            {
                return false;
            }

            if ( Path.HasExtension( strPath ) )
            {
                string strExt = Path.GetExtension( strPath );

                strSHX = strPath.Replace( strExt, ".SHX" );
                strSHP = strPath.Replace( strExt, ".SHP" );
                strDBF = strPath.Replace( strExt, ".DBF" );
            }
            else
            {
                strSHX = strPath + ".SHX";
                strSHP = strPath + ".SHP";
                strDBF = strPath + ".DBF";
            }

            if ( File.Exists( strSHX ) )
            {
                // Read SHX
                using ( FileStream fs = new FileStream( strSHX, FileMode.Open, FileAccess.Read ) )
                {
                    using ( BinaryReader br = new BinaryReader( fs ) )
                    {
                        m_SHXInfo = new SHXInfo();

                        bool bRet = true;

                        bRet &= ReadSHPHeader( br, ref m_SHXInfo.m_ShxHeaderInfo );
                        bRet &= ReadSHXBody( br, ref m_SHXInfo );

                        if ( !bRet )
                        {
                            m_SHXInfo = null;
                        }

                        Debug.WriteLine( m_SHXInfo.ToString() );
                    }
                }
            }


            if ( File.Exists( strSHP ) )
            {
                // Read SHP
                using ( FileStream fs = new FileStream( strSHP, FileMode.Open, FileAccess.Read ) )
                {
                    using ( BinaryReader br = new BinaryReader( fs ) )
                    {
                        m_SHPHeader = new SHPHeaderInfo();
                        ReadSHPHeader( br, ref m_SHPHeader );

                        if ( m_SHXInfo is null )
                        {
                            ReadSHPBodyWithoutSHX( br );
                        }
                        else
                        {
                            ReadSHPBodyWithSHX( br );
                        }

                        Debug.WriteLine( m_SHXInfo.ToString() );
                    }
                }
            }

            if ( !m_SHPHeader.Equals( m_SHXInfo.m_ShxHeaderInfo ) )
            {
                Debug.WriteLine( "Invalid SHP, SHX" );
                return false;
            }

            if ( File.Exists( strDBF ) )
            {
                // Read DBF
                using ( FileStream fs = new FileStream( strDBF, FileMode.Open, FileAccess.Read ) )
                {
                    using ( BinaryReader br = new BinaryReader( fs ) )
                    {

                        if ( true == ReadDBFHeader( br ) )
                        {
                            ReadDBFBody( br );
                        }

                        Debug.WriteLine( m_SHXInfo.ToString() );
                    }
                }
            }


            return true;
        }

        #region DBF

        public bool ReadDBFHeader( BinaryReader br )
        {
            try
            {
                br.BaseStream.Seek( 0, SeekOrigin.Begin );
                byte[] byteBuffer = br.ReadBytes( DBFHeaderLength );

                m_DBFHeader = new DBFHeaderInfo();

                // ???
                m_DBFHeader.test1 = new byte[ 1 ];
                m_DBFHeader.test1[ 0 ] = byteBuffer[ 0 ];

                // Date of last update
                m_DBFHeader.test2 = new byte[ 3 ];
                Array.Copy( byteBuffer, 1, m_DBFHeader.test2, 0, 3 );

                // Number of records in the database file
                m_DBFHeader.nRecords = BitConverter.ToInt32( byteBuffer, 4 );

                // Number of bytes in the header
                m_DBFHeader.nHeaderLength = BitConverter.ToInt16( byteBuffer, 8 );

                // Number of bytes in the record
                m_DBFHeader.nRecordLength = BitConverter.ToInt16( byteBuffer, 10 );

                // Flag indicating incomplete transaction
                m_DBFHeader.bIncompleteTransaction = ( 0 != byteBuffer[ 14 ] );
                // Encryption flag
                m_DBFHeader.bEncryption = ( 0 != byteBuffer[ 15 ] );

                // Production .mdx file flag; 1 if there is a production .mdx file, 0 if not
                m_DBFHeader.bMDFFileExist = ( 0 != byteBuffer[ 28 ] );
                // Language driver ID
                m_DBFHeader.languageDriverID = byteBuffer[ 29 ];

                m_DBFHeader.nFields = ( m_DBFHeader.nHeaderLength - DBFHeaderLength ) / 32;
                m_DBFHeader.m_FieldInfo = new DBFFieldInfo[ m_DBFHeader.nFields ];
                ASCIIEncoding enc = new ASCIIEncoding();

                byteBuffer = br.ReadBytes( m_DBFHeader.nHeaderLength - DBFHeaderLength );

                for ( int idx = 0; idx < m_DBFHeader.nFields; ++idx )
                {

                    DBFFieldInfo dfi = new DBFFieldInfo();

                    int iOffset = idx * DBFFieldLength;
                    // Get Field Name
                    dfi.m_Name = enc.GetString( byteBuffer, iOffset, 10 );

                    // Get Field Type
                    byte fieldType = byteBuffer[ iOffset + 11 ];
                    string strFieldType = enc.GetString( byteBuffer, iOffset + 11, 1 );

                    // ASCII
                    switch ( strFieldType )
                    {
                    // Date
                    case "D":
                        dfi.m_FieldType = DBFFieldType.FTDate;
                        break;
                    // Float
                    case "F":
                        dfi.m_FieldType = DBFFieldType.FTDouble;
                        break;

                    // Logical
                    case "L":
                        dfi.m_FieldType = DBFFieldType.FTLogical;
                        break;

                    // Character, Memo
                    case "C":
                    case "M":
                        dfi.m_FieldType = DBFFieldType.FTString;
                        break;

                    // Numeric
                    case "N":
                        dfi.m_FieldType = DBFFieldType.FTInteger;
                        break;

                    default:
                        dfi.m_FieldType = DBFFieldType.FTInvalid;
                        break;

                    }

                    dfi.m_NWidth = byteBuffer[ iOffset + 16 ];
                    dfi.m_NDecimal = byteBuffer[ iOffset + 17 ];


#if DEBUG
                    Debug.WriteLine( "Field Index : {0}", idx );
                    Debug.WriteLine( "m_Name : {1}", idx, dfi.m_Name );
                    Debug.WriteLine( "m_NWidth : {1}", idx, dfi.m_NWidth );
                    Debug.WriteLine( "m_NDecimal : {1}", idx, dfi.m_NDecimal );
                    Debug.WriteLine( "m_FieldType : {1}", idx, dfi.m_FieldType );
#endif
                    // ??? 써본적이없다..
                    int nWorkAreadID = BitConverter.ToInt16( byteBuffer, iOffset + 18 );
                    byte example = byteBuffer[ iOffset + 20];
                    byte mdxFlag = byteBuffer[ iOffset + 31];

                    m_DBFHeader.m_FieldInfo[ idx ] = dfi;

                }


            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex.ToString() );
                return false;
            }

            return true;
        }

        private bool ReadDBFBody( BinaryReader br )
        {
            try
            {
                ASCIIEncoding enc = null;
                br.BaseStream.Seek( m_DBFHeader.nHeaderLength + 1, SeekOrigin.Begin );

                for ( int iRecord = 0; iRecord < m_DBFHeader.nRecords; ++iRecord )
                {
                    byte[] byteBuffer = br.ReadBytes( m_DBFHeader.nRecordLength );

                    int iOffset = 0;

                    for ( int iField = 0; iField < m_DBFHeader.nFields; ++iField )
                    {
                        int nWidth = m_DBFHeader.m_FieldInfo[ iField ].m_NWidth;

                        DBFAttribute da = new DBFAttribute( ref m_DBFHeader.m_FieldInfo[ iField ], byteBuffer.Skip( iOffset ).Take( nWidth ).ToArray() );

                        iOffset += nWidth;

#if DEBUG
                        Debug.WriteLine( "iRecord : {0}, iField : {1}, Data : {2}", iRecord, iField, da.DBFReadAttribute() );
#endif
                    }

                }

            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex.ToString() );
                return false;
            }

            return true;
        }

        #endregion DBF

        #region SHP, SHX

        public bool ReadSHPHeader( BinaryReader br, ref SHPHeaderInfo shpHeader )
        {
            try
            {
                br.BaseStream.Seek( 0, SeekOrigin.Begin );
                shpHeader = new SHPHeaderInfo();

                byte[] SHPHeader = br.ReadBytes( SHPHeaderLength );
                byte[] byteBuffer;

                byteBuffer = Constants.SwapByte( SHPHeader, 0, 4 );
                shpHeader.fileCode = BitConverter.ToInt32( byteBuffer, 0 );

                byteBuffer = Constants.SwapByte( SHPHeader, 24, 4 );
                shpHeader.shxLength = BitConverter.ToInt32( byteBuffer, 0 );

                shpHeader.version = BitConverter.ToInt32( SHPHeader, 28 );
                shpHeader.shpType = BitConverter.ToInt32( SHPHeader, 32 );

                shpHeader.mbr = new double[ 4 ];
                shpHeader.mbr[ 0 ] = BitConverter.ToDouble( SHPHeader, 36 );
                shpHeader.mbr[ 1 ] = BitConverter.ToDouble( SHPHeader, 44 );
                shpHeader.mbr[ 2 ] = BitConverter.ToDouble( SHPHeader, 52 );
                shpHeader.mbr[ 3 ] = BitConverter.ToDouble( SHPHeader, 60 );

                shpHeader.zRange = new double[ 2 ];
                shpHeader.zRange[ 0 ] = BitConverter.ToDouble( SHPHeader, 68 );
                shpHeader.zRange[ 1 ] = BitConverter.ToDouble( SHPHeader, 76 );

                shpHeader.mRange = new double[ 2 ];
                shpHeader.mRange[ 0 ] = BitConverter.ToDouble( SHPHeader, 84 );
                shpHeader.mRange[ 1 ] = BitConverter.ToDouble( SHPHeader, 92 );
            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex.ToString() );
                return false;
            }

            return true;
        }

        public bool ReadSHXBody( BinaryReader br, ref SHXInfo shxInfo )
        {
            br.BaseStream.Seek( SHPHeaderLength, SeekOrigin.Begin );
            shxInfo.nRecords = ( ( int )br.BaseStream.Length - SHPHeaderLength ) / SHXRecordSize;
            byte[] byteArr;

            shxInfo.m_SHXDataList = new List<SHXData>();


            for ( int idx = 0; idx < shxInfo.nRecords; ++idx )
            {
                SHXData shxData = new SHXData();

                byteArr = br.ReadBytes( SHXRecordSize );
                shxData.iOffset = BitConverter.ToInt32( Constants.SwapByte( byteArr, 0, 4 ), 0 );
                shxData.iLength = BitConverter.ToInt32( Constants.SwapByte( byteArr, 4, 4 ), 0 );

                shxData.iOffset *= 2;
                shxData.iLength *= 2;

                shxInfo.m_SHXDataList.Add( shxData );
            }

            return true;
        }
        public bool ReadSHPBodyWithSHX( BinaryReader br )
        {
            if ( m_SHXInfo is null )
            {
                return false;
            }

            for ( int iRecord = 0; iRecord < m_SHXInfo.nRecords; ++iRecord )
            {
                Debug.WriteLine( " iRecord : {0}", iRecord );
                SHXData shxData = m_SHXInfo.m_SHXDataList[iRecord];

                br.BaseStream.Seek( shxData.iOffset, SeekOrigin.Begin );

                byte[] byteArr = br.ReadBytes( shxData.iLength + 8 );

                int recNo = BitConverter.ToInt32( Constants.SwapByte( byteArr, 0, 4 ), 0 );
                int contentLength = BitConverter.ToInt32( Constants.SwapByte( byteArr, 4, 4 ), 0 );

                int shpType = BitConverter.ToInt32( byteArr, 8 );

                bool bZValue = ( 1 == shpType / 10 );
                int ptsize = ( bZValue ? ( 3 ) : ( 2 ) ) * sizeof( double );

                switch ( ( SHPType )shpType )
                {
                case SHPType.Point:
                case SHPType.PointZ:
                    {

                    }
                    break;

                case SHPType.PolyLine:
                case SHPType.PolyLineZ:
                case SHPType.Polygon:
                case SHPType.PolygonZ:
                    {
                        double[] box = new double[4];
                        int numParts = BitConverter.ToInt32( byteArr, 44 );
                        int numPoints = BitConverter.ToInt32( byteArr, 48 );
                        int[] parts = new int[numParts];
                        KBPoint[] pts = new KBPoint[numPoints];

                        box[ 0 ] = BitConverter.ToDouble( byteArr, 12 );
                        box[ 1 ] = BitConverter.ToDouble( byteArr, 20 );
                        box[ 2 ] = BitConverter.ToDouble( byteArr, 28 );
                        box[ 3 ] = BitConverter.ToDouble( byteArr, 36 );


                        Debug.WriteLine( " numParts : {0}", numParts );
                        Debug.WriteLine( " numPoints : {0}", numPoints );

                        // Read Parts
                        int beginOffset = 52;

                        for ( int partIdx = 0; partIdx < numParts; ++partIdx )
                        {
                            int iOffset = beginOffset + partIdx * sizeof( int );

                            parts[ partIdx ] = BitConverter.ToInt32( byteArr, iOffset );
                        }


                        // Read Points
                        beginOffset += sizeof( int ) * numParts;


                        for ( int pointIdx = 0; pointIdx < numPoints; ++pointIdx )
                        {
                            int iOffset = beginOffset + ( pointIdx * ptsize );

                            KBPoint pt = new KBPoint();

                            pt.x = BitConverter.ToDouble( byteArr, iOffset );

                            iOffset += sizeof( double );
                            pt.y = BitConverter.ToDouble( byteArr, iOffset );

                            if ( bZValue )
                            {
                                iOffset += sizeof( double );
                                pt.z = BitConverter.ToDouble( byteArr, iOffset );
                            }

                            pts[ pointIdx ] = pt;
                        }

                    }
                    break;

                case SHPType.NullShape:
                default:
                    return false;
                }
            }


            return true;
        }

        public bool ReadSHPBodyWithoutSHX( BinaryReader br )
        {
            br.BaseStream.Seek( SHPHeaderLength, SeekOrigin.Begin );

            byte[] byteArr;

            while ( br.BaseStream.Position < br.BaseStream.Length )
            {
                SHXData shxData = new SHXData();
                byteArr = br.ReadBytes( SHXRecordSize );
                shxData.iOffset = BitConverter.ToUInt16( Constants.SwapByte( byteArr, 0, 4 ), 0 );
                shxData.iLength = BitConverter.ToInt32( Constants.SwapByte( byteArr, 4, 4 ), 0 );

                shxData.iOffset *= 2;
                shxData.iLength *= 2;
            }


            return true;
        }

        #endregion SHP, SHX
    }
}
