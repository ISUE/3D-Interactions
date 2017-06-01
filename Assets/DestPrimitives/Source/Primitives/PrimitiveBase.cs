using UnityEngine;

namespace Dest.Modeling
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[ExecuteInEditMode]
	public abstract class PrimitiveBase : MonoBehaviour
	{
		public Mesh GeneratedMesh;

		public abstract void CreateMesh();

		private void OnDestroy()
		{
			if (GeneratedMesh != null)
			{
				Object.DestroyImmediate(GeneratedMesh);
			}
		}
	}
}
