using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Voxel.MathUtil
{
    public static class Util
    {
        public static bool HasFlag(this Enum e, Enum flag) 
        {
            var a = Convert.ToInt64(e);
            var b = Convert.ToInt64(flag);

            return (a & b) == b;
        }

        public static string ToMinMaxString(this Bounds b)
        {
            return string.Format("Min: {0} Max: {1}", b.min, b.max);
        }

        public static Vector3 Abs(this Vector3 vec)
        {
            return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
        }

        public static T MostCommon<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First();
        }

        public static T MostCommon<T>(this IEnumerable<T> list, Func<T, T> selector)
        {
            return list.GroupBy(selector).OrderByDescending(x => x.Count()).Select(x => x.Key).First();
        }
    }
}
