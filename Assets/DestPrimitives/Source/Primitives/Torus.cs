using UnityEngine;

namespace Dest.Modeling
{
	public class Torus : PrimitiveBase
	{
		public float Radius = 1f;
		public float Thickness = 0.3f;
		public int Tessellation = 30;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateTorus(Radius, Thickness, Tessellation * 2, Tessellation, GenerateNormals, GenerateUVs);
		}
	}
}
