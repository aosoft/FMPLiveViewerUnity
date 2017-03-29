using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	static private int[] _indicesTriangles = new int[]
		{
			0, 1, 2, 2, 3, 0,
			7, 6, 5, 5, 4, 7,
			4, 5, 1, 1, 0, 4,
			3, 2, 6, 6, 7, 3,
			1, 5, 6, 6, 2, 1,
			4, 0, 3, 3, 7, 4,
		};

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

		mesh.SetVertices(_vertices);
		mesh.SetIndices(_indicesTriangles, MeshTopology.Triangles, 0);

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
