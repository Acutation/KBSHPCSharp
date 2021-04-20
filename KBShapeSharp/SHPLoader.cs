
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
        SHXInfo m_SHXInfo;
        SHPHeaderInfo m_SHPHeader;

        public bool m_ValidDBF;
        public bool m_ValidSHP;
        public bool m_ValidSHX;
        public KBShapeBase[] m_Shape;

        public SHPLoader()
        {
            m_ValidDBF = false;
            m_ValidSHP = false;
            m_Shape = null;
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
                        ReadSHPheader( br, ref m_SHXInfo.m_ShxHeaderInfo );
                        ReadSHXBody( br, ref m_SHXInfo );

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
                        //Constants.ReadSHPBody( br, ref m_SHXInfo );

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


        public static bool ReadSHPheader( BinaryReader br, ref SHPHeaderInfo shpHeader )
        {
            try
            {
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

        public static bool ReadSHXBody( BinaryReader br, ref SHXInfo shxInfo )
        {
            int shxSize = ( (int)br.BaseStream.Length - SHPHeaderLength ) / SHXRecordSize;
            byte[] byteArr;// = new byte[ SHXRecordSize ];

            shxInfo.m_SHXDataList = new List<SHXData>();

            br.BaseStream.Seek( SHPHeaderLength, SeekOrigin.Begin );

            for ( int idx = 0; idx < shxSize; ++idx )
            {
                int iOffset = SHPHeaderLength + ( idx * SHXRecordSize );
                byteArr = br.ReadBytes( SHXRecordSize );

                SHXData shxData = new SHXData();

                shxData.iOffset = BitConverter.ToInt32( Constants.SwapByte( byteArr, 0, 4 ), 0 );
                shxData.iLength = BitConverter.ToInt32( Constants.SwapByte( byteArr, 4, 4 ), 0 );

                shxInfo.m_SHXDataList.Add( shxData );
            }

            return true;
        }
    }
}
