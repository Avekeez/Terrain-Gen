using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMap : VoxelMap {

	protected override void Awake() {
		base.Awake();
	}
	public void Init (int seed,float lower,float upper) {
		EditVoxels(seed,lower,upper);
	}

	protected override void Update() { }
	protected override void OnGUI() { }

	protected void EditVoxels(int seed,float lower,float upper) {
		int centerX = (int)(halfSize / voxelSize);
		int centerY = (int)(halfSize / voxelSize);

		int xStart = (centerX - ChunkResolution * VoxelResolution - 1) / VoxelResolution;
		if(xStart < 0)
			xStart = 0;

		int xEnd = (centerX + ChunkResolution * VoxelResolution) / VoxelResolution;
		if(xEnd >= ChunkResolution)
			xEnd = ChunkResolution - 1;

		int yStart = (centerY - ChunkResolution * VoxelResolution - 1) / VoxelResolution;
		if(yStart < 0)
			yStart = 0;

		int yEnd = (centerY + ChunkResolution * VoxelResolution) / VoxelResolution;
		if(yEnd >= ChunkResolution)
			yEnd = ChunkResolution - 1;

		NoiseStencil activeStencil = new NoiseStencil();
		activeStencil.Init(true,ChunkResolution * VoxelResolution,seed,lower,upper);

		int yOffset = yEnd * VoxelResolution;
		for(int y = yEnd; y >= yStart; y--) {
			int i = y * ChunkResolution + xEnd;
			int xOffset = xEnd * VoxelResolution;
			for(int x = xEnd; x >= xStart; x--, i--) {
				activeStencil.SetCenter(centerX - xOffset,centerY - yOffset);
				chunks[i].Apply(activeStencil);
				xOffset -= VoxelResolution;
			}
			yOffset -= VoxelResolution;
		}
	}
}
