using UnityEngine;

namespace Dest.Modeling
{
	public class Box : PrimitiveBase
	{
		public float SizeX = 1f;
		public float SizeY = 1f;
		public float SizeZ = 1f;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		public bool Invert;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateBox(SizeX, SizeY, SizeZ, GenerateNormals, GenerateUVs, Invert);
		}
	}
}
