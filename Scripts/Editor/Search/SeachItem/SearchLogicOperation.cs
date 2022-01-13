using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    abstract internal class SearchLogicOperation : SearchLogic<TreeDataOperationRoot, TreeDataOperation, TreeDataOperationData>
    {
        protected const string PropertyNameGlobalGimmickKey = "globalGimmickKey";

        public SearchLogicOperation(Type type) : base(type, EditorIconName.PreAudioAutoPlayOff) { }
        public override SearchKind SearchKind => SearchKind.Operation;
    }

    internal class SearchLogicOperationGlobal : SearchLogicOperation
    {
        public SearchLogicOperationGlobal(Type type) : base(type) { }
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

    internal class SearchLogicOperationLocal : SearchLogicOperation
    {
        public SearchLogicOperationLocal(Type type) : base(type) { }
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

    internal class SearchLogicOperationPlayer : SearchLogicOperation
    {
        public SearchLogicOperationPlayer(Type type) : base(type) { }
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
}
