using UnityEngine;

namespace Dest.Modeling
{
	public class Arrow : PrimitiveBase
	{
		public MeshGenerator.Directions Direction = MeshGenerator.Directions.Forward;
		public float LineLength = 1f;
		public float LineThickness = .1f;
		public float CapLength = .7f;
		public float CapThickness = .2f;
		public float CapOverhang = 0f;
		public bool GenerateNormals = true;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateArrow(Direction, LineLength, LineThickness, CapLength, CapThickness, CapOverhang, GenerateNormals);
		}
	}
}
