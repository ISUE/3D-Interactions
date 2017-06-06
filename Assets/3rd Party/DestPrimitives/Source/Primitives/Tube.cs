using UnityEngine;

namespace Dest.Modeling
{
	public class Tube : PrimitiveBase
	{
		public float OuterRadius = .5f;
		public float InnerRadius = .4f;
		public float Height = 2f;
		public int Sides = 30;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateTube(OuterRadius, InnerRadius, Height, Sides, GenerateNormals, GenerateUVs);
		}
	}
}
