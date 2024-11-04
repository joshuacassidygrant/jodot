using Godot;
using System;
using System.Collections.Generic;

public partial class SpiralIterator
{
	Vector2I Origin;
	int Max;

	private int layer = 1;
	private int leg;
	private int x;
	private int y;
	private int total;

	public SpiralIterator(Vector2I start, int max) {
		Origin = start;
		Max = max;
	}
	
	private Vector2I Iterate() {
		switch (leg) {
			case 0:
				x++;
				total++;
				if (x == layer) {
					leg++;
				}
				break;
			case 1:
				y++;
				total++;
				if (y == layer) {
					leg++;
				}
				break;
			case 2:
				x--;
				total++;
				if (-x == layer) {
					leg++;
				}
				break;
			case 3:
				y--;
				total++;
				if (-y == layer) {
					leg=0;
					layer++;
				}
				break;
		}
		return CurrentLocation();
	}

	public Vector2I CurrentLocation() {
		return new Vector2I(x, y) + Origin;
	}

	public List<Vector2I> GetClosest(Func<Vector2I, bool> predicate, int count = 1) {
		List<Vector2I> found = new();
		if (predicate(Origin)) {
			found.Add(Origin);
		}

		while (found.Count < count && total < Max) {
			Vector2I next = Iterate();
			if (predicate(next)) {
				found.Add(next);
			}
		}
		return found;
	}
}
