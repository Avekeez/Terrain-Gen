using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour {
	public int Resolution = 8;
	public GameObject VoxelPrefab;
	public bool drawVoxels = false;

	public Voxel[] Voxels;

	public VoxelGridSurface surfacePrefab;
	private VoxelGridSurface surface;

	public VoxelGridWall wallPrefab;
	private VoxelGridWall wall;

	private float voxelSize, gridSize;
	private Voxel dummyX, dummyY, dummyT;

	public VoxelGrid xNeighbor, yNeighbor, xyNeighbor;

	public void Init (int Resolution, float Size) {
		this.Resolution = Resolution;
		voxelSize = Size / Resolution;
		gridSize = Size;
		Voxels = new Voxel[Resolution * Resolution];
		dummyX = new Voxel();
		dummyY = new Voxel();
		dummyT = new Voxel();

		for (int i = 0, y = 0; y < Resolution; y++) {
			for (int x = 0; x < Resolution; x++, i++) {
				createVoxel(i,x,y);
			}
		}

		surface = Instantiate(surfacePrefab,transform,false);
		surface.transform.localPosition = Vector3.zero;
		surface.Init(Resolution);

		wall = Instantiate(wallPrefab,transform,false);
		wall.transform.localPosition = Vector3.zero;
		wall.Init(Resolution);

		refresh();
	}

	private void createVoxel (int index, int x, int y) {
		if (drawVoxels) {
			GameObject v = Instantiate(VoxelPrefab,transform,false);
			v.transform.localPosition = new Vector3((x + 0.5f) * voxelSize,(y + 0.5f) * voxelSize, -0.01f);
			v.transform.localScale = Vector3.one * voxelSize * 0.1f;
		}
		Voxels[index] = new Voxel(x,y,voxelSize);
	}

	public void Apply (Stencil stencil) {
		int xStart = stencil.XStart;
		if(xStart < 0)
			xStart = 0;

		int xEnd = stencil.XEnd;
		if(xEnd >= Resolution)
			xEnd = Resolution - 1;

		int yStart = stencil.YStart;
		if(yStart < 0)
			yStart = 0;

		int yEnd = stencil.YEnd;
		if(yEnd >= Resolution)
			yEnd = Resolution - 1;

		for (int y = yStart; y <= yEnd; y++) {
			int i = y * Resolution + xStart;
			for (int x = xStart; x <= xEnd; x++, i++) {
				Voxels[i].State = stencil.Apply(x,y,Voxels[i].State);
			}
		}
		refresh();
	}

	public void Apply (int x, int y, bool active) {
		Voxels[y * Resolution + x].State = active;
		refresh();
	}

	private void refresh () {
		triangulate();
	}

	private void cacheFirstCorner(Voxel voxel) {
		if (voxel.State) {
			surface.cacheFirstCorner(voxel);
		}
	}

	private void cacheNextEdgeAndCorner(int i,Voxel xMin,Voxel xMax) {
		if(xMin.State != xMax.State) {
			surface.cacheXEdge(i,xMin);
			wall.cacheXEdge(i,xMin);
		}
		if(xMax.State) {
			surface.cacheNextCorner(i,xMax);
		}
	}
	private void cacheNextMiddleEdge(Voxel yMin,Voxel yMax) {
		surface.prepareCacheForNextCell();
		wall.prepareCacheForNextCell();
		if (yMin.State != yMax.State) {
			surface.cacheYEdge(yMin);
			wall.cacheYEdge(yMin);
		}
	}

	private void swapRowCaches() {
		surface.prepareCacheForNextRow();
		wall.prepareCacheForNextRow();
	}

	private void triangulate() {
		surface.Clear();
		wall.Clear();
		if (xNeighbor != null) {
			dummyX.BecomeXDummyOf(xNeighbor.Voxels[0],gridSize);
		}
		fillFirstRowCache();
		triangulateCellRows();
		if (yNeighbor != null) {
			triangulateGapRow();
		}
		surface.Apply();
		wall.Apply();
    }

	private void fillFirstRowCache() {
		cacheFirstCorner(Voxels[0]);
		int i = 0;
		for (i = 0; i < Resolution - 1; i++) {
			cacheNextEdgeAndCorner(i,Voxels[i],Voxels[i + 1]);
		}
		if (xNeighbor != null) {
			dummyX.BecomeXDummyOf(xNeighbor.Voxels[0],gridSize);
			cacheNextEdgeAndCorner(i,Voxels[i],dummyX);
		}
	}

	private void triangulateCellRows() {
		int cells = Resolution - 1;

		for (int i = 0, y = 0; y < cells; y++, i++) {
			swapRowCaches();
			cacheFirstCorner(Voxels[i + Resolution]);
			cacheNextMiddleEdge(Voxels[i],Voxels[i + Resolution]);
			for (int x = 0; x < cells; x++, i++) {
				Voxel
					a = Voxels[i],
					b = Voxels[i + 1],
					c = Voxels[i + Resolution],
					d = Voxels[i + Resolution + 1];
				cacheNextEdgeAndCorner(x,c,d);
				cacheNextMiddleEdge(b,d);
				triangulateCell(x,a,b,c,d);
			}
			if (xNeighbor != null) {
				triangulateGapCell(i);
			}
		}
	}
	private void triangulateGapCell(int i) {
		Voxel dummySwap = dummyT;
		dummySwap.BecomeXDummyOf(xNeighbor.Voxels[i + 1],gridSize);
		dummyT = dummyX;
		dummyX = dummySwap;

		int cacheIndex = Resolution - 1;
        cacheNextEdgeAndCorner(cacheIndex,Voxels[i + Resolution],dummyX);
		cacheNextMiddleEdge(dummyT,dummyX);
		triangulateCell(cacheIndex,Voxels[i],dummyT,Voxels[i + Resolution],dummyX);
	}
	private void triangulateGapRow() {
		dummyY.BecomeYDummyOf(yNeighbor.Voxels[0],gridSize);
		int cells = Resolution - 1;
		int offset = cells * Resolution;

		swapRowCaches();
		cacheFirstCorner(dummyY);
		cacheNextMiddleEdge(Voxels[cells * Resolution],dummyY);

		for (int x = 0; x < cells; x ++) {
			Voxel dummySwap = dummyT;
			dummySwap.BecomeYDummyOf(yNeighbor.Voxels[x + 1],gridSize);
			dummyT = dummyY;
			dummyY = dummySwap;
			
			cacheNextEdgeAndCorner(x,dummyT,dummyY);
			cacheNextMiddleEdge(Voxels[x + offset + 1],dummyY);
			triangulateCell(x,Voxels[x + offset],Voxels[x + offset + 1],dummyT,dummyY);
		}

		if (xNeighbor != null) {
			dummyT.BecomeXYDummyOf(xyNeighbor.Voxels[0],gridSize);
            cacheNextEdgeAndCorner(cells,dummyY,dummyT);
			cacheNextMiddleEdge(dummyX,dummyT);
			triangulateCell(cells,Voxels[Voxels.Length - 1],dummyX,dummyY,dummyT);
		}
	}

	/*
		c(4)    d(8)


		a(1)    b(2)   
	*/
	private void triangulateCell(int i, Voxel a, Voxel b, Voxel c, Voxel d) {
		int cellType = 0;
		if(a.State)
			cellType |= 1;
		if(b.State)
			cellType |= 2;
		if(c.State)
			cellType |= 4;
		if(d.State)
			cellType |= 8;

		/*
		* Disgusting.
		* Abandon hope all me who enter here.
		* It works, don't screw with it mate
		*/

		switch(cellType) {
			case 0:
				return;
			//---------------------------------------------------------------
			case 1: //bottom left
				surface.addTriangle(surface.cornersMin[i],surface.yEdgeMin,surface.xEdgesMax[i]);
				wall.AddACAB(i);
				break;
			case 2: // bottom right
				surface.addTriangle(surface.cornersMin[i + 1],surface.xEdgesMax[i],surface.yEdgeMax);
				wall.AddABBD(i);
				break;
			case 4: // top left
				surface.addTriangle(surface.cornersMax[i],surface.xEdgesMin[i],surface.yEdgeMin);
				wall.AddCDAC(i);
				break;
			case 8: // top right
				surface.addTriangle(surface.cornersMax[i + 1],surface.yEdgeMax,surface.xEdgesMin[i]);
				wall.AddBDCD(i);
				break;
			//---------------------------------------------------------------
			case 3: // bottom half
				surface.addQuad(surface.cornersMin[i],surface.yEdgeMin,surface.yEdgeMax,surface.cornersMin[i + 1]);
				wall.AddACBD(i);
				break;
			case 5: // left half
				surface.addQuad(surface.cornersMin[i],surface.cornersMax[i],surface.xEdgesMin[i],surface.xEdgesMax[i]);
				wall.AddCDAB(i);
				break;
			case 10: // right half
				surface.addQuad(surface.xEdgesMax[i],surface.xEdgesMin[i],surface.cornersMax[i + 1],surface.cornersMin[i + 1]);
				wall.AddABCD(i);
				break;
			case 12: // top half
				surface.addQuad(surface.yEdgeMin,surface.cornersMax[i],surface.cornersMax[i + 1],surface.yEdgeMax);
				wall.AddBDAC(i);
				break;
			case 15: // full
				surface.addQuad(surface.cornersMin[i],surface.cornersMax[i],surface.cornersMax[i + 1],surface.cornersMin[i + 1]);
				break;
			//---------------------------------------------------------------
			case 7: // top right missing
				surface.addPentagon(surface.cornersMin[i],surface.cornersMax[i],surface.xEdgesMin[i],surface.yEdgeMax,surface.cornersMin[i + 1]);
				wall.AddCDBD(i);
				break;
			case 11: // top left missing
				surface.addPentagon(surface.cornersMin[i + 1],surface.cornersMin[i],surface.yEdgeMin,surface.xEdgesMin[i],surface.cornersMax[i + 1]);
				wall.AddACCD(i);
				break;
			case 13: // bottom right missing
				surface.addPentagon(surface.cornersMax[i],surface.cornersMax[i + 1],surface.yEdgeMax,surface.xEdgesMax[i],surface.cornersMin[i]);
				wall.AddBDAB(i);
				break;
			case 14: // bottom left missing
				surface.addPentagon(surface.cornersMax[i + 1],surface.cornersMin[i + 1],surface.xEdgesMax[i],surface.yEdgeMin,surface.cornersMax[i]);
				wall.AddABAC(i);
				break;
			//---------------------------------------------------------------
			case 6: // top left, bottom right
				surface.addTriangle(surface.cornersMin[i + 1],surface.xEdgesMax[i],surface.yEdgeMax);
				wall.AddABBD(i);
				surface.addTriangle(surface.cornersMax[i],surface.xEdgesMin[i],surface.yEdgeMin);
				wall.AddCDAC(i);
				break;
			case 9: // top right, bottom left
				surface.addTriangle(surface.cornersMin[i],surface.yEdgeMin,surface.xEdgesMax[i]);
				wall.AddACAB(i);
				surface.addTriangle(surface.cornersMax[i + 1],surface.yEdgeMax,surface.xEdgesMin[i]);
				wall.AddBDCD(i);
				break;
		}
	}

	
}
