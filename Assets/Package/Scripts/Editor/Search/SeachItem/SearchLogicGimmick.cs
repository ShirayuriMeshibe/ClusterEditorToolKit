using ClusterVR.CreatorKit.Item.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    abstract internal class SearchLogicGimmick : SearchLogic<TreeDataGimmickRoot, TreeDataGimmic, TreeDataGimmicData>
    {
        protected const string PropertyNameMovableItem = "movableItem";
        protected const string PropertyNameRidableItem = "ridableItem";
        protected const string PropertyNameCharacterItem = "characterItem";
        protected const string PropertyNameGlobalGimmickKey = "globalGimmickKey";

        public SearchLogicGimmick(Type type) : base(type, EditorIconName.D_UnityEditor_Graphs_AnimatorControllerTool) { }
        public override SearchKind SearchKind => SearchKind.Gimmick;
    }

    internal class SearchLogicGimmickGlobal : SearchLogicGimmick
    {
        public SearchLogicGimmickGlobal(Type type) : base(type) {}
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedProperty = serializedObject.FindProperty(PropertyNameGlobalGimmickKey);
            var serializedPropertyGimmickKey = serializedProperty.FindPropertyRelative(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyItem = serializedProperty.FindPropertyRelative(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
        }
    }

    internal class SearchLogicGimmickLocal : SearchLogicGimmick
    {
        public SearchLogicGimmickLocal(Type type) : base(type) { }
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyGimmickKey = serializedObject.FindProperty(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyItem = serializedObject.FindProperty(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
        }
    }

    internal class SearchLogicGimmickLocal<T> : SearchLogicGimmick where T : UnityEngine.Object
    {
        public SearchLogicGimmickLocal(Type type, string propertyName) : base(type)
        {
            PropertyName = propertyName;
        }
        string PropertyName { get; set; }
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyGimmickKey = serializedObject.FindProperty(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyMovableItem = serializedObject.FindProperty(PropertyName);
            var parentItem = serializedPropertyMovableItem.objectReferenceValue as T;
            var serializedObjectParentItem = new SerializedObject(parentItem);
            var serializedPropertyItem = serializedObjectParentItem.FindProperty(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
        }
    }
    internal class SearchLogicGimmickLocalMovable : SearchLogicGimmickLocal<MovableItem>
    {
        public SearchLogicGimmickLocalMovable(Type type) : base(type, PropertyNameMovableItem) { }
    }

    internal class SearchLogicGimmickLocalRidable : SearchLogicGimmickLocal<RidableItem>
    {
        public SearchLogicGimmickLocalRidable(Type type) : base(type, PropertyNameRidableItem) { }
    }

    internal class SearchLogicGimmickLocalCharacter : SearchLogicGimmickLocal<CharacterItem>
    {
        public SearchLogicGimmickLocalCharacter(Type type) : base(type, PropertyNameCharacterItem) { }
    }

    internal class SearchLogicGimmickPlayer : SearchLogicGimmick
    {
        public SearchLogicGimmickPlayer(Type type) : base(type) { }
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyGimmickKey = serializedObject.FindProperty(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyItem = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
        }
    }

    internal class SearchLogicWarpItemGimmick : SearchLogicGimmick
    {
        public SearchLogicWarpItemGimmick(Type type) : base(type) { }
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyMovableItem = serializedObject.FindProperty(PropertyNameMovableItem);
            var movableItem = serializedPropertyMovableItem.objectReferenceValue as MovableItem;
            var serializedObjectMovableItem = new SerializedObject(movableItem);
            var serializedPropertyItem = serializedObjectMovableItem.FindProperty(PropertyNameItem);
            AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
        }
    }
}
