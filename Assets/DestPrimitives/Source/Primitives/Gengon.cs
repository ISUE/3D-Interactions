using UnityEngine;

namespace Dest.Modeling
{
	public class Gengon : PrimitiveBase
	{
		public float Radius = .5f;
		public float Height = 2f;
		public int Sides = 6;
		public bool GenerateCaps = true;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		public bool Invert;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateGengon(Radius, Height, Sides, GenerateCaps, GenerateNormals, GenerateUVs, Invert);
		}
	}
}
