using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
namespace CogSim
{
    public static class Vector3IntExtensions
    {
        // Convert Vector3Int to Vector3
        public static Vector3 ToVector3(this Vector3Int vector3Int)
        {
            return new Vector3(vector3Int.x, vector3Int.y, vector3Int.z);
        }
        public static Vector3 ToVector3Shifted(this Vector3Int vector3Int)
        {
            return new Vector3(vector3Int.x + 0.5f, vector3Int.y, vector3Int.z + 0.5f);
        }
        // Convert Vector3 to Vector3Int (using rounding)
        public static Vector3Int ToVector3Int(this Vector3 vector3)
        {
            return new Vector3Int(
                Mathf.RoundToInt(vector3.x),
                Mathf.RoundToInt(vector3.y),
                Mathf.RoundToInt(vector3.z)
            );
        }
        public static Vector3 ToVector3Shifted(this Vector3 vector3)
        {
            return new Vector3(vector3.x + 0.5f, vector3.y, vector3.x + 0.5f);

        }
        // Optional: Add other utility methods for Vector3Int
        public static float DistanceTo(this Vector3Int a, Vector3Int b)
        {
            return Vector3.Distance(a.ToVector3(), b.ToVector3());
        }

        public static Vector3Int Sum(this Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3Int[] GetNeighbors(this Vector3Int vector3Int)
        {
            return new Vector3Int[]
            {
        vector3Int + new Vector3Int(1, 0, 0),
        vector3Int + new Vector3Int(-1, 0, 0),
        vector3Int + new Vector3Int(0, 1, 0),
        vector3Int + new Vector3Int(0, -1, 0),
        vector3Int + new Vector3Int(0, 0, 1),
        vector3Int + new Vector3Int(0, 0, -1)
            };
        }
        public static T RandomElement<T>(this List<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            IList<T> list = source as IList<T>;
            if (list == null)
            {
                list = source;
            }
            if (list.Count == 0)
            {
                Debug.Log("ERROR: Getting random element from empty collection.");
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            IList<T> list = source as IList<T>;
            if (list == null)
            {
                list = source.ToList<T>();
            }
            if (list.Count == 0)
            {
                Debug.Log("ERROR: Getting random element from empty collection.");
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}