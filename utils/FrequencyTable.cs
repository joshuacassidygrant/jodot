using Godot;
using System;
using System.Linq;

public partial class FrequencyTable<T>
{
	public Entry<T>[] Entries;
	public int FrequencyTotal;

	public RandomNumberGenerator Random = new RandomNumberGenerator();

	public FrequencyTable(Entry<T>[] entries) {
		Entries = entries;
		FrequencyTotal = entries.Select(e => e.Weight).Sum();
	}

	public T PickRandom(int reduceBy = 0) {
		int target = Random.RandiRange(0, FrequencyTotal);

		for (int index = 0; index <= FrequencyTotal; index++) {
			if (target < Entries[index].Weight) {
				Entries[index].Weight = Mathf.Max(0, Entries[index].Weight - reduceBy);
				return Entries[index].Value;
			} else {
				target -= Entries[index].Weight;
			}
		}
		throw new Exception("Failed out of frequency table");
	}

	public class Entry<T> {
		public int Weight;
		public T Value;

		public Entry(int weight, T value) {
			Weight = weight;
			Value = value;
		}
	}

}
