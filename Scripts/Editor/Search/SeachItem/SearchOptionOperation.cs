using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchItem
{
    abstract public class SearchOptionOperation : SearchOption
    {
        protected const string PropertyNameGlobalGimmickKey = "globalGimmickKey";

        public SearchOptionOperation(Type type) : base(type) { }
        public override SearchKind SearchKind => SearchKind.Operation;
    }

    public class SearchOptionOperationGlobal : SearchOptionOperation
    {
        public SearchOptionOperationGlobal(Type type) : base(type) { }
        override protected int SearchEveryComponent(Context context, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedProperty = serializedObject.FindProperty(PropertyNameGlobalGimmickKey);
            var serializedPropertyGimmickKey = serializedProperty.FindPropertyRelative(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyItem = serializedProperty.FindPropertyRelative(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            return context.StartId;
        }
    }

    public class SearchOptionOperationLocal : SearchOptionOperation
    {
        public SearchOptionOperationLocal(Type type) : base(type) { }
        override protected int SearchEveryComponent(Context context, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyGimmickKey = serializedObject.FindProperty(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyItem = serializedObject.FindProperty(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            return context.StartId;
        }
    }

    public class SearchOptionOperationPlayer : SearchOptionOperation
    {
        public SearchOptionOperationPlayer(Type type) : base(type) { }
        override protected int SearchEveryComponent(Context context, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyGimmickKey = serializedObject.FindProperty(PropertyNameGimmickKey);
            var serializedPropertyGimmickTarget = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameGimmickTarget);
            var serializedPropertyItem = serializedPropertyGimmickKey.FindPropertyRelative(PropertyNameItem);

            // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Gimmick/GimmickTarget.cs
            // GimmickTarget.Itemのとき
            if (serializedPropertyGimmickTarget.enumValueIndex == 0)
                context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            return context.StartId;
        }
    }
}
