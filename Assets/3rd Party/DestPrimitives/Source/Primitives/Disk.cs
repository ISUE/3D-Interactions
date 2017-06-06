using UnityEngine;

namespace Dest.Modeling
{
	public class Disk : PrimitiveBase
	{
		public MeshGenerator.Directions Direction = MeshGenerator.Directions.Up;
		public float Radius = 1f;
		public int Sides = 30;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateDisk(Direction, Radius, Sides, GenerateNormals, GenerateUVs);
		}
	}
}
