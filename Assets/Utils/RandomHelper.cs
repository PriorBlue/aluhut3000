using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 219

public static class RandomHelper {
	private static Random r = new Random();
	
	// min/max is included
	public static int Next(int min, int max)
	{
		return min + r.Next() % (Math.Max(max, min) - min + 1);
	}

	// [min,max[
	public static float Next(float min, float max) 
	{
		return min + Next () * (max - min);
	}
	
	// [0-1[
	public static float Next()
	{
		return (float)r.NextDouble();	
	}

	/// <summary>
	/// Fisher–Yates shuffle
	/// </summary>
	/// <remarks>
	/// https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
	/// </remarks>
	public static void Shuffle<T>(T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			int j = Next(i, array.Length - 1);
			T tmp = array[j];
			array[j] = array[i];
			array[i] = tmp;
		}
	}
	
	/// <summary>
	/// Pick randomly from weighted elements
	/// </summary>
	/// <returns>
	/// A element
	/// </returns>
	/// <param name='itemWeightMap'>
	/// Item weight map, key = items, value = weight (must be int > 0)
	/// </param>
	/// <typeparam name='T'>
	/// Item type
	/// </typeparam>
	public static T PickWeightedRandom<T>(Dictionary<T,int> itemWeightMap)
	{
		int weightSum = 0;
		
		foreach(var pair in itemWeightMap)
		{
			weightSum += pair.Value;
		}
		
		int random = RandomHelper.Next(0, weightSum - 1);
		
		foreach(var pair in itemWeightMap)
		{
			int weight = pair.Value;
			if (random < weight)
			{
				return pair.Key;
			}
			
			random -= weight;
		}
		
		throw new Exception("this will not happen");
	}
	
	public static T PickRandom<T>(List<T> items)
	{
		if (items.Count == 0)throw new Exception("you cant pick from an empty list");
		
		int index = Next(0, items.Count - 1);
		return items[index];
	}
	
	public static T PickRandom<T>(IEnumerable<T> items)
	{
		int count = 0;
	
		foreach (T i in items) ++count;

		if (count == 0)throw new Exception("you cant pick from an empty list");
		
		int index = Next(0, count - 1);
		foreach (T i in items) {
			if (index == 0) return i;
			--index;
		}

		throw new Exception("pick random is broken");
	}

	public static T PickWeightedRandom<T>(IEnumerable<T> items, Func<T, int> weightFun)
	{
		var weightMap = new Dictionary<T, int>();
		
		foreach(var item in items)
		{
			weightMap[item] = weightFun(item);
		}
		
		return PickWeightedRandom(weightMap);
	}

    public static string String(int len)
    {
        StringBuilder b = new StringBuilder();
        for (int i = 0; i < len; ++i )
        {
            b.Append(Convert.ToChar(Next((int)'0', (int)'9')));
        }
        return b.ToString();
    }
}
