using UnityEngine;
using UnityEditor;

namespace Dest.Modeling
{
	public class PrimitiveBaseEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			DrawDefaultInspector();

			if (EditorGUI.EndChangeCheck())
			{
				CreateMesh();
			}

			EditorGUILayout.Space();
			if (GUILayout.Button("Create Mesh"))
			{
				CreateMesh();
			}
			if (GUILayout.Button("Save Mesh"))
			{
				SaveMesh();
			}
		}

		private void CreateMesh()
		{
			PrimitiveBase primitive = target as PrimitiveBase;
			primitive.CreateMesh();

			MeshFilter meshFilter = primitive.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				if (meshFilter.sharedMesh != null)
				{
					Object.DestroyImmediate(meshFilter.sharedMesh);
				}
				meshFilter.sharedMesh = primitive.GeneratedMesh;
			}
		}

		private void SaveMesh()
		{
			PrimitiveBase primitive = target as PrimitiveBase;
			Mesh mesh = primitive.GeneratedMesh;
			if (mesh != null)
			{
				string path = EditorUtility.SaveFilePanelInProject("Save Mesh", mesh.name + ".asset", "asset", "Save Mesh");
				AssetDatabase.CreateAsset(mesh, path);
				AssetDatabase.Refresh();
			}
			else
			{
				EditorUtility.DisplayDialog("Error", "Mesh is not generated. Generate mesh first.", "OK");
			}
		}
	}
}
