using UnityEngine;

public class Stencil {
	protected bool fill;
	protected int centerX, centerY, radius;

	public int XStart {
		get {
			return centerX - radius;
		}
	}
	public int XEnd {
		get {
			return centerX + radius;
		}
	}
	public int YStart {
		get {
			return centerY - radius;
		}
	}
	public int YEnd {
		get {
			return centerY + radius;
		}
	}

	public virtual void Init(bool fill, int radius) {
		this.fill = fill;
		this.radius = radius;
	}

	public virtual void SetCenter (int x, int y) {
		centerX = x;
		centerY = y;
	}

	public virtual bool Apply (int x, int y, bool original) {
		return fill;
	}
}
