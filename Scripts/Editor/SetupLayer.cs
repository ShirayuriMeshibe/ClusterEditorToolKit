using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe
{
    public sealed class SetupLayer
    {
        /// <summary>
        /// https://note.com/hobione/n/n97c1871264b9
        /// </summary>
        static readonly Dictionary<int, string> LAYER_NAMES = new Dictionary<int, string>()
        {
            {8,  "Hand"},
            {9,  "FIRSTPERSON_ONLY_LAYER"},
            {10, "THIRDPERSON_ONLY_LAYER"},
            {14, "InteractableItem"},
            {16, "OwnAvatar"},
            {17, "GrabbableUI"},
            {18, "GrabbingItem"},
            {21, "PostProcessing"},
            {25, "Keyboard"},
            {26, "UIPointer"},
            {30, "Nameplate"},
        };

        [MenuItem(EditorDefine.MenuNameRoot + "Setup Layer")]
        static void OnSetupLayer()
        {
            Setup();
        }

        static void Setup()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if(assets==null || assets.Length==0)
            {
                Debug.LogError("SetupLayer.Setup() Not found TagManager.asset.");
                return;
            }

            var serializedObject = new SerializedObject(assets[0]);
            var layers = serializedObject.FindProperty("layers");

            if(layers==null)
            {
                Debug.LogError("SetupLayer.Setup() Not found property(layers).");
                return;
            }

            for (int i = 0; i < layers.arraySize; i++)
            {
                Debug.Log($"layers[{i}]:{layers.GetArrayElementAtIndex(i).stringValue}");

                if (LAYER_NAMES.TryGetValue(i, out string layerName))
                {
                    if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                        layers.GetArrayElementAtIndex(i).stringValue = layerName;
                }
                //else
                //    Debug.LogError($"SetupLayer.Setup() layer name not found. i:{i}");
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}
