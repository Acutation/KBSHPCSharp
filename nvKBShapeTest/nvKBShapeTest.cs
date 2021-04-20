using KBShapeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvKBShapeTest
{
    class nvKBShapeTest
    {
        static void Main( string [] args )
        {
            SHPLoader shpLoader = new SHPLoader();

            shpLoader.Load( @"D:\SHP\CTPRVN_202101\TL_SCCO_CTPRVN.SHP" );
        }
    }
}
