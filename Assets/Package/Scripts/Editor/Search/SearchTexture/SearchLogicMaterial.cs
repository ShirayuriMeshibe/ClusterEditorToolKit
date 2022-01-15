using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchTexture
{
    internal class SearchLogicMaterial : SearchLogic
    {
        public SearchLogicMaterial() : base(typeof(Material)) {}

        public override void Search(TreeDataSourceBuilder dataSourceBuilder, GameObject[] sceneObjects, GameObject[] prefabObjects)
        {
            var context = dataSourceBuilder.Context;
            var materials = AssetDatabaseUtils.LoadAllAssets<Material>();
            var dataRoot = dataSourceBuilder.CreateOrGetRoot<TreeDataMaterialRoot>();
            dataRoot.Icon = EditorGUIUtility.IconContent(EditorIconName.Material_Icon).image as Texture2D;

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
                TreeDataMaterial data = null;

                foreach (var propertyName in material.GetTexturePropertyNames())
                {
                    var texture = material.GetTexture(Shader.PropertyToID(propertyName));

                    if (texture != null && context.Texture == texture)
                    {
                        var propertyIndex = shader.FindPropertyIndex(propertyName);

                        // シェーダーにはないプロパティも検索に含めるとき、または
                        // Shaderに含まれているプロパティのとき
                        if (context.SearchUnusedProperties || (0 <= propertyIndex && shader.GetPropertyType(propertyIndex) == UnityEngine.Rendering.ShaderPropertyType.Texture))
                        {
                            data = dataSourceBuilder.CreateAndAddChild<TreeDataMaterial>(dataRoot);
                            data.Name = material.name;
                            data.Object = material;
                            data.Icon = EditorIcon.GetIcon(material);

                            if (context.ShowDebugLog)
                                Debug.Log($"Material. Name:{material.name}", material);
                            break;
                        }
                    }
                }

                if(data != null)
                {
                    if (context.SearchRenderers)
                    {
                        SearchRenderer(dataSourceBuilder, rendererInScenes, material, data);
                        SearchRenderer(dataSourceBuilder, rendererInPrefabs, material, data);
                    }

                    if (context.SearchCustomRenderTextures)
                        SearchCustomRenderer(dataSourceBuilder, customRenderTextureInPrefabs, material, data);

                    // どこから参照されていないときは自分自身をカウントとして数える
                    if (data.Children.Count == 0)
                        data.Count = 1;
                }
            }
        }

        void SearchRenderer(TreeDataSourceBuilder dataSourceBuilder, Renderer[] renderers, Material material, TreeDataMaterial parent)
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
                        var data = dataSourceBuilder.CreateAndAddChild<TreeDataRenderer>(parent);
                        data.Name = Utils.GetPath(obj.transform);
                        data.Object = obj;
                        data.Icon = EditorIcon.GetIcon(obj);
                        hashSet.Add(obj);
                    }
                }
            }
        }
        void SearchCustomRenderer(TreeDataSourceBuilder dataSourceBuilder, CustomRenderTexture[] textures, Material material, TreeDataMaterial parent)
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
                    var data = dataSourceBuilder.CreateAndAddChild<TreeDataRenderer>(parent);
                    data.Name = texture.name;
                    data.Object = texture;
                    data.Icon = EditorIcon.GetIcon(texture);
                    hashSet.Add(texture);
                }
            }
        }
    }
}
