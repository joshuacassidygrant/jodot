namespace Jodot.Extensions;

using System.Collections.Generic;
using Godot;

public static class ListExtensions
{
    public static T PickRandom<T>(this List<T> list)
    {
        if (list.Count == 0) return default;
        RandomNumberGenerator rng = new RandomNumberGenerator();
        int idx = rng.RandiRange(0, list.Count - 1);
        return list[idx];
    }

    public static List<T> Shuffle<T>(this List<T> list)
    {
        if (list.Count <= 1) return list;
        
        RandomNumberGenerator rng = new RandomNumberGenerator();
        int n = list.Count;
        while (n > 1)
        {
            int k =  rng.RandiRange(0, --n);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }

        return list;
    }

    public static T Pop<T>(this List<T> list, int index = 0) {
        if (list.Count <= 0) return default;
        T result = list[index];
        list.RemoveAt(index);
        return result;
    } 
}

