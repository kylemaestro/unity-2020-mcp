// BCL members missing from Unity 2020.3's .NET Standard 2.0 profile, provided as
// extensions so call sites written against .NET Standard 2.1 compile unchanged.
using System;
using System.Collections.Generic;

namespace MCPForUnity.Editor.Helpers.Compat
{
    public static class NetStandard20Compat
    {
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) >= 0;
        }

        public static bool Contains(this string s, char value)
        {
            return s.IndexOf(value) >= 0;
        }

        public static bool StartsWith(this string s, char value)
        {
            return s.Length > 0 && s[0] == value;
        }

        public static bool EndsWith(this string s, char value)
        {
            return s.Length > 0 && s[s.Length - 1] == value;
        }

        public static string[] Split(this string s, char separator, StringSplitOptions options)
        {
            return s.Split(new[] { separator }, options);
        }

        public static string[] Split(this string s, string separator, StringSplitOptions options)
        {
            return s.Split(new[] { separator }, options);
        }

        public static string Replace(this string s, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(oldValue)) throw new ArgumentException("oldValue must be non-empty", nameof(oldValue));
            var result = new System.Text.StringBuilder();
            int start = 0;
            while (true)
            {
                int idx = s.IndexOf(oldValue, start, comparison);
                if (idx < 0) break;
                result.Append(s, start, idx - start);
                result.Append(newValue);
                start = idx + oldValue.Length;
            }
            result.Append(s, start, s.Length - start);
            return result.ToString();
        }

        public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value)
        {
            if (dict.TryGetValue(key, out value))
            {
                dict.Remove(key);
                return true;
            }
            value = default;
            return false;
        }
    }

    public static class MathCompat
    {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }
    }

    public static class PathCompat
    {
        public static string GetRelativePath(string relativeTo, string path)
        {
            var fromUri = new Uri(AppendSeparator(System.IO.Path.GetFullPath(relativeTo)));
            var toUri = new Uri(System.IO.Path.GetFullPath(path));
            if (fromUri.Scheme != toUri.Scheme) return path;
            string rel = Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString())
                .Replace('/', System.IO.Path.DirectorySeparatorChar);
            return string.IsNullOrEmpty(rel) ? "." : rel;
        }

        private static string AppendSeparator(string p)
        {
            return p.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? p : p + System.IO.Path.DirectorySeparatorChar;
        }
    }
}
