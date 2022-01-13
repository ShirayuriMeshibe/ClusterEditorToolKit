using ShirayuriMeshibe.Search.TreeDataExtension;
using ClusterVR.CreatorKit.Gimmick.Implements;
using ClusterVR.CreatorKit.Item.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    internal abstract class SearchLogic
    {
        protected const string PropertyNameGimmickKey = "key";
        protected const string PropertyNameGimmickTarget = "target";
        protected const string PropertyNameItem = "item";

        public SearchLogic(Type type)
        {
            Assert.IsNotNull(type, "Invalid param. type is null.");
            Type = type;
            Name = type.Name;
        }

        public Type Type { get; protected set; }
        public string Name { get; protected set; }
        public bool IncludeSearch { get; set; } = true;
        public abstract SearchKind SearchKind { get; }
        public abstract void Search(TreeDataSourceBuilder dataSourceBuilder, GameObject[] rootObjects, GameObject[] prefabObjects);
        protected abstract void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataRoot, Component component);
        protected abstract void AddResultIfSpecifiedItem(TreeDataSourceBuilder dataSourceBuilder, TreeData dataRoot, Component component, SerializedProperty serializedProperty);
    }
    internal abstract class SearchLogic<TreeDataRoot, TreeDataType, TreeDataElement> : SearchLogic
        where TreeDataRoot : TreeData, new()
        where TreeDataType : TreeData, new()
        where TreeDataElement : TreeData, new()
    {
        readonly string _iconNameDataRoot;
        public SearchLogic(Type type, string iconNameDataRoot) : base(type)
        {
            _iconNameDataRoot = iconNameDataRoot;
        }
        public override void Search(TreeDataSourceBuilder dataSourceBuilder, GameObject[] rootObjects, GameObject[] prefabObjects)
        {
            var icon = EditorGUIUtility.IconContent(_iconNameDataRoot).image as Texture2D;
            var dataRoot = dataSourceBuilder.CreateOrGetRoot<TreeDataRoot>();
            dataRoot.SetIcon(icon);

            var dataType = dataSourceBuilder.GetTreeDataType<TreeDataType>(dataRoot, Type);
            dataType.Name = Name;
            dataType.SetIcon(icon);

            try
            {
                if (rootObjects != null)
                {
                    foreach (var gameObject in rootObjects)
                    {
                        var componets = gameObject.GetComponentsInChildren(Type, true);
                        foreach (var component in componets)
                            SearchEveryComponent(dataSourceBuilder, dataType, component);
                    }
                }

                if (prefabObjects != null)
                {
                    foreach (var gameObject in prefabObjects)
                    {
                        var componets = gameObject.GetComponentsInChildren(Type, true);
                        foreach (var component in componets)
                            SearchEveryComponent(dataSourceBuilder, dataType, component);
                    }
                }
            }
            catch (Exception ex)
            {
                if (dataSourceBuilder.Context.ShowDebugLog)
                {
                    Debug.LogError($"Failed search. Type:{Type.Name}");
                    Debug.LogException(ex);
                    throw ex;
                }
            }
        }
        protected override void AddResultIfSpecifiedItem(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component, SerializedProperty serializedProperty)
        {
            if (serializedProperty == null)
            {
                if (dataSourceBuilder.Context.ShowDebugLog)
                    Debug.LogError($"Invalid param . serializedProperty is null. Type:{Name}, SearchOption:{GetType().Name}");
                return;
            }

            var searchItem = dataSourceBuilder.Context.Item;
            var item = serializedProperty.objectReferenceValue as Item;

            if (item != null && item.gameObject == searchItem.gameObject && !dataSourceBuilder.RecordedObjects.Contains(component.gameObject))
            {
                var data = dataSourceBuilder.CreateAndAddChild<TreeDataElement>(dataParent);
                data.Name = Utils.GetPath(component.transform);
                data.SetObject(component);

                var icon = data as ITreeDataIcon;
                if (icon != null)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(component))
                        icon.Icon = EditorGUIUtility.IconContent(EditorIconName.PrefabModel_Icon).image as Texture2D;
                    else
                        icon.Icon = EditorGUIUtility.IconContent(EditorIconName.GameObject_Icon).image as Texture2D;
                }

                dataSourceBuilder.RecordedObjects.Add(component.gameObject);
            }
        }
    }
}
