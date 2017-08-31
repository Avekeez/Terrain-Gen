using System;
using UnityEngine;

public class LayeredTerrain : MonoBehaviour {
	private TerrainMap[] layers;

	private void Start () {
		layers = new TerrainMap[transform.childCount];
		for (int i = 0; i < transform.childCount; i ++) {
			layers[i] = transform.GetChild(i).GetComponent<TerrainMap>();
		}

		int seed = DateTime.Now.GetHashCode();

		layers[0].Init(seed,-0.5f,0.5f);
		layers[1].Init(seed,0.5f,0.7f);
		layers[2].Init(seed,0.7f,0.9f);
		layers[3].Init(seed,0.9f,1.5f);
	}
}
