using System;

namespace KBShapeSharp.Type
{
	/// <summary>
	/// xBase field type enumeration
	/// </summary>
	public enum DBFFieldType
	{
		///<summary>String data type</summary> 
		FTString = 0,
		///<summary>Integer data type</summary>
		FTInteger = 1,
		///<summary>Double data type</summary> 
		FTDouble = 2,
		///<summary>Logical data type</summary>
		FTLogical = 3,
		///<summary>Invalid data type</summary>
		FTInvalid = 4,
		/// <summary>Date data type</summary>
		FTDate = 5
	};
}