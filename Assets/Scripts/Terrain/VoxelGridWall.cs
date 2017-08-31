using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGridWall : MonoBehaviour {
	private Mesh mesh;
	private MeshCollider col;

	private List<Vector3> vertices;
	private List<int> triangles;

	private int[] xEdgesMin, xEdgesMax;
	private int yEdgeMin, yEdgeMax;

	public float bottom, top;

	public void Init(int resolution) {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Grid Wall Mesh";
		col = GetComponent<MeshCollider>();
		col.sharedMesh = mesh;
		vertices = new List<Vector3>();
		triangles = new List<int>();
		xEdgesMin = new int[resolution];
		xEdgesMax = new int[resolution];
    }

	public void Clear() {
		vertices.Clear();
		triangles.Clear();
		mesh.Clear();
	}

	public void Apply() {
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		col.sharedMesh = mesh;
	}

	public void cacheXEdge (int i, Voxel voxel) {
		xEdgesMax[i] = vertices.Count;
		Vector3 v = voxel.XEdgePoint;
		v.z = bottom;
		vertices.Add(v);
		v.z = top;
		vertices.Add(v);
	}

	public void cacheYEdge(Voxel voxel) {
		yEdgeMax = vertices.Count;
		Vector3 v = voxel.YEdgePoint;
		v.z = bottom;
		vertices.Add(v);
		v.z = top;
		vertices.Add(v);
	}

	public void prepareCacheForNextCell() {
		yEdgeMin = yEdgeMax;
	}

	public void prepareCacheForNextRow() {
		int[] swap = xEdgesMin;
		xEdgesMin = xEdgesMax;
		xEdgesMax = swap;
	}

	private void addSection (int a, int b) {
		triangles.Add(a);
		triangles.Add(b + 1);
		triangles.Add(b);
		
		triangles.Add(a);
		triangles.Add(a + 1);
		triangles.Add(b + 1);
	}

	public void AddABAC(int i) {
		addSection(xEdgesMin[i],yEdgeMin);
	}
	public void AddABBD(int i) {
		addSection(xEdgesMin[i],yEdgeMax);
	}
	public void AddABCD(int i) {
		addSection(xEdgesMin[i],xEdgesMax[i]);
	}
	public void AddACAB(int i) {
		addSection(yEdgeMin,xEdgesMin[i]);
	}
	public void AddACBD(int i) {
		addSection(yEdgeMin,yEdgeMax);
	}
	public void AddACCD(int i) {
		addSection(yEdgeMin,xEdgesMax[i]);
	}
	public void AddBDAB(int i) {
		addSection(yEdgeMax,xEdgesMin[i]);
	}
	public void AddBDAC(int i) {
		addSection(yEdgeMax,yEdgeMin);
	}
	public void AddBDCD(int i) {
		addSection(yEdgeMax,xEdgesMax[i]);
	}
	public void AddCDAB(int i) {
		addSection(xEdgesMax[i],xEdgesMin[i]);
	}
	public void AddCDAC(int i) {
		addSection(xEdgesMax[i],yEdgeMin);
	}
	public void AddCDBD(int i) {
		addSection(xEdgesMax[i],yEdgeMax);
	}
}
