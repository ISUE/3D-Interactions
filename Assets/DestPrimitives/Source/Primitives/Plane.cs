using UnityEngine;

namespace Dest.Modeling
{
	public class Plane : PrimitiveBase
	{
		public MeshGenerator.Directions Direction = MeshGenerator.Directions.Back;
		public float Width = 10f;
		public float Height = 10f;
		public int WidthSegments = 4;
		public int HeightSegments = 4;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreatePlane(Direction, Width, Height, WidthSegments, HeightSegments, GenerateNormals, GenerateUVs);
		}
	}
}
