using UnityEngine;

namespace Dest.Modeling
{
	public class Sphere : PrimitiveBase
	{
		public float Radius = 1f;
		public int Tesselation = 30;
		[Range(0f, 360f)]
		public float SlicesMaxAngle = 360f;
		[Range(0f, 180f)]
		public float StacksMaxAngle = 180f;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		public bool Invert;

		public override void CreateMesh()
		{
			GeneratedMesh = MeshGenerator.CreateSphere(Radius, Tesselation * 2, Tesselation, SlicesMaxAngle, StacksMaxAngle, GenerateNormals, GenerateUVs, Invert);
		}
	}
}
