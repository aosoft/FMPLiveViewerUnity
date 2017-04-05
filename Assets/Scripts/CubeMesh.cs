using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class CubeMesh : MonoBehaviour
{
	public bool _lineMesh;

	void Awake()
	{
		var meshfilter = GetComponent<MeshFilter>();
		if (_lineMesh)
		{
			meshfilter.sharedMesh = CreateLineMesh();
		}
		else
		{
			meshfilter.sharedMesh = CreateTriangleMesh();
		}
	}


	static private List<Vector3> _vertices = new List<Vector3>
		{
			new Vector3(-0.5f, 0.0f, -0.5f),
			new Vector3(-0.5f, 1.0f, -0.5f),
			new Vector3( 0.5f, 1.0f, -0.5f),
			new Vector3( 0.5f, 0.0f, -0.5f),
			new Vector3(-0.5f, 0.0f,  0.5f),
			new Vector3(-0.5f, 1.0f,  0.5f),
			new Vector3( 0.5f, 1.0f,  0.5f),
			new Vector3( 0.5f, 0.0f,  0.5f),
		};

	static private int[][] _indicesSurfaces = new int[][]
		{
			new int[] { 0, 1, 2, 2, 3, 0, },
			new int[] { 7, 6, 5, 5, 4, 7, },
			new int[] { 4, 5, 1, 1, 0, 4, },
			new int[] { 3, 2, 6, 6, 7, 3, },
			new int[] { 1, 5, 6, 6, 2, 1, },
			new int[] { 4, 0, 3, 3, 7, 4, },
		};

	static private List<Vector3> _verticesTriangles = null;
	static private List<Vector3> _normalsTriangles = null;
	static private int[] _indicesTriangles = null;

	static private int[] _indicesLines = new int[]
		{
			0, 1, 1, 2, 2, 3, 3, 0,
			7, 6, 6, 5, 5, 4, 4, 7,
			4, 5, 5, 1, 1, 0, 0, 4,
			3, 2, 2, 6, 6, 7, 7, 3,
			1, 5, 5, 6, 6, 2, 2, 1,
			4, 0, 0, 3, 3, 7, 7, 4,
		};

	static private Mesh CreateTriangleMesh()
	{
		var mesh = new Mesh();

		if (_verticesTriangles == null ||
			_normalsTriangles == null ||
			_indicesTriangles == null)
		{
			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			foreach (var surface in _indicesSurfaces)
			{
				Vector3 a = _vertices[surface[1]] - _vertices[surface[0]];
				Vector3 b = _vertices[surface[2]] - _vertices[surface[0]];
				Vector3 c = Vector3.Cross(a, b);

				foreach (var index in surface)
				{
					vertices.Add(_vertices[index]);
					normals.Add(c);
				}
			}
			_verticesTriangles = vertices;
			_normalsTriangles = normals;
			_indicesTriangles = Enumerable.Range(0, vertices.Count).ToArray();
		}

		mesh.SetVertices(_verticesTriangles);
		mesh.SetIndices(_indicesTriangles, MeshTopology.Triangles, 0);
		mesh.SetNormals(_normalsTriangles);

		return mesh;
	}

	static private Mesh CreateLineMesh()
	{
		var mesh = new Mesh();

		mesh.SetVertices(_vertices);
		mesh.SetIndices(_indicesLines, MeshTopology.Lines, 0);

		return mesh;
	}
}
