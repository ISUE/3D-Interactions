using UnityEngine;

namespace Dest.Modeling
{
	public class Cylinder : PrimitiveBase
	{
		public float Radius = .5f;
		public float Height = 2f;
		public int Sides = 30;
		public bool GenerateCaps = true;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		public bool Invert;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateCylinder(Radius, Height, Sides, GenerateCaps, GenerateNormals, GenerateUVs, Invert);
		}
	}
}
