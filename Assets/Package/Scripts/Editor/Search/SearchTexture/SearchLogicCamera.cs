using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchTexture
{
    internal class SearchLogicCamera : SearchLogic
    {
        public SearchLogicCamera(): base(typeof(Camera)) { }
        public override void Search(TreeDataSourceBuilder dataSourceBuilder, GameObject[] sceneObjects, GameObject[] prefabObjects)
        {
            SearchObjects(dataSourceBuilder, sceneObjects, EditorIconName.GameObject_On_Icon);
            SearchObjects(dataSourceBuilder, prefabObjects, EditorIconName.PrefabModel_Icon);
        }

        void SearchObjects(TreeDataSourceBuilder dataSourceBuilder, GameObject[] gameObjects, string iconName)
        {
            if (gameObjects == null)
                return;

            var dataRoot = dataSourceBuilder.CreateOrGetRoot<TreeDataCameraRoot>();
            dataRoot.Icon = EditorGUIUtility.IconContent(EditorIconName.Camera_Icon).image as Texture2D;
            var searchTexture = dataSourceBuilder.Context.Texture;

            foreach (var gameObject in gameObjects)
            {
                var cameras = gameObject.GetComponentsInChildren<Camera>(true);

                foreach (var camera in cameras)
                {
                    var texture = camera.targetTexture;

                    if (texture != null && texture == searchTexture)
                    {
                        var data = dataSourceBuilder.CreateAndAddChild<TreeDataCamera>(dataRoot);
                        data.Name = Utils.GetPath(camera.transform);
                        data.Object = camera;
                        data.Icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;

                        if (dataSourceBuilder.Context.ShowDebugLog)
                            Debug.Log($"Camera. GameObject Path:{Utils.GetPath(camera.transform)}", camera);
                    }
                }
            }
        }
    }
}
