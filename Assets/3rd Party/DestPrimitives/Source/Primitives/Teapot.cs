using UnityEngine;

namespace Dest.Modeling
{
	public class Teapot : PrimitiveBase
	{
		public float Size = 1.0f;
		public int Tesselation = 8;
		public bool Spoute = true;
		public bool Handle = true;
		public bool Lid = true;
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;

		public override void CreateMesh()
		{
			GeneratedMesh = TeapotGenerator.CreateTeapot(Size, Tesselation, Spoute, Handle, Lid, GenerateNormals, GenerateUVs);
		}
	}
}
