using UnityEngine;
using System;
using System.Collections.Generic;

namespace Bounce
{
    /// <summary>
    /// Provides useful extension methods for inbuilt objects.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds random offset to vector2.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="xOffset">xOffset.</param>
        /// <param name="yOffset">yOffset.</param>
        /// <returns></returns>
        public static Vector2 WithRandomOffset(this Vector2 start, float xOffset, float yOffset)
        {
            return new Vector2(start.x + UnityEngine.Random.Range(-xOffset, xOffset), start.y + UnityEngine.Random.Range(-yOffset, yOffset));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns>true if list is null or empty.</returns>
        public static bool IsNullOrEmpty(this Array list)
        {
            return list == null || list.Length == 0;
        }

        private static System.Random rng = new System.Random();

        /// <summary>
        /// Shuffle the specified list in a pseudo random manner.
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Shuffle<T>(this T[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>true if list is null or empty.</returns>
        public static bool IsNullOrEmpty<T>(this T[] list)
        {
            return list == null || list.Length == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns>returns true if string is null or contains only whitespace.</returns>
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return System.String.IsNullOrEmpty(s) || s.Trim().Length == 0;
        }

        /// <summary>
        /// Used to determine if a line is a comment line.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsComment(this string s)
        {
            var str = s.TrimStart();
            if (str.Length < 2)
            {
                return false;
            }

            return str[0].Equals('/') && str[1].Equals('/');
        }

        /// <summary>
        /// Provides for each enumeration on generic IEnumerable T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">The action to perform on each object in list.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                return;
            }

            foreach (T element in source)
            {
                action(element);
            }
        }

    }
}
