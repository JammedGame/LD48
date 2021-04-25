using System;
using System.Linq;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;

public static class RandomnessProvider
{
    private static Random random = new Random();

    public static List<T> Shuffle<T>(List<T> list)
    {
        return list.OrderBy(a => Guid.NewGuid()).ToList();
    }

    public static float GetFloat()
    {
        return UnityRandom.Range(0f, 1f);
    }
}