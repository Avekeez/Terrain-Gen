using UnityEngine;
using System;

public class NoiseStencil : Stencil {
	System.Random random;
	protected int seed;
	protected float lower;
	protected float upper;

	public void Init(bool fill,int radius, int seed, float lower, float upper) {
		base.Init(fill,radius);
		this.seed = seed;
		this.lower = lower;
		this.upper = upper;
	}

	public override bool Apply(int x,int y,bool original) {
		random = new System.Random(seed);
		x -= centerX;
		y -= centerY;
		float xOffset = (float)random.NextDouble() * 100000;
		float yOffset = (float)random.NextDouble() * 100000;

		float result = Mathf.PerlinNoise(x / 16f + xOffset,y / 16f + yOffset);

		return lower <= result && result < upper;
    }
}
