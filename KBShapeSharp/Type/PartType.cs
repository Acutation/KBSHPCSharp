
namespace KBShapeSharp.Type
{
	/// <summary>
	/// Part type enumeration - everything but ShapeType.MultiPatch just uses PartType.Ring.
	/// </summary>
	public enum PartType
	{
		/// <summary>
		/// Linked strip of triangles, where every vertex (after the first two) completes a new triangle.
		/// A new triangle is always formed by connecting the new vertex with its two immediate predecessors.
		/// </summary>
		TriangleStrip = 0,
		/// <summary>
		/// A linked fan of triangles, where every vertex (after the first two) completes a new triangle.
		/// A new triangle is always formed by connecting the new vertex with its immediate predecessor 
		/// and the first vertex of the part.
		/// </summary>
		TriangleFan = 1,
		/// <summary>The outer ring of a polygon</summary>
		OuterRing = 2,
		/// <summary>The first ring of a polygon</summary>
		InnerRing = 3,
		/// <summary>The outer ring of a polygon of an unspecified type</summary>
		FirstRing = 4,
		/// <summary>A ring of a polygon of an unspecified type</summary>
		Ring = 5
	}
}