using UnityEngine;

namespace Dest.Modeling
{
	public class Cone : PrimitiveBase
	{
		public float Radius1 = .5f;
		public float Radius2 = 0f;
		public float Height = 2f;
		public int Sides = 30;
		public bool GenerateCaps = true;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		public bool Invert;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateCone(Radius1, Radius2, Height, Sides, GenerateCaps, GenerateNormals, GenerateUVs, Invert);
		}
	}
}
