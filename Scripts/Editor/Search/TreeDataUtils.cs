using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search
{
    internal static class TreeDataUtils
    {
        static public void Selection(TreeData treeData, bool showDebugLog = false)
        {
            if (treeData == null)
                return;

            var treeDataObject = treeData as ITreeDataObject;

            if (treeDataObject == null)
                return;

            var obj = treeDataObject.Object;

            if (obj == null)
                return;

            if (!PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                UnityEditor.Selection.activeObject = obj;
                return;
            }

            var nearest = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
            if (nearest != null)
            {
                UnityEditor.Selection.activeObject = obj;
                return;
            }

            GameObject gameObject = null;
            var component = obj as Component;

            if (component != null)
                gameObject = component.gameObject;

            if (gameObject == null)
                gameObject = obj as GameObject;

            Assert.IsNotNull(gameObject, $"Faild cast. {obj.GetType().Name}");
            if (gameObject != null)
            {
                if (showDebugLog)
                    Debug.Log($"PingObject:{gameObject.transform.GetRoot()}");
                EditorGUIUtility.PingObject(gameObject.transform.GetRoot());
            }
        }
    }
}
