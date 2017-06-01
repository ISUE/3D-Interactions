using UnityEngine;
using System.Collections.Generic;

namespace Dest.Modeling
{
	public static class MeshGenerator
	{
		public enum Directions
		{
			Right,
			Left,
			Up,
			Down,
			Forward,
			Back
		}


		public static void Invert(Mesh m)
		{
			for (int i = 0; i < m.subMeshCount; ++i)
			{
				int[] indices = m.GetIndices(i);
				int temp, k1, k2;
				for (int k = 0; k < indices.Length; k += 3)
				{
					k1 = k + 1;
					k2 = k + 2;
					temp = indices[k1];
					indices[k1] = indices[k2];
					indices[k2] = temp;
				}
				m.SetIndices(indices, MeshTopology.Triangles, i);
			}

			Vector3[] normals = m.normals;
			for (int i = 0; i < normals.Length; ++i)
			{
				normals[i] = -normals[i];
			}
			m.normals = normals;
		}


		public static Mesh CreatePlane(Directions direction, float width, float height, int widthSegments, int heightSegments, bool generateNormals, bool generateUVs)
		{
			Mesh m = new Mesh();
			m.name = "Plane";

			if (widthSegments < 1) widthSegments = 1;
			if (heightSegments < 1) heightSegments = 1;

			Vector3[] frames =
			{
				Vector3.forward, Vector3.up     , Vector3.right  ,
				Vector3.back   , Vector3.up     , Vector3.left   ,
				Vector3.right  , Vector3.forward, Vector3.up     ,
				Vector3.right  , Vector3.back   , Vector3.down   ,
				Vector3.left   , Vector3.up     , Vector3.forward,
				Vector3.right  , Vector3.up     , Vector3.back   ,
			};

			int directionIndex = (int)direction;
			Vector3 frameX = frames[directionIndex * 3];
			Vector3 frameY = frames[directionIndex * 3 + 1];
			Vector3 frameZ = frames[directionIndex * 3 + 2];

			float startX = -width * .5f;
			float startY = -height * .5f;
			float deltaX = width / widthSegments;
			float deltaY = height / heightSegments;
			float deltaU = 1f / widthSegments;
			float deltaV = 1f / heightSegments;

			Vector3 startPoint = frameX * startX + frameY * startY;

			Vector3[] vertices = new Vector3[(widthSegments + 1) * (heightSegments + 1)];
			int[] indices = new int[widthSegments * heightSegments * 6];
			Vector2[] uvs = null;
			if (generateUVs)
			{
				uvs = new Vector2[vertices.Length];
			}

			int vertexCount = 0;
			for (int i = 0; i <= widthSegments; ++i)
			{
				for (int j = 0; j <= heightSegments; ++j)
				{
					float x = i * deltaX;
					float y = j * deltaY;
					vertices[vertexCount] = startPoint + x * frameX + y * frameY;
					if (generateUVs)
					{
						uvs[vertexCount] = new Vector2(i * deltaU, j * deltaV);
					}
					++vertexCount;
				}
			}

			int indexCount = 0;
			for (int i = 0; i < widthSegments; ++i)
			{
				for (int j = 0; j < heightSegments; ++j)
				{
					int i0 = i * (heightSegments + 1) + j;
					int i1 = i * (heightSegments + 1) + j + 1;
					int i2 = (i + 1) * (heightSegments + 1) + j + 1;
					int i3 = (i + 1) * (heightSegments + 1) + j;

					indices[indexCount] = i0;
					++indexCount;
					indices[indexCount] = i1;
					++indexCount;
					indices[indexCount] = i2;
					++indexCount;
					indices[indexCount] = i0;
					++indexCount;
					indices[indexCount] = i2;
					++indexCount;
					indices[indexCount] = i3;
					++indexCount;
				}
			}

			m.vertices = vertices;
			m.SetIndices(indices, MeshTopology.Triangles, 0);
			if (generateUVs)
			{
				m.uv = uvs;
			}

			if (generateNormals)
			{
				Vector3[] normals = new Vector3[vertices.Length];
				for (int i = 0; i < normals.Length; ++i)
				{
					normals[i] = frameZ;
				}
				m.normals = normals;
			}

			return m;
		}

		public static Mesh CreateQuad(Directions direction, float width, float height, bool generateNormals, bool generateUVs)
		{
			Mesh m = new Mesh();
			m.name = "Quad";

			Vector3[] frames =
			{
				Vector3.forward, Vector3.up     , Vector3.right  ,
				Vector3.back   , Vector3.up     , Vector3.left   ,
				Vector3.right  , Vector3.forward, Vector3.up     ,
				Vector3.right  , Vector3.back   , Vector3.down   ,
				Vector3.left   , Vector3.up     , Vector3.forward,
				Vector3.right  , Vector3.up     , Vector3.back   ,
			};

			int directionIndex = (int)direction;
			Vector3 frameX = frames[directionIndex * 3];
			Vector3 frameY = frames[directionIndex * 3 + 1];
			Vector3 frameZ = frames[directionIndex * 3 + 2];

			Vector3 halfX = width * .5f * frameX;
			Vector3 halfY = height * .5f * frameY;

			Vector3[] vertices =
			{
				 halfX - halfY,
				-halfX - halfY,
				-halfX + halfY,
				 halfX + halfY,
			};
			m.vertices = vertices;

			int[] indices =
			{
				0, 1, 2, 0, 2, 3
			};
			m.SetIndices(indices, MeshTopology.Triangles, 0);

			if (generateNormals)
			{
				Vector3[] normals =
				{
					frameZ, frameZ, frameZ, frameZ
				};
				m.normals = normals;
			}

			if (generateUVs)
			{
				Vector2[] uvs = 
				{
					new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f)
				};
				m.uv = uvs;
			}

			return m;
		}

		public static Mesh CreateDisk(Directions direction, float radius, int sides, bool generateNormals, bool generateUVs)
		{
			Mesh m = new Mesh();
			m.name = "Disk";

			if (sides < 3) sides = 3;

			Vector3[] frames =
			{
				Vector3.forward, Vector3.up     , Vector3.right  ,
				Vector3.back   , Vector3.up     , Vector3.left   ,
				Vector3.right  , Vector3.forward, Vector3.up     ,
				Vector3.right  , Vector3.back   , Vector3.down   ,
				Vector3.left   , Vector3.up     , Vector3.forward,
				Vector3.right  , Vector3.up     , Vector3.back   ,
			};

			int directionIndex = (int)direction;
			Vector3 frameX = frames[directionIndex * 3];
			Vector3 frameY = frames[directionIndex * 3 + 1];
			Vector3 frameZ = frames[directionIndex * 3 + 2];

			Vector3[] vertices = new Vector3[sides + 1];
			int[] indices = new int[sides * 3];
			Vector3[] normals = new Vector3[vertices.Length];
			Vector2[] uvs = new Vector2[vertices.Length];

			float deltaTheta = Mathf.PI * 2.0f / sides;
			int poleIndex = vertices.Length - 1;

			for (int i = 0; i < sides; ++i)
			{
				float theta = i * deltaTheta;

				float x = Mathf.Cos(theta) * radius;
				float y = Mathf.Sin(theta) * radius;

				vertices[i] = frameX * x + frameY * y;
				normals[i] = frameZ;
				Vector2 normal = (new Vector2(x, y)).normalized;
				uvs[i] = new Vector2(normal.x * .5f + .5f, normal.y * .5f + .5f);

				int index = i * 3;
				indices[index] = poleIndex;
				indices[index + 1] = (i + 1) % sides;
				indices[index + 2] = i;
			}

			vertices[poleIndex] = new Vector3();
			normals[poleIndex] = frameZ;
			uvs[poleIndex] = new Vector2(.5f, .5f);

			m.vertices = vertices;
			m.SetIndices(indices, MeshTopology.Triangles, 0);
			if (generateNormals)
			{
				m.normals = normals;
			}
			if (generateUVs)
			{
				m.uv = uvs;
			}

			return m;
		}


		public static Mesh CreateBox(float sizeX, float sizeY, float sizeZ, bool generateNormals, bool generateUVs, bool invert)
		{
			Mesh m = new Mesh();
			m.name = "Box";

			float halfX = sizeX * .5f;
			float halfY = sizeY * .5f;
			float halfZ = sizeZ * .5f;

			Vector3 v0 = new Vector3(-halfX, -halfY, -halfZ);
			Vector3 v1 = new Vector3(-halfX, -halfY, +halfZ);
			Vector3 v2 = new Vector3(+halfX, -halfY, +halfZ);
			Vector3 v3 = new Vector3(+halfX, -halfY, -halfZ);
			Vector3 v4 = new Vector3(-halfX, +halfY, -halfZ);
			Vector3 v5 = new Vector3(-halfX, +halfY, +halfZ);
			Vector3 v6 = new Vector3(+halfX, +halfY, +halfZ);
			Vector3 v7 = new Vector3(+halfX, +halfY, -halfZ);

			Vector3[] vertices =
			{
				v0, v1, v5, v4,
				v1, v2, v6, v5,
				v2, v3, v7, v6,
				v3, v0, v4, v7,
				v3, v2, v1, v0,
				v4, v5, v6, v7,
			};

			int[] indices =
			{
				0, 1, 2, 0, 2, 3,
				4, 5, 6, 4, 6, 7,
				8, 9, 10, 8, 10, 11,
				12, 13, 14, 12, 14, 15,
				16, 17, 18, 16, 18, 19,
				20, 21, 22, 20, 22, 23,
			};

			m.vertices = vertices;
			m.SetIndices(indices, MeshTopology.Triangles, 0);

			if (generateNormals)
			{
				Vector3[] normals = new Vector3[]
				{
					Vector3.left, Vector3.left, Vector3.left, Vector3.left,
					Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
					Vector3.right, Vector3.right, Vector3.right, Vector3.right, 
					Vector3.back, Vector3.back, Vector3.back, Vector3.back, 
					Vector3.down, Vector3.down, Vector3.down, Vector3.down, 
					Vector3.up, Vector3.up, Vector3.up, Vector3.up, 
				};
				m.normals = normals;
			}

			if (generateUVs)
			{
				Vector2 uv0 = new Vector2(1f, 0f);
				Vector2 uv1 = new Vector2(0f, 0f);
				Vector2 uv2 = new Vector2(0f, 1f);
				Vector2 uv3 = new Vector2(1f, 1f);

				Vector2[] uvs = 
				{
					uv0, uv1, uv2, uv3,
					uv0, uv1, uv2, uv3,
					uv0, uv1, uv2, uv3,
					uv0, uv1, uv2, uv3,
					uv0, uv1, uv2, uv3,
					uv0, uv1, uv2, uv3,
				};
				m.uv = uvs;
			}

			if (invert)
			{
				Invert(m);
			}

			return m;
		}

		public static Mesh CreateWireBox(float sizeX, float sizeY, float sizeZ)
		{
			Mesh m = new Mesh();
			m.name = "WireBox";

			float halfX = sizeX * .5f;
			float halfY = sizeY * .5f;
			float halfZ = sizeZ * .5f;

			Vector3 v0 = new Vector3(-halfX, -halfY, -halfZ);
			Vector3 v1 = new Vector3(-halfX, -halfY, +halfZ);
			Vector3 v2 = new Vector3(+halfX, -halfY, +halfZ);
			Vector3 v3 = new Vector3(+halfX, -halfY, -halfZ);
			Vector3 v4 = new Vector3(-halfX, +halfY, -halfZ);
			Vector3 v5 = new Vector3(-halfX, +halfY, +halfZ);
			Vector3 v6 = new Vector3(+halfX, +halfY, +halfZ);
			Vector3 v7 = new Vector3(+halfX, +halfY, -halfZ);

			Vector3[] vertices =
			{
				v0, v1, v2, v3,
				v4, v5, v6, v7
			};
			m.vertices = vertices;

			int[] indices = 
			{
				0, 1, 1, 2, 2, 3, 3, 0,
				4, 5, 5, 6, 6, 7, 7, 4,
				0, 4, 1, 5, 2, 6, 3, 7,
			};
			m.SetIndices(indices, MeshTopology.Lines, 0);

			return m;
		}

		public static Mesh CreateOutlinedBox(float sizeX, float sizeY, float sizeZ, float outline, bool generateNormals, bool generateUVs)
		{
			Mesh m = new Mesh();
			m.name = "OutlinedBox";

			Vector3[] frames =
			{
				Vector3.back   , Vector3.up   , Vector3.right  ,
				Vector3.forward, Vector3.up   , Vector3.left   ,
				Vector3.right  , Vector3.up   , Vector3.forward,
				Vector3.left   , Vector3.up   , Vector3.back   ,
				Vector3.back   , Vector3.right, Vector3.down   ,
				Vector3.back   , Vector3.left , Vector3.up     ,
			};

			float halfSizeX = sizeX * .5f;
			float halfSizeY = sizeY * .5f;
			float halfSizeZ = sizeZ * .5f;

			float[] halfSizes =
			{
				halfSizeZ, halfSizeY, halfSizeX,
				halfSizeZ, halfSizeY, halfSizeX,
				halfSizeX, halfSizeY, halfSizeZ,
				halfSizeX, halfSizeY, halfSizeZ,
				halfSizeZ, halfSizeX, halfSizeY,
				halfSizeZ, halfSizeX, halfSizeY,
			};

			int verticesPerSide = 24;
			int indicesPerSide = 48;

			Vector3[] vertices = new Vector3[verticesPerSide * 6];
			int[] indices = new int[indicesPerSide * 6];
			Vector2[] uvs = null;
			if (generateUVs)
			{
				uvs = new Vector2[verticesPerSide * 6];
			}

			for (int frameIndex = 0; frameIndex < 6; ++frameIndex)
			{
				int i0 = frameIndex * 3;
				int i1 = i0 + 1;
				int i2 = i1 + 1;

				Vector3 frameX = frames[i0];
				Vector3 frameY = frames[i1];
				Vector3 frameZ = frames[i2];
				float halfX = halfSizes[i0];
				float halfY = halfSizes[i1];
				float halfZ = halfSizes[i2];

				Vector3 outerX = frameX * halfX;
				Vector3 outerY = frameY * halfY;
				Vector3 outerZ = frameZ * halfZ;

				Vector3 v0 = outerX - outerY - outerZ;
				Vector3 v1 = -outerX - outerY - outerZ;
				Vector3 v2 = -outerX + outerY - outerZ;
				Vector3 v3 = outerX + outerY - outerZ;

				Vector3 innerX = frameX * outline;
				Vector3 innerY = frameY * outline;

				Vector3 v4 = v0 - innerX + innerY;
				Vector3 v5 = v1 + innerX + innerY;
				Vector3 v6 = v2 + innerX - innerY;
				Vector3 v7 = v3 - innerX - innerY;

				Vector3 depth = frameZ * outline;

				Vector3 v8 = v4 + depth;
				Vector3 v9 = v5 + depth;
				Vector3 v10 = v6 + depth;
				Vector3 v11 = v7 + depth;

				int vertexOffset = verticesPerSide * frameIndex;
				Vector3[] sideVertices =
				{
					v0, v1, v2, v3,
					v4, v5, v6, v7,

					v4, v5, v9, v8,
					v5, v6, v10, v9,
					v6, v7, v11, v10,
					v7, v4, v8, v11,
				};
				System.Array.Copy(sideVertices, 0, vertices, vertexOffset, verticesPerSide);

				int[] sideIndices =
				{
					0, 1, 5, 0, 5, 4,
					1, 2, 6, 1, 6, 5,
					2, 3, 7, 2, 7, 6,
					3, 0, 4, 3, 4, 7,

					8, 9, 10, 8, 10, 11,
					12, 13, 14, 12, 14, 15,
					16, 17, 18, 16, 18, 19,
					20, 21, 22, 20, 22, 23,
				};
				for (int k = 0; k < sideIndices.Length; ++k)
				{
					sideIndices[k] += vertexOffset;
				}
				System.Array.Copy(sideIndices, 0, indices, indicesPerSide * frameIndex, indicesPerSide);

				if (generateUVs)
				{
					float outlineAbs = Mathf.Abs(outline);
					float deltaU = outlineAbs / (Mathf.Abs(halfX) * 2.0f);
					float deltaV = outlineAbs / (Mathf.Abs(halfY) * 2.0f);

					Vector2[] sideUvs =
					{
						new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f),
						new Vector2(1f - deltaU, deltaV), new Vector2(deltaU, deltaV), new Vector2(deltaU, 1f - deltaV), new Vector2(1f - deltaU, 1f - deltaV),

						new Vector2(1f - deltaU, 0f), new Vector2(deltaU, 0f), new Vector2(deltaU, deltaV), new Vector2(1f - deltaU, deltaV),
						Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, 
						Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, 
						Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, 
					};
					System.Array.Copy(sideUvs, 0, uvs, verticesPerSide * frameIndex, verticesPerSide);
				}
			}
			
			m.vertices = vertices;			
			m.SetIndices(indices, MeshTopology.Triangles, 0);

			if (generateNormals)
			{
				m.RecalculateNormals();
			}

			if (generateUVs)
			{
				for (int i = 0; i < indices.Length; i += 3)
				{
					int index0 = indices[i    ];
					int index1 = indices[i + 1];
					int index2 = indices[i + 2];

					Vector3 v0 = vertices[index0];
					Vector3 v1 = vertices[index1];
					Vector3 v2 = vertices[index2];
					Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

					float minDiff = float.NegativeInfinity;
					int minIndex = -1;
					for (int k = 0; k < 6; ++k)
					{
						Vector3 direction = -frames[k * 3 + 2];
						float dot = Vector3.Dot(normal, direction);
						if (dot > minDiff)
						{
							minDiff = dot;
							minIndex = k;
						}
					}

					int i0 = minIndex * 3;
					int i1 = i0 + 1;
					int i2 = i1 + 1;

					Vector3 frameY =  frames[i1];
					Vector3 frameZ = -frames[i2];
					
					float maxX  = halfSizes[i0];
					float fullX = maxX * 2.0f;
					float maxY  = halfSizes[i1];
					float fullY = maxY * 2.0f;

					Quaternion rotation = Quaternion.LookRotation(frameZ, frameY);
					rotation = Quaternion.Inverse(rotation);

					Vector3 v0Local = rotation * v0;
					Vector3 v1Local = rotation * v1;
					Vector3 v2Local = rotation * v2;

					uvs[index0] = new Vector2((maxX - v0Local.x) / fullX, 1f - (maxY - v0Local.y) / fullY);
					uvs[index1] = new Vector2((maxX - v1Local.x) / fullX, 1f - (maxY - v1Local.y) / fullY);
					uvs[index2] = new Vector2((maxX - v2Local.x) / fullX, 1f - (maxY - v2Local.y) / fullY);
				}
				m.uv = uvs;
			}

			return m;
		}		

		public static Mesh CreateArrow(Directions direction, float lineLength, float lineThickness, float capLength, float capThickness, float capOverhang, bool generateNormals)
		{
			Mesh m = new Mesh();
			m.name = "Arrow";

			float lineSize = lineThickness * .5f;
			float capSize = capThickness * .5f;
			Vector3[] vertices = new Vector3[44];

			vertices[0] = vertices[7] = vertices[10] = new Vector3(+lineSize, +lineSize, 0);
			vertices[1] = vertices[6] = vertices[19] = new Vector3(+lineSize, -lineSize, 0);
			vertices[2] = vertices[15] = vertices[18] = new Vector3(-lineSize, -lineSize, 0);
			vertices[3] = vertices[11] = vertices[14] = new Vector3(-lineSize, +lineSize, 0);
			vertices[4]  = vertices[9]  = new Vector3(+lineSize, +lineSize, lineLength);
			vertices[5]  = vertices[16] = new Vector3(+lineSize, -lineSize, lineLength);
			vertices[12] = vertices[17] = new Vector3(-lineSize, -lineSize, lineLength);
			vertices[8]  = vertices[13] = new Vector3(-lineSize, +lineSize, lineLength);

			float capCoord = lineLength - capOverhang;
			vertices[20] = vertices[24] = vertices[29] = new Vector3(+capSize, +capSize, capCoord);
			vertices[21] = vertices[26] = vertices[33] = new Vector3(+capSize, -capSize, capCoord);
			vertices[22] = vertices[30] = vertices[35] = new Vector3(-capSize, -capSize, capCoord);
			vertices[23] = vertices[27] = vertices[32] = new Vector3(-capSize, +capSize, capCoord);
			vertices[25] = vertices[28] = vertices[31] = vertices[34] = new Vector3(0, 0, lineLength + capLength);

			vertices[36] = vertices[37] = vertices[38] = vertices[39] = new Vector3(0f, 0f, lineLength);
			vertices[40] = vertices[20];
			vertices[41] = vertices[21];
			vertices[42] = vertices[22];
			vertices[43] = vertices[23];

			if (direction != Directions.Forward)
			{
				Quaternion rotation;
				switch (direction)
				{
					case Directions.Back: rotation = Quaternion.Euler(0f, 180f, 0f); break;
					case Directions.Right: rotation = Quaternion.Euler(0f, 90f, 0f); break;
					case Directions.Left: rotation = Quaternion.Euler(0f, -90f, 0f); break;
					case Directions.Up: rotation = Quaternion.Euler(-90f, 0f, 0f); break;
					case Directions.Down: rotation = Quaternion.Euler(90f, 0f, 0f); break;
					default: rotation = Quaternion.identity; break;
				}
				for (int i = 0; i < vertices.Length; ++i)
				{
					vertices[i] = rotation * vertices[i];
				}
			}

			int[] indices = new int[]
			{
				0, 1, 2, 0, 2, 3,
				4, 5, 6, 4, 6, 7,
				8, 9, 10, 8, 10, 11,
				12, 13, 14, 12, 14, 15,
				16, 17, 18, 16, 18, 19,
				20, 21, 36, 41, 42, 37, 22, 23, 38, 43, 40, 39,
				24, 25, 26,
				27, 28, 29,
				30, 31, 32,
				33, 34, 35
			};

			m.vertices = vertices;
			m.SetIndices(indices, MeshTopology.Triangles, 0);
			
			if (generateNormals)
			{
				m.RecalculateNormals();
			}

			m.uv = new Vector2[vertices.Length];

			return m;
		}

		public static Mesh CreateCylinder(float radius, float height, int sides, bool generateCaps, bool generateNormals, bool generateUVs, bool invert)
		{
			Mesh m = new Mesh();
			m.name = "Cylinder";

			if (sides < 3) sides = 3;

			List<Vector3> vertices = new List<Vector3>(sides * 4 + (sides * 2 + 1) * 2);
			List<int> indices = new List<int>(sides * 12);
			List<Vector3> normals = new List<Vector3>(vertices.Capacity);
			List<Vector2> uvs = new List<Vector2>(vertices.Capacity);

			float deltaTheta = Mathf.PI * 2.0f / sides;

			for (int i = 0; i <= sides; ++i)
			{
				float theta = i * deltaTheta;

				Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
				Vector3 normal = position.normalized;
				float u = (float)i / sides;

				vertices.Add(position);
				normals.Add(normal);
				uvs.Add(new Vector2(u, 0f));

				position.y = height;
				vertices.Add(position);
				normals.Add(normal);
				uvs.Add(new Vector2(u, 1f));
			}

			for (int i = 0; i < sides; ++i)
			{
				int i0 = i * 2;
				int i1 = i0 + 1;
				int i2 = i1 + 1;
				int i3 = i2 + 1;

				indices.Add(i0);
				indices.Add(i1);
				indices.Add(i3);

				indices.Add(i0);
				indices.Add(i3);
				indices.Add(i2);
			}

			if (generateCaps)
			{
				int baseIndex = vertices.Count;

				for (int i = 0; i <= sides; ++i)
				{
					float theta = i * deltaTheta;

					Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
					Vector3 normal = position.normalized;
					Vector2 uv = new Vector2(normal.x * .5f + .5f, normal.z * .5f + .5f);

					vertices.Add(position);
					normals.Add(Vector3.down);
					uvs.Add(uv);

					position.y = height;
					vertices.Add(position);
					normals.Add(Vector3.up);
					uvs.Add(uv);
				}

				int botPoleIndex = vertices.Count;
				int topPoleIndex = botPoleIndex + 1;
				
				vertices.Add(new Vector3(0f, 0f, 0f));
				normals.Add(Vector3.down);
				uvs.Add(new Vector2(.5f, .5f));

				vertices.Add(new Vector3(0f, height, 0f));
				normals.Add(Vector3.up);
				uvs.Add(new Vector2(.5f, .5f));

				for (int i = 0; i < sides; ++i)
				{
					indices.Add(baseIndex + i * 2);
					indices.Add(baseIndex + i * 2 + 2);
					indices.Add(botPoleIndex);

					indices.Add(baseIndex + i * 2 + 3);
					indices.Add(baseIndex + i * 2 + 1);
					indices.Add(topPoleIndex);
				}
			}

			m.vertices = vertices.ToArray();
			m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
			if (generateNormals)
			{
				m.normals = normals.ToArray();
			}
			if (generateUVs)
			{
				m.uv = uvs.ToArray();
			}
			if (invert)
			{
				Invert(m);
			}

			return m;
		}

		public static Mesh CreateTube(float outerRadius, float innerRadius, float height, int sides, bool generateNormals, bool generateUVs)
		{
			Mesh m = new Mesh();
			m.name = "Tube";

			if (sides < 3) sides = 3;
			if (innerRadius > outerRadius) innerRadius = outerRadius;

			List<Vector3> vertices = new List<Vector3>();
			List<int> indices = new List<int>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();

			float deltaTheta = Mathf.PI * 2.0f / sides;
			float radiusDiff = outerRadius - innerRadius;
			float total = 2f * (height + radiusDiff);
			float v1 = height / total;
			float v2 = v1 + radiusDiff / total;
			float v3 = v2 + v1;

			for (int i = 0; i <= sides; ++i)
			{
				float theta = i * deltaTheta;

				float x = Mathf.Cos(theta);
				float y = Mathf.Sin(theta);

				Vector3 position = new Vector3(outerRadius * x, 0f, outerRadius * y);
				Vector3 normal = position.normalized;
				Vector2 uv = new Vector2((float)i / sides, 0f);

				vertices.Add(position);
				vertices.Add(position);
				normals.Add(Vector3.down);
				normals.Add(normal);
				uvs.Add(new Vector2(uv.x, 1f));
				uvs.Add(uv);

				position.y = height;
				uv.y = v1;

				vertices.Add(position);
				vertices.Add(position);
				normals.Add(normal);
				normals.Add(Vector3.up);
				uvs.Add(uv);
				uvs.Add(uv);

				position = new Vector3(innerRadius * x, height, innerRadius * y);
				normal = -normal;
				uv.y = v2;

				vertices.Add(position);
				vertices.Add(position);
				normals.Add(Vector3.up);
				normals.Add(normal);
				uvs.Add(uv);
				uvs.Add(uv);

				position.y = 0f;
				uv.y = v3;

				vertices.Add(position);
				vertices.Add(position);
				normals.Add(normal);
				normals.Add(Vector3.down);
				uvs.Add(uv);
				uvs.Add(uv);
			}

			int i0, i1, i2, i3;

			for (int i = 0; i < sides; ++i)
			{
				// Front
				i0 = i * 8 + 1;
				i1 = i0 + 1;
				i2 = (i + 1) * 8 + 1;
				i3 = i2 + 1;

				indices.Add(i0);
				indices.Add(i1);
				indices.Add(i3);
				indices.Add(i0);
				indices.Add(i3);
				indices.Add(i2);

				// Top
				i0 = i1 + 1;
				i1 = i0 + 1;
				i2 = i3 + 1;
				i3 = i2 + 1;

				indices.Add(i0);
				indices.Add(i1);
				indices.Add(i3);
				indices.Add(i0);
				indices.Add(i3);
				indices.Add(i2);

				// Back
				i0 = i1 + 1;
				i1 = i0 + 1;
				i2 = i3 + 1;
				i3 = i2 + 1;

				indices.Add(i0);
				indices.Add(i1);
				indices.Add(i3);
				indices.Add(i0);
				indices.Add(i3);
				indices.Add(i2);

				// Bottom
				i0 = i1 + 1;
				i1 = i * 8;
				i2 = i3 + 1;
				i3 = (i + 1) * 8;

				indices.Add(i0);
				indices.Add(i1);
				indices.Add(i3);
				indices.Add(i0);
				indices.Add(i3);
				indices.Add(i2);
			}

			m.vertices = vertices.ToArray();
			m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
			if (generateNormals)
			{
				m.normals = normals.ToArray();
			}
			if (generateUVs)
			{
				m.uv = uvs.ToArray();
			}

			return m;
		}

		public static Mesh CreateGengon(float radius, float height, int sides, bool generateCaps, bool generateNormals, bool generateUVs, bool invert)
		{
			Mesh m = new Mesh();
			m.name = "Gengon";

			if (sides < 3) sides = 3;

			List<Vector3> vertices = new List<Vector3>(sides * 4 + (sides * 2 + 1) * 2);
			List<int> indices = new List<int>(sides * 12);
			List<Vector2> uvs = new List<Vector2>(vertices.Capacity);

			float deltaTheta = Mathf.PI * 2.0f / sides;

			for (int i = 0; i < sides; ++i)
			{
				float theta = i * deltaTheta;

				Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
				float u = (float)i / sides;

				vertices.Add(position);
				vertices.Add(position);
				uvs.Add(new Vector2(u, 0f));
				uvs.Add(new Vector2(u, 0f));

				position.y = height;
				vertices.Add(position);
				vertices.Add(position);
				uvs.Add(new Vector2(u, 1f));
				uvs.Add(new Vector2(u, 1f));
			}

			uvs[1] = new Vector2(1f, 0f);
			uvs[3] = new Vector2(1f, 1f);

			for (int i0 = sides - 1, i1 = 0; i1 < sides; i0 = i1, ++i1)
			{
				int ind0 = i0 * 4;
				int ind1 = ind0 + 2;
				int ind2 = i1 * 4 + 1;
				int ind3 = ind2 + 2;

				indices.Add(ind0);
				indices.Add(ind1);
				indices.Add(ind3);

				indices.Add(ind0);
				indices.Add(ind3);
				indices.Add(ind2);
			}

			if (generateCaps)
			{
				int baseIndex = vertices.Count;

				for (int i = 0; i < sides; ++i)
				{
					float theta = i * deltaTheta;

					Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
					Vector3 normal = position.normalized;
					Vector2 uv = new Vector2(normal.x * .5f + .5f, normal.z * .5f + .5f);

					vertices.Add(position);
					uvs.Add(uv);

					position.y = height;
					vertices.Add(position);
					uvs.Add(uv);
				}

				int botPoleIndex = vertices.Count;
				int topPoleIndex = botPoleIndex + 1;

				vertices.Add(new Vector3(0f, 0f, 0f));
				uvs.Add(new Vector2(.5f, .5f));

				vertices.Add(new Vector3(0f, height, 0f));
				uvs.Add(new Vector2(.5f, .5f));

				for (int i0 = sides - 1, i1 = 0; i1 < sides; i0 = i1, ++i1)
				{
					indices.Add(baseIndex + i0 * 2);
					indices.Add(baseIndex + i1 * 2);
					indices.Add(botPoleIndex);

					indices.Add(baseIndex + i0 * 2 + 1);
					indices.Add(topPoleIndex);
					indices.Add(baseIndex + i1 * 2 + 1);
				}
			}

			m.vertices = vertices.ToArray();
			m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

			if (generateNormals)
			{
				m.RecalculateNormals();
			}
			if (generateUVs)
			{
				m.uv = uvs.ToArray();
			}
			if (invert)
			{
				Invert(m);
			}

			return m;
		}

		public static Mesh CreateCone(float radius1, float radius2, float height, int sides, bool generateCaps, bool generateNormals, bool generateUVs, bool invert)
		{
			Mesh m = new Mesh();
			m.name = "Cone";

			if (sides < 3) sides = 3;

			List<Vector3> vertices = new List<Vector3>((sides + 1) * 2 + 2);
			List<int> indices = new List<int>(sides * 4);
			List<Vector2> uvs = new List<Vector2>(vertices.Capacity);

			float deltaTheta = Mathf.PI * 2.0f / sides;

			for (int i = 0; i <= sides; ++i)
			{
				float theta = i * deltaTheta;
				float u = (float)i / sides;

				Vector3 position0 = new Vector3(radius1 * Mathf.Cos(theta), 0f, radius1 * Mathf.Sin(theta));				
				vertices.Add(position0);
				uvs.Add(new Vector2(u, 0f));

				Vector3 position1 = new Vector3(radius2 * Mathf.Cos(theta), height, radius2 * Mathf.Sin(theta));
				vertices.Add(position1);
				uvs.Add(new Vector2(u, 1f));
			}

			for (int i = 0; i < sides; ++i)
			{
				int i0 = i * 2;
				int i1 = i0 + 1;
				int i2 = i1 + 1;
				int i3 = i2 + 1;

				indices.Add(i0);
				indices.Add(i1);
				indices.Add(i3);

				indices.Add(i0);
				indices.Add(i3);
				indices.Add(i2);
			}

			if (generateCaps)
			{
				int baseIndex = vertices.Count;
				bool genTop = radius2 > 0f;

				for (int i = 0; i <= sides; ++i)
				{
					float theta = i * deltaTheta;

					Vector3 position0 = new Vector3(radius1 * Mathf.Cos(theta), 0f, radius1 * Mathf.Sin(theta));
					Vector3 normal0 = position0.normalized;
					Vector2 uv = new Vector2(normal0.x * .5f + .5f, normal0.z * .5f + .5f);

					vertices.Add(position0);
					uvs.Add(uv);

					if (genTop)
					{
						Vector3 position1 = new Vector3(radius2 * Mathf.Cos(theta), height, radius2 * Mathf.Sin(theta));

						vertices.Add(position1);
						uvs.Add(uv);
					}
				}

				int botPoleIndex = vertices.Count;
				int topPoleIndex = botPoleIndex + 1;

				vertices.Add(new Vector3(0f, 0f, 0f));
				uvs.Add(new Vector2(.5f, .5f));

				if (genTop)
				{
					vertices.Add(new Vector3(0f, height, 0f));
					uvs.Add(new Vector2(.5f, .5f));
				}

				if (genTop)
				{
					for (int i = 0; i < sides; ++i)
					{
						indices.Add(baseIndex + i * 2);
						indices.Add(baseIndex + i * 2 + 2);
						indices.Add(botPoleIndex);

						indices.Add(baseIndex + i * 2 + 3);
						indices.Add(baseIndex + i * 2 + 1);
						indices.Add(topPoleIndex);
					}
				}
				else
				{
					for (int i = 0; i < sides; ++i)
					{
						indices.Add(baseIndex + i);
						indices.Add(baseIndex + i + 1);
						indices.Add(botPoleIndex);
					}
				}
			}

			m.vertices = vertices.ToArray();
			m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
			if (generateNormals)
			{
				m.RecalculateNormals();
			}
			if (generateUVs)
			{
				m.uv = uvs.ToArray();
			}
			if (invert)
			{
				Invert(m);
			}

			return m;
		}

		public static Mesh CreateSphere(float radius, int slices, int stacks, float slicesMaxAngle, float stacksMaxAngle, bool generateNormals, bool generateUVs, bool invert)
		{
			Mesh m = new Mesh();
			m.name = "Sphere";

			if (slices < 3) slices = 3;
			if (stacks < 3) stacks = 3;

			int vertexCapacity = (slices + 1) * (stacks + 1);

			List<Vector3> vertices = new List<Vector3>(vertexCapacity);
			List<int>     indices  = new List<int>(slices * stacks * 6);
			List<Vector3> normals  = new List<Vector3>(vertexCapacity);
			List<Vector2> uvs      = new List<Vector2>(vertexCapacity);

			float stacksAngle = stacksMaxAngle * Mathf.Deg2Rad;
			float slicesAngle = slicesMaxAngle * Mathf.Deg2Rad;
			float phiStep = stacksAngle / stacks;
			float thetaStep = slicesAngle / slices;

			for (int i = 0; i <= stacks; ++i)
			{
				float phi = i * phiStep;

				for (int j = 0; j <= slices; ++j)
				{
					float theta = j * thetaStep;

					Vector3 position = new Vector3(radius * Mathf.Sin(phi) * Mathf.Cos(theta), radius * Mathf.Cos(phi), radius * Mathf.Sin(phi) * Mathf.Sin(theta));
					vertices.Add(position);
					normals.Add(position.normalized);
					uvs.Add(new Vector2(theta / slicesAngle, 1f - phi / stacksAngle));
				}
			}
						
			int ringVertexCount = slices + 1;
			for (int i = 0; i < stacks; ++i)
			{
				for (int j = 0; j < slices; ++j)
				{
					indices.Add(i       * ringVertexCount + j    );
					indices.Add(i       * ringVertexCount + j + 1);
					indices.Add((i + 1) * ringVertexCount + j    );

					indices.Add((i + 1) * ringVertexCount + j    );
					indices.Add(i       * ringVertexCount + j + 1);
					indices.Add((i + 1) * ringVertexCount + j + 1);
				}
			}

			m.vertices = vertices.ToArray();
			m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

			if (generateNormals)
			{
				m.normals = normals.ToArray();
			}

			if (generateUVs)
			{
				m.uv = uvs.ToArray();
			}

			if (invert)
			{
				Invert(m);
			}
			
			return m;
		}

		public static Mesh CreateTorus(float radius, float thickness, int slices, int sliceTessellation, bool generateNormals, bool generateUVs)
		{
			Mesh m = new Mesh();
			m.name = "Torus";

			if (slices < 3) slices = 3;
			if (sliceTessellation < 3) sliceTessellation = 3;

			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> indices = new List<int>();

			int circleVertexCount = slices + 1;
			int sliceVertexCount = sliceTessellation + 1;
			float pi = Mathf.PI;
			float twoPi = Mathf.PI * 2.0f;
			float invSlices = 1f / slices;
			float intSliceTessellation = 1f / sliceTessellation;
			Vector3 axisY = Vector3.up;

			for (int i = 0; i <= slices; ++i)
			{
				float u = i * invSlices;
				float circleAngle = u * twoPi;
				
				Vector3 axisX = new Vector3(Mathf.Cos(circleAngle), 0f, Mathf.Sin(circleAngle));
				Vector3 center = new Vector3(axisX.x * radius, 0f, axisX.z * radius);

				for (int j = 0; j <= sliceTessellation; ++j)
				{
					float v = j * intSliceTessellation;
					float tubeAngle = v * twoPi + pi;

					float x = Mathf.Cos(tubeAngle);
					float y = Mathf.Sin(tubeAngle);

					Vector3 xVector = x * axisX;
					Vector3 yVector = y * axisY;

					Vector3 vertex = xVector * thickness + yVector * thickness + center;
					Vector3 normal = xVector + yVector;
					Vector2 uv = new Vector2(u, v);

					vertices.Add(vertex);
					normals.Add(normal);
					uvs.Add(uv);

					int iNext = (i + 1) % circleVertexCount;
					int jNext = (j + 1) % sliceVertexCount;

					indices.Add(i     * sliceVertexCount + j    );
					indices.Add(i     * sliceVertexCount + jNext);
					indices.Add(iNext * sliceVertexCount + j);

					indices.Add(i     * sliceVertexCount + jNext);
					indices.Add(iNext * sliceVertexCount + jNext);
					indices.Add(iNext * sliceVertexCount + j    );
				}
			}

			m.vertices = vertices.ToArray();
			m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
			if (generateNormals)
			{
				m.normals = normals.ToArray();
			}
			if (generateUVs)
			{
				m.uv = uvs.ToArray();
			}

			return m;
		}
	}
}
