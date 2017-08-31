using System.Collections.Generic;
using UnityEngine;

public class VoxelGridSurface : MonoBehaviour {
	private Mesh mesh;
	private MeshCollider col;

	private List<Vector3> vertices;
	private List<int> triangles;

	public int[] cornersMin, cornersMax;
	public int[] xEdgesMin, xEdgesMax;
	public int yEdgeMin, yEdgeMax;

	public void Init(int Resolution) {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Grid Surface Mesh";
		col = GetComponent<MeshCollider>();
		col.sharedMesh = mesh;
		vertices = new List<Vector3>();
		triangles = new List<int>();

		cornersMax = new int[Resolution + 1];
		cornersMin = new int[Resolution + 1];
		xEdgesMax = new int[Resolution];
		xEdgesMin = new int[Resolution];
	}

	public void Clear () {
		vertices.Clear();
		triangles.Clear();
		mesh.Clear();
	}
	public void Apply () {
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		col.sharedMesh = mesh;
	}

	public void addTriangle(int a,int b,int c) {
		triangles.Add(a);
		triangles.Add(b);
		triangles.Add(c);
	}
	public void addQuad(int a,int b,int c,int d) {
		addTriangle(a,b,c);
		addTriangle(a,c,d);
	}
	public void addPentagon(int a,int b,int c,int d,int e) {
		addTriangle(a,b,c);
		addTriangle(a,c,d);
		addTriangle(a,d,e);
	}

	public void cacheFirstCorner(Voxel voxel) {
		cornersMax[0] = vertices.Count;
		vertices.Add(voxel.position);
	}

	public void cacheNextCorner(int i,Voxel voxel) {
		cornersMax[i + 1] = vertices.Count;
		vertices.Add(voxel.position);
	}

	public void cacheXEdge(int i,Voxel voxel) {
		xEdgesMin[i] = vertices.Count;
		vertices.Add(voxel.XEdgePoint);
	}

	public void cacheYEdge(Voxel voxel) {
		yEdgeMax = vertices.Count;
		vertices.Add(voxel.YEdgePoint);
	}

	public void prepareCacheForNextCell() {
		yEdgeMin = yEdgeMax;
	}

	public void prepareCacheForNextRow() {
		int[] rowSwap = cornersMin;
		cornersMin = cornersMax;
		cornersMax = rowSwap;
		rowSwap = xEdgesMin;
		xEdgesMin = xEdgesMax;
		xEdgesMax = rowSwap;
	}
}