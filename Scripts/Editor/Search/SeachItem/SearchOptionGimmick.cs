using ClusterVR.CreatorKit.Item.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchItem
{
    abstract public class SearchOptionGimmick : SearchOption
    {
        protected const string PropertyNameMovableItem = "movableItem";
        protected const string PropertyNameRidableItem = "ridableItem";
        protected const string PropertyNameCharacterItem = "characterItem";
        protected const string PropertyNameGlobalGimmickKey = "globalGimmickKey";

        public SearchOptionGimmick(Type type) : base(type) { }
        public override SearchKind SearchKind => SearchKind.Gimmick;
    }

    public class SearchOptionGimmickGlobal : SearchOptionGimmick
    {
        public SearchOptionGimmickGlobal(Type type) : base(type) {}
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

    public class SearchOptionGimmickLocal : SearchOptionGimmick
    {
        public SearchOptionGimmickLocal(Type type) : base(type) { }
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

    public class SearchOptionGimmickLocal<T> : SearchOptionGimmick where T : UnityEngine.Object
    {
        public SearchOptionGimmickLocal(Type type, string propertyName) : base(type)
        {
            PropertyName = propertyName;
        }
        string PropertyName { get; set; }
        override protected int SearchEveryComponent(Context context, Component component)
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
                context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            return context.StartId;
        }
    }
    public class SearchOptionGimmickLocalMovable : SearchOptionGimmickLocal<MovableItem>
    {
        public SearchOptionGimmickLocalMovable(Type type) : base(type, PropertyNameMovableItem) { }
    }

    public class SearchOptionGimmickLocalRidable : SearchOptionGimmickLocal<RidableItem>
    {
        public SearchOptionGimmickLocalRidable(Type type) : base(type, PropertyNameRidableItem) { }
    }

    public class SearchOptionGimmickLocalCharacter : SearchOptionGimmickLocal<CharacterItem>
    {
        public SearchOptionGimmickLocalCharacter(Type type) : base(type, PropertyNameCharacterItem) { }
    }

    public class SearchOptionGimmickPlayer : SearchOptionGimmick
    {
        public SearchOptionGimmickPlayer(Type type) : base(type) { }
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

    public class SearchOptionWarpItemGimmick : SearchOptionGimmick
    {
        public SearchOptionWarpItemGimmick(Type type) : base(type) { }
        override protected int SearchEveryComponent(Context context, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyMovableItem = serializedObject.FindProperty(PropertyNameMovableItem);
            var movableItem = serializedPropertyMovableItem.objectReferenceValue as MovableItem;
            var serializedObjectMovableItem = new SerializedObject(movableItem);
            var serializedPropertyItem = serializedObjectMovableItem.FindProperty(PropertyNameItem);
            context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            return context.StartId;
        }
    }
}
