using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe
{
    public static class Utils
    {
        static public string GetPath(Transform transform)
        {
            if (transform == null)
                return string.Empty;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat($"/{transform.name}");
            GetPathRecursive(transform, stringBuilder);

            if (0 < stringBuilder.Length)
                stringBuilder.Remove(0, 1);

            return stringBuilder.ToString();
        }

        static public void GetPathRecursive(Transform transform, StringBuilder stringBuilder)
        {
            if (transform.parent == null)
                return;
            stringBuilder.Insert(0, string.Format($"/{transform.parent.name}"));
            GetPathRecursive(transform.parent, stringBuilder);
        }
    }
}
