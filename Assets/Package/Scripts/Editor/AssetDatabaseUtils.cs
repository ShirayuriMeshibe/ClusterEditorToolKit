using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe
{
    internal static class AssetDatabaseUtils
    {
        static public T[] LoadAllAssets<T>() where T : UnityObject
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var list = new List<T>(guids.Length);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null)
                    continue;

                list.Add(asset);
            }
            return list.ToArray();
        }
    }
}
