using System.Collections.Generic;

namespace Utils
{
    public static class RandomUtils
    {
        public static T RandomEntry<T>(this IReadOnlyList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}