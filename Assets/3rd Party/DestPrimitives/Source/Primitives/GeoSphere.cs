using UnityEngine;

namespace Dest.Modeling
{
	public class GeoSphere : PrimitiveBase
	{
		public float Radius = 1f;
		[Range(0, 6)]
		public int Tesselation = 4; // More than 6 gives error due to 65k vertices overflow
		public bool GenerateNormals = true;
		public bool GenerateUVs = true;
		public bool Invert;

		public override void CreateMesh()
		{
			GeneratedMesh = GeoSphereGenerator.CreateGeoSphere(Radius, Tesselation, GenerateNormals, GenerateUVs, Invert);
		}
	}
}
