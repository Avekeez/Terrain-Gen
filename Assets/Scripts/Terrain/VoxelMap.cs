using UnityEngine;

public class VoxelMap : MonoBehaviour {
	public float Size = 2f;
	public int VoxelResolution = 8; //height/width of chunk in voxels
	public int ChunkResolution = 2; //height/width of map in chunks

	public VoxelGrid GridPrefab;

	protected VoxelGrid[] chunks;
	protected float chunkSize, voxelSize, halfSize;

	protected virtual void Awake () {
		halfSize = Size * 0.5f;
		chunkSize = Size / ChunkResolution;
		voxelSize = chunkSize / VoxelResolution;

		chunks = new VoxelGrid[ChunkResolution * ChunkResolution];
		for (int i = 0, y = 0; y < ChunkResolution; y++) {
			for (int x = 0; x < ChunkResolution; x ++, i++) {
				CreateChunk(i,x,y);
			}
		}

		//BoxCollider box = gameObject.AddComponent<BoxCollider>();
		//box.size = new Vector3(Size,Size);
	}

	protected virtual void Update () {
		if (Input.GetMouseButton(0)) {
			RaycastHit hit;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
				if (hit.collider.gameObject == gameObject) {
					EditVoxels(transform.InverseTransformPoint(hit.point));
				}
			}
		}
	}

	protected void CreateChunk (int index, int x, int y) {
		VoxelGrid c = Instantiate(GridPrefab,transform,false);
		c.Init(VoxelResolution,chunkSize);
		c.transform.localPosition = new Vector3(x * chunkSize - halfSize,y * chunkSize - halfSize);
		chunks[index] = c;

		if (x > 0) {
			chunks[index - 1].xNeighbor = c;
		}
		if (y > 0) {
			chunks[index - ChunkResolution].yNeighbor = c;
			if (x > 0) {
				chunks[index - ChunkResolution - 1].xyNeighbor = c;
			}
		}
	}
	protected int fillIndex, radiusIndex;
	protected virtual void OnGUI() {
		GUILayout.BeginArea(new Rect(45,45,150f,500f));
		fillIndex = GUILayout.SelectionGrid(fillIndex,new[] { "fill","empty" },2);
		GUILayout.Label("Radius");
		radiusIndex = GUILayout.SelectionGrid(radiusIndex,new[] { "0","1","2","3","4","5" },6);
		GUILayout.EndArea();
	}

	protected virtual void EditVoxels (Vector3 pos) {
		int centerX = (int)((pos.x + halfSize) / voxelSize);
		int centerY = (int)((pos.y + halfSize) / voxelSize);

		int xStart = (centerX - radiusIndex - 1) / VoxelResolution;
		if(xStart < 0)
			xStart = 0;

		int xEnd = (centerX + radiusIndex) / VoxelResolution;
		if(xEnd >= ChunkResolution)
			xEnd = ChunkResolution - 1;

		int yStart = (centerY - radiusIndex - 1) / VoxelResolution;
		if(yStart < 0)
			yStart = 0;

		int yEnd = (centerY + radiusIndex) / VoxelResolution;
		if(yEnd >= ChunkResolution)
			yEnd = ChunkResolution - 1;

		Stencil activeStencil = new Stencil();
		activeStencil.Init(fillIndex == 0, radiusIndex);

		int yOffset = yEnd * VoxelResolution;
		for (int y = yEnd; y >= yStart; y--) {
			int i = y * ChunkResolution + xEnd;
			int xOffset = xEnd * VoxelResolution;
			for (int x = xEnd; x >= xStart; x--, i--) {
				activeStencil.SetCenter(centerX - xOffset,centerY - yOffset);
				chunks[i].Apply(activeStencil);
				xOffset -= VoxelResolution;
			}
			yOffset -= VoxelResolution;
		}
	}
}
