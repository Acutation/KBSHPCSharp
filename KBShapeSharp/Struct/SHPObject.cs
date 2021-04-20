using System;
using KBShapeSharp.Type;

namespace KBShapeSharp.Struct
{
    /// <summary>
    /// SHPObject - represents on shape (without attributes) read from the .shp file.
    /// </summary>
    /// 
    /// 멀티파트를 고려하면 어떻게 만들어야 할까...
    /// 고민좀 해보자
    public class SHPObject
    {
        /////<summary>Shape type as a ShapeType enum</summary>	
        public SHPType shpType;
        /////<summary>Shape number (-1 is unknown/unassigned)</summary>	
        public int nShapeId;
        /////<summary>Number of parts (0 implies single part with no info)</summary>	
        public int nParts;
        /////<summary>Pointer to int array of part start offsets, of size nParts</summary>	
        //public IntPtr paPartStart;
        /////<summary>Pointer to PartType array (PartType.Ring if not ShapeType.MultiPatch) of size nParts</summary>	
        //public IntPtr paPartType;
        /////<summary>Number of vertices</summary>	
        //public int nVertices;
        /////<summary>Pointer to double array containing X coordinates</summary>	
        //public double[] padfX;
        /////<summary>Pointer to double array containing Y coordinates</summary>		
        //public double[] padfY;
        /////<summary>Pointer to double array containing Z coordinates (all zero if not provided)</summary>	
        //public double[] padfZ;
        /////<summary>Pointer to double array containing Measure coordinates(all zero if not provided)</summary>	
        //public IntPtr padfM;
        /////<summary>Bounding rectangle's min X</summary>	
        //public double dfXMin;
        /////<summary>Bounding rectangle's min Y</summary>	
        //public double dfYMin;
        /////<summary>Bounding rectangle's min Z</summary>	
        //public double dfZMin;
        /////<summary>Bounding rectangle's min M</summary>	
        //public double dfMMin;
        /////<summary>Bounding rectangle's max X</summary>	
        //public double dfXMax;
        /////<summary>Bounding rectangle's max Y</summary>	
        //public double dfYMax;
        /////<summary>Bounding rectangle's max Z</summary>	
        //public double dfZMax;
        /////<summary>Bounding rectangle's max M</summary>	
        //public double dfMMax;

        
    }
}

