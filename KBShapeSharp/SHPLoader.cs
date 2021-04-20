
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
        public static int DBFHeaderLength   = 100;
        public static int SHXRecordSize     = 8;

        //SHX Info;
        SHXInfo m_SHXInfo = null;
        SHPHeaderInfo m_SHPHeader = null;

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

            if ( File.Exists( strDBF ) )
            {
                // Read SHX
                using ( FileStream fs = new FileStream( strSHX, FileMode.Open, FileAccess.Read ) )
                {
                    using ( BinaryReader br = new BinaryReader( fs ) )
                    {
                        m_SHXInfo = new SHXInfo();

                        bool bRet = true;

                        bRet &= ReadSHPheader( br, ref m_SHXInfo.m_ShxHeaderInfo );
                        bRet &= ReadSHXBody( br, ref m_SHXInfo );

                        if( !bRet )
                        {
                            m_SHXInfo = null;
                        }

                        Console.WriteLine( m_SHXInfo.ToString() );
                    }
                }
            }


            if ( File.Exists( strDBF ) )
            {
                // Read SHP
                using ( FileStream fs = new FileStream( strSHP, FileMode.Open, FileAccess.Read ) )
                {
                    using ( BinaryReader br = new BinaryReader( fs ) )
                    {
                        m_SHPHeader = new SHPHeaderInfo();
                        ReadSHPheader( br, ref m_SHPHeader );

                        if( m_SHXInfo is null )
                        {
                            ReadSHPBodyWithoutSHX( br );
                        }
                        else
                        {
                            ReadSHPBodyWithSHX( br );
                        }

                        Console.WriteLine( m_SHXInfo.ToString() );
                    }
                }
            }

            if ( !m_SHPHeader.Equals( m_SHXInfo.m_ShxHeaderInfo ) )
            {
                Console.WriteLine( "Invalid SHP, SHX" );
                return false;
            }

            if ( File.Exists( strDBF ) )
            {
                // Read DBF
                using ( FileStream fs = new FileStream( strDBF, FileMode.Open, FileAccess.Read ) )
                {
                    using ( BinaryReader br = new BinaryReader( fs ) )
                    {
                        //m_SHXInfo = new SHXInfo();
                        //Constants.ReadSHPheader( br, ref m_SHPHeader );
                        //Constants.ReadSHPBody( br, ref m_SHXInfo );

                        Console.WriteLine( m_SHXInfo.ToString() );
                    }
                }
            }


            return true;
        }


        public bool ReadSHPheader( BinaryReader br, ref SHPHeaderInfo shpHeader )
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
            shxInfo.nRecords = ( (int)br.BaseStream.Length - SHPHeaderLength ) / SHXRecordSize;
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
            if( m_SHXInfo is null )
            {
                return false;
            }

            for( int iRecord = 0; iRecord < m_SHXInfo.nRecords; ++iRecord )
            {
                Console.WriteLine( " iRecord : {0}", iRecord );
                SHXData shxData = m_SHXInfo.m_SHXDataList[iRecord];

                br.BaseStream.Seek( shxData.iOffset, SeekOrigin.Begin );

                byte[] byteArr = br.ReadBytes( shxData.iLength + 8 );

                int recNo = BitConverter.ToInt32( Constants.SwapByte( byteArr, 0, 4 ), 0 );
                int contentLength = BitConverter.ToInt32( Constants.SwapByte( byteArr, 4, 4 ), 0 );

                int shpType = BitConverter.ToInt32( byteArr, 8 );

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

                        box[ 0 ] = BitConverter.ToDouble( byteArr, 12 );
                        box[ 1 ] = BitConverter.ToDouble( byteArr, 20 );
                        box[ 2 ] = BitConverter.ToDouble( byteArr, 28 );
                        box[ 3 ] = BitConverter.ToDouble( byteArr, 36 );

                        int numParts = BitConverter.ToInt32( byteArr, 44 );
                        int numPoints = BitConverter.ToInt32( byteArr, 48 );

                        Console.WriteLine( " numParts : {0}", numParts );
                        Console.WriteLine( " numPoints : {0}", numPoints );

                        // Read Parts
                        int[] parts = new int[numParts];

                        int beginOffset = 52;

                        for ( int partIdx = 0; partIdx < numParts; ++partIdx )
                        {
                            int iOffset = beginOffset + partIdx * sizeof( int );

                            parts[ partIdx ] = BitConverter.ToInt32( byteArr, iOffset );
                        }

                        // Read Points
                        KBPoint[] pts = new KBPoint[numPoints];

                        beginOffset += sizeof( int ) * numParts;

                        bool bZValue = ( 1 == shpType / 10 );
                        int ptsize = ( bZValue ? ( 3 ) : ( 2 ) ) * sizeof( double );

                        for ( int pointIdx = 0; pointIdx < numPoints; ++pointIdx )
                        {
                            int iOffset = beginOffset + ( pointIdx * ptsize );

                            KBPoint pt = new KBPoint();

                            pt.x = BitConverter.ToDouble( byteArr, iOffset );

                            iOffset += sizeof( double );
                            pt.y = BitConverter.ToDouble( byteArr, iOffset );

                            if( bZValue )
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

        public  bool ReadSHPBodyWithoutSHX( BinaryReader br )
        {
            br.BaseStream.Seek( SHPHeaderLength, SeekOrigin.Begin );

            byte[] byteArr;

            while( br.BaseStream.Position < br.BaseStream.Length )
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

    }
}
