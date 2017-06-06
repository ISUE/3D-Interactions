// Original code was taken from sharpdx project

#region Original code header

// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// The following code is a port of DirectXTk http://directxtk.codeplex.com
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.

#endregion

using UnityEngine;
using System.Collections.Generic;

namespace Dest.Modeling
{
	public class GeoSphereGenerator
	{
		// An undirected edge between two vertices, represented by a pair of indexes into a vertex array.
		// Becuse this edge is undirected, (a,b) is the same as (b,a).
		private struct UndirectedEdge : System.IEquatable<UndirectedEdge>
		{
			public readonly int Item1;
			public readonly int Item2;

			public UndirectedEdge(int item1, int item2)
			{
				// Makes an undirected edge. Rather than overloading comparison operators to give us the (a,b)==(b,a) property,
				// we'll just ensure that the larger of the two goes first. This'll simplify things greatly.

				if (item1 > item2)
				{
					Item1 = item1;
					Item2 = item2;
				}
				else
				{
					Item1 = item2;
					Item2 = item1;
				}
			}

			public bool Equals(UndirectedEdge other)
			{
				return Item1 == other.Item1 && Item2 == other.Item2;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				return obj is UndirectedEdge && Equals((UndirectedEdge)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Item1.GetHashCode() * 397) ^ Item2.GetHashCode();
				}
			}

			public static bool operator ==(UndirectedEdge left, UndirectedEdge right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(UndirectedEdge left, UndirectedEdge right)
			{
				return !left.Equals(right);
			}
		}


		#region Octaherdon vertices and indices

		private static readonly Vector3[] OctahedronVertices = new Vector3[]
            {
				// when looking down the negative z-axis (into the screen)
                new Vector3( 0,  1,  0), // 0 top
                new Vector3( 0,  0, -1), // 1 front
                new Vector3( 1,  0,  0), // 2 right
                new Vector3( 0,  0,  1), // 3 back
                new Vector3(-1,  0,  0), // 4 left
                new Vector3( 0, -1,  0), // 5 bottom
            };

		private static readonly int[] OctahedronIndices = new int[]
            {
                0, 2, 1, // top front-right face
                0, 3, 2, // top back-right face
                0, 4, 3, // top back-left face
                0, 1, 4, // top front-left face
                5, 4, 1, // bottom front-left face
                5, 3, 4, // bottom back-left face
                5, 2, 3, // bottom back-right face
                5, 1, 2, // bottom front-right face
            };

		#endregion

		private List<Vector3> vertexPositions;
		private List<int> indexList;
		private List<Vector3> vertices;
		private List<Vector3> normals;
		private List<Vector2> uvs;
		private int[] indices;
		// Key: an edge
		// Value: the index of the vertex which lies midway between the two vertices pointed to by the key value
		// This map is used to avoid duplicating vertices when subdividing triangles along edges.
		private Dictionary<UndirectedEdge, int> subdividedEdges;


		private GeoSphereGenerator()
		{
		}

		public static Mesh CreateGeoSphere(float radius, int tessellation, bool generateNormals, bool generateUVs, bool invert)
		{
			GeoSphereGenerator sphere = new GeoSphereGenerator();
			return sphere.Create(radius, tessellation, generateNormals, generateUVs, invert);
		}

		private Mesh Create(float radius, int tessellation, bool generateNormals, bool generateUVs, bool invert)
		{
			if (tessellation < 0) tessellation = 0;

			subdividedEdges = new Dictionary<UndirectedEdge, int>();

			// Start with an octahedron; copy the data into the vertex/index collection.
			vertexPositions = new List<Vector3>(OctahedronVertices);
			indexList = new List<int>(OctahedronIndices);

			// We know these values by looking at the above index list for the octahedron. Despite the subdivisions that are
			// about to go on, these values aren't ever going to change because the vertices don't move around in the array.
			// We'll need these values later on to fix the singularities that show up at the poles.
			int northPoleIndex = 0;
			int southPoleIndex = 5;

			for (int iSubdivision = 0; iSubdivision < tessellation; ++iSubdivision)
			{
				// The new index collection after subdivision.
				var newIndices = new List<int>();
				subdividedEdges.Clear();

				int triangleCount = indexList.Count / 3;
				for (int iTriangle = 0; iTriangle < triangleCount; ++iTriangle)
				{
					// For each edge on this triangle, create a new vertex in the middle of that edge.
					// The winding order of the triangles we output are the same as the winding order of the inputs.

					// Indices of the vertices making up this triangle
					int iv0 = indexList[iTriangle * 3    ];
					int iv1 = indexList[iTriangle * 3 + 1];
					int iv2 = indexList[iTriangle * 3 + 2];

					// Get the new vertices
					Vector3 v01; // vertex on the midpoint of v0 and v1
					Vector3 v12; // ditto v1 and v2
					Vector3 v20; // ditto v2 and v0
					int iv01; // index of v01
					int iv12; // index of v12
					int iv20; // index of v20

					// Add/get new vertices and their indices
					DivideEdge(iv0, iv1, out v01, out iv01);
					DivideEdge(iv1, iv2, out v12, out iv12);
					DivideEdge(iv0, iv2, out v20, out iv20);

					// Add the new indices. We have four new triangles from our original one:
					//        v0
					//        o
					//       /a\
					//  v20 o---o v01
					//     /b\c/d\
					// v2 o---o---o v1
					//       v12

					// a
					newIndices.Add(iv0);
					newIndices.Add(iv01);
					newIndices.Add(iv20);

					// b
					newIndices.Add(iv20);
					newIndices.Add(iv12);
					newIndices.Add(iv2);

					// d
					newIndices.Add(iv20);
					newIndices.Add(iv01);
					newIndices.Add(iv12);

					// d
					newIndices.Add(iv01);
					newIndices.Add(iv1);
					newIndices.Add(iv12);
				}

				indexList.Clear();
				indexList.AddRange(newIndices);
			}

			// Now that we've completed subdivision, fill in the final vertex collection
			int vertexCount = vertexPositions.Count;
			vertices = new List<Vector3>(vertexCount);
			normals = new List<Vector3>(vertexCount);
			uvs = new List<Vector2>(vertexCount);

			for (int i = 0; i < vertexPositions.Count; ++i)
			{
				var vertexValue = vertexPositions[i];

				var normal = vertexValue;
				normal.Normalize();

				var pos = normal * radius;

				// calculate texture coordinates for this vertex
				float longitude = Mathf.Atan2(normal.x, -normal.z);
				float latitude = Mathf.Acos(normal.y);

				float u = (float)(longitude / (Mathf.PI * 2.0) + 0.5);
				float v = 1f - (float)(latitude / Mathf.PI);

				Vector2 texcoord = new Vector2(u, v);

				vertices.Add(pos);
				normals.Add(normal);
				uvs.Add(texcoord);
			}

			float eps = 1e-7f;

			// There are a couple of fixes to do. One is a texture coordinate wraparound fixup. At some point, there will be
			// a set of triangles somewhere in the mesh with texture coordinates such that the wraparound across 0.0/1.0
			// occurs across that triangle. Eg. when the left hand side of the triangle has a U coordinate of 0.98 and the
			// right hand side has a U coordinate of 0.0. The intent is that such a triangle should render with a U of 0.98 to
			// 1.0, not 0.98 to 0.0. If we don't do this fixup, there will be a visible seam across one side of the sphere.
			// 
			// Luckily this is relatively easy to fix. There is a straight edge which runs down the prime meridian of the
			// completed sphere. If you imagine the vertices along that edge, they circumscribe a semicircular arc starting at
			// y=1 and ending at y=-1, and sweeping across the range of z=0 to z=1. x stays zero. It's along this edge that we
			// need to duplicate our vertices - and provide the correct texture coordinates.
			int preCount = vertices.Count;
			indices = indexList.ToArray();

			for (int i = 0; i < preCount; ++i)
			{
				// This vertex is on the prime meridian if position.x and texcoord.u are both zero (allowing for small epsilon).
				bool isOnPrimeMeridian = Mathf.Abs(vertices[i].x) < eps && Mathf.Abs(1f - uvs[i].x) < eps;
				if (isOnPrimeMeridian)
				{
					int newIndex = vertices.Count;

					// copy this vertex, correct the texture coordinate, and add the vertex						
					vertices.Add(vertices[i]);
					normals.Add(normals[i]);
					uvs.Add(new Vector2(0f, uvs[i].y));

					// Now find all the triangles which contain this vertex and update them if necessary
					for (int j = 0; j < indexList.Count; j += 3)
					{
						int triIndex0 = indices[j    ];
						int triIndex1 = indices[j + 1];
						int triIndex2 = indices[j + 2];
						int jTemp = j;

						if (triIndex0 == i)
						{
							// nothing; just keep going
						}
						else if (triIndex1 == i)
						{
							int temp = triIndex0;
							triIndex0 = triIndex1;
							triIndex1 = temp;
							jTemp = j + 1;
						}
						else if (triIndex2 == i)
						{
							int temp = triIndex0;
							triIndex0 = triIndex2;
							triIndex2 = temp;
							jTemp = j + 2;
						}
						else
						{
							// this triangle doesn't use the vertex we're interested in
							continue;
						}

						// check the other two vertices to see if we might need to fix this triangle
						if (Mathf.Abs(uvs[triIndex0].x - uvs[triIndex1].x) > 0.5f ||
							Mathf.Abs(uvs[triIndex0].x - uvs[triIndex2].x) > 0.5f)
						{
							// yep; replace the specified index to point to the new, corrected vertex
							indices[jTemp] = newIndex;
						}
					}
				}
			}

			FixPole(northPoleIndex);
			FixPole(southPoleIndex);


			Mesh m = new Mesh();
			m.name = "GeoSphere";
			m.vertices = vertices.ToArray();
			m.SetIndices(indices, MeshTopology.Triangles, 0);
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
				MeshGenerator.Invert(m);
			}

			return m;
		}

		private void FixPole(int poleIndex)
		{
			Vector3 poleVertexPosition = vertices[poleIndex];
			Vector3 poleVertexNormal = normals[poleIndex];
			Vector2 poleVertexUV = uvs[poleIndex];

			bool overwrittenPoleVertex = false; // overwriting the original pole vertex saves us one vertex

			for (int i = 0; i < indexList.Count; i += 3)
			{
				// These pointers point to the three indices which make up this triangle. pPoleIndex is the pointer to the
				// entry in the index array which represents the pole index, and the other two pointers point to the other
				// two indices making up this triangle.
				
				int pPoleIndex;
				int pOtherIndex0;
				int pOtherIndex1;

				if (indices[i + 0] == poleIndex)
				{
					pPoleIndex   = i;
					pOtherIndex0 = indices[i + 1];
					pOtherIndex1 = indices[i + 2];
				}
				else if (indices[i + 1] == poleIndex)
				{
					pPoleIndex   = i + 1;
					pOtherIndex0 = indices[i + 2];
					pOtherIndex1 = indices[i    ];
				}
				else if (indices[i + 2] == poleIndex)
				{
					pPoleIndex   = i + 2;
					pOtherIndex0 = indices[i    ];
					pOtherIndex1 = indices[i + 1];
				}
				else
				{
					continue;
				}

				// Calculate the texcoords for the new pole vertex, add it to the vertices and update the index

				Vector2 newPoleVertexUV;
				newPoleVertexUV.x = (uvs[pOtherIndex0].x + uvs[pOtherIndex1].x) * 0.5f;
				newPoleVertexUV.y = poleVertexUV.y;

				if (!overwrittenPoleVertex)
				{
					vertices[poleIndex] = poleVertexPosition;
					normals[poleIndex] = poleVertexNormal;
					uvs[poleIndex] = newPoleVertexUV;
					
					overwrittenPoleVertex = true;
				}
				else
				{
					indices[pPoleIndex] = vertices.Count;

					vertices.Add(poleVertexPosition);
					normals.Add(poleVertexNormal);
					uvs.Add(newPoleVertexUV);
				}
			}
		}

		private void DivideEdge(int i0, int i1, out Vector3 outVertex, out int outIndex)
		{
			// Function that, when given the index of two vertices, creates a new vertex at the midpoint of those vertices.

			UndirectedEdge edge = new UndirectedEdge(i0, i1);

			// Check to see if we've already generated this vertex
			if (subdividedEdges.TryGetValue(edge, out outIndex))
			{
				// We've already generated this vertex before
				outVertex = vertexPositions[outIndex]; // and the vertex itself
			}
			else
			{
				// Haven't generated this vertex before: so add it now

				outVertex = (vertexPositions[i0] + vertexPositions[i1]) * 0.5f;
				outIndex = vertexPositions.Count;

				vertexPositions.Add(outVertex);
				subdividedEdges[edge] = outIndex; // Now add it to the map.
			}
		}		
	}
}
