using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchTexture
{
    internal class SearchLogicMaterial : SearchLogic<Material>
    {
        public SearchLogicMaterial() {}

        public override void Search(TreeDataSourceBuilder dataSourceBuilder, GameObject[] sceneObjects, GameObject[] prefabObjects)
        {
            var context = dataSourceBuilder.Context;
            var materials = AssetDatabaseUtils.LoadAllAssets<Material>();
            var dataSourceRoot = dataSourceBuilder.CreateOrGetRoot<TreeDataSourceMaterialRoot>();
            dataSourceRoot.Icon = EditorGUIUtility.IconContent(EditorIconName.Material_Icon).image as Texture2D;

            Renderer[] rendererInScenes = null;
            Renderer[] rendererInPrefabs = null;
            if (context.SearchRenderers)
            {
                rendererInScenes = Utils.GetComponents<Renderer>(sceneObjects);
                rendererInPrefabs = AssetDatabaseUtils.LoadAllAssets<Renderer>();
            }

            CustomRenderTexture[] customRenderTextureInPrefabs = null;
            if (context.SearchRenderers)
                customRenderTextureInPrefabs = AssetDatabaseUtils.LoadAllAssets<CustomRenderTexture>();

            foreach (var material in materials)
            {
                if (material == null)
                    continue;

                var shader = material.shader;
                TreeDataSourceMaterial dataSourceMaterial = null;

                foreach (var propertyName in material.GetTexturePropertyNames())
                {
                    bool isInclude = false;
                    var texture = material.GetTexture(Shader.PropertyToID(propertyName));

                    if (texture != null && context.Texture == texture)
                    {
                        var propertyIndex = shader.FindPropertyIndex(propertyName);

                        // Shaderに含まれているプロパティのとき
                        if (0 <= propertyIndex && shader.GetPropertyType(propertyIndex) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                        {
                            isInclude = true;
                        }
                        // 他のシェーダのプロパティがマテリアルの設定に残っているとき
                        else
                        {
                            if (context.SearchUnusedProperties)
                                isInclude = true;
                        }

                        if(isInclude)
                        {
                            dataSourceMaterial = dataSourceBuilder.CreateAndAddChild<TreeDataSourceMaterial>(dataSourceRoot);
                            dataSourceMaterial.Name = material.name;
                            dataSourceMaterial.Object = material;
                            dataSourceMaterial.Icon = EditorIcon.GetIcon(material);

                            if (context.ShowDebugLog)
                                Debug.Log($"Material. Name:{material.name}", material);
                        }
                    }
                }

                if(dataSourceMaterial != null)
                {
                    if (context.SearchRenderers)
                    {
                        SearchRenderer(dataSourceBuilder, rendererInScenes, material, dataSourceMaterial);
                        SearchRenderer(dataSourceBuilder, rendererInPrefabs, material, dataSourceMaterial);
                    }

                    if (context.SearchCustomRenderTextures)
                        SearchCustomRenderer(dataSourceBuilder, customRenderTextureInPrefabs, material, dataSourceMaterial);

                    // どこから参照されていないときは自分自身をカウントとして数える
                    if (dataSourceMaterial.Children.Count == 0)
                        dataSourceMaterial.Count = 1;
                }
            }
        }

        void SearchRenderer(TreeDataSourceBuilder dataSourceBuilder, Renderer[] renderers, Material material, TreeDataSourceMaterial parent)
        {
            if (renderers == null)
                return;

            if (material == null)
                return;

            var hashSet = new HashSet<Renderer>();

            foreach(var obj in renderers)
            {
                foreach(var sharedMaterial in obj.sharedMaterials)
                {
                    if (sharedMaterial == null)
                        continue;

                    if(sharedMaterial == material && !hashSet.Contains(obj))
                    {
                        var dataSource = dataSourceBuilder.CreateAndAddChild<TreeDataSourceRenderer>(parent);
                        dataSource.Name = Utils.GetPath(obj.transform);
                        dataSource.Object = obj;
                        dataSource.Icon = EditorIcon.GetIcon(obj);
                        hashSet.Add(obj);
                    }
                }
            }
        }
        void SearchCustomRenderer(TreeDataSourceBuilder dataSourceBuilder, CustomRenderTexture[] textures, Material material, TreeDataSourceMaterial parent)
        {
            if (textures == null)
                return;

            if (material == null)
                return;

            var hashSet = new HashSet<CustomRenderTexture>();

            foreach (var texture in textures)
            {
                bool isAdd = false;

                if(texture.initializationSource == CustomRenderTextureInitializationSource.Material)
                {
                    if(texture.initializationMaterial==material)
                        isAdd = true;
                }

                if (texture.material == material)
                    isAdd = true;

                if (isAdd)
                {
                    var dataSource = dataSourceBuilder.CreateAndAddChild<TreeDataSourceRenderer>(parent);
                    dataSource.Name = texture.name;
                    dataSource.Object = texture;
                    dataSource.Icon = EditorIcon.GetIcon(texture);
                    hashSet.Add(texture);
                }
            }
        }
    }
}
