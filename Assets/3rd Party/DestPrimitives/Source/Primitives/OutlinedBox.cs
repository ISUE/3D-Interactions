using UnityEngine;

namespace Dest.Modeling
{
	public class OutlinedBox : PrimitiveBase
	{
		public float SizeX = 1f;
		public float SizeY = 1f;
		public float SizeZ = 1f;
		public float Outline = .1f;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateOutlinedBox(SizeX, SizeY, SizeZ, Outline, GenerateNormals, GenerateUVs);
		}
	}
}
