using UnityEngine;

namespace Dest.Modeling
{
	public class Quad : PrimitiveBase
	{
		public MeshGenerator.Directions Direction = MeshGenerator.Directions.Back;
		public float Width = 1f;
		public float Height = 1f;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		
		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateQuad(Direction, Width, Height, GenerateNormals, GenerateUVs);
		}
	}
}
