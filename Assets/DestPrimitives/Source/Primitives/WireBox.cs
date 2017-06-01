using UnityEngine;

namespace Dest.Modeling
{
	public class WireBox : PrimitiveBase
	{
		public float SizeX = 1f;
		public float SizeY = 1f;
		public float SizeZ = 1f;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateWireBox(SizeX, SizeY, SizeZ);
		}
	}
}
