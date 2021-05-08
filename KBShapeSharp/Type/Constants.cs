using KBShapeSharp.Struct;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBShapeSharp.Type
{
    public class Constants
    {

        public static byte[] SwapByte( byte[] byteArray, int startIndex, int length )
        {
            if( length > byteArray.Length )
            {
                return null;
            }

            byte [] returnValue = new byte[ length ];

            for( int count = 0; count < length ; ++ count )
            {
                int idx = length + startIndex - ( count + 1 );

                returnValue [ count ] = byteArray [ idx ];
            }

            return returnValue;
        }

        public static byte[] GetByteArray( int intValue )
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);

            if ( !BitConverter.IsLittleEndian )
            {
                Array.Reverse( intBytes );
            }

            return intBytes;
        }

        public static byte[] GetByteArray( short intValue )
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);

            if ( BitConverter.IsLittleEndian )
            {
                Array.Reverse( intBytes );
            }

            return intBytes;
        }

        public static byte BooleanToByte( bool bValue )
        {
            if ( bValue )
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
