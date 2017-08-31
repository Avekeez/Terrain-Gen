using UnityEngine;
using System;

[Serializable]
public class Voxel {
	public bool State;

	public Vector2 position;

	public float xEdge, yEdge;

	public Voxel() { }

	/*
		y

		pos  x
	*/
	public Voxel (int x, int y, float size) {
		position.x = (x + 0.5f) * size;
		position.y = (y + 0.5f) * size;

		xEdge = position.x + size * 0.5f;
		yEdge = position.y + size * 0.5f;
	}

	public void BecomeXDummyOf (Voxel voxel, float offset) {
		State = voxel.State;
		position = voxel.position;
		position.x += offset;
		xEdge = voxel.xEdge + offset;
		yEdge = voxel.yEdge;
	}

	public void BecomeYDummyOf(Voxel voxel,float offset) {
		State = voxel.State;
		position = voxel.position;
		position.y += offset;
		xEdge = voxel.xEdge;
		yEdge = voxel.yEdge + offset;
	}

	public void BecomeXYDummyOf(Voxel voxel, float offset) {
		State = voxel.State;
		position = voxel.position;
		position.x += offset;
		position.y += offset;
		xEdge = voxel.xEdge + offset;
		yEdge = voxel.yEdge + offset;
	}

	public Vector2 XEdgePoint {
		get {
			return new Vector2(xEdge,position.y);
		}
	}
	public Vector2 YEdgePoint {
		get {
			return new Vector2(position.x,yEdge);
		}
	}
}
