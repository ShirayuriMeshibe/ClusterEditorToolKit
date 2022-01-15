using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    abstract internal class SearchOptionTrigger : SearchLogic<TreeDataTriggerRoot, TreeDataTrigger, TreeDataTriggerData>
    {
        protected const string PropertyNameTriggers = "triggers";
        protected const string PropertyNameTarget = "target";
        protected const string PropertyNameSpecifiedTargetItem = "specifiedTargetItem";
        public SearchOptionTrigger(Type type) : base(type, EditorIconName.AvatarPivot) { }
        public override SearchKind SearchKind => SearchKind.Trigger;
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/ConstantTriggerParam.cs
    /// </summary>
    internal class SearchLogicTriggerConstant : SearchOptionTrigger
    {
        public SearchLogicTriggerConstant(Type type) : base(type) {}
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyTriggers = serializedObject.FindProperty(PropertyNameTriggers);

            for (int i = 0; i < serializedPropertyTriggers.arraySize; ++i)
            {
                var serializedPropertyTrigger = serializedPropertyTriggers.GetArrayElementAtIndex(i);
                var serializedPropertyTarget = serializedPropertyTrigger.FindPropertyRelative(PropertyNameTarget);
                var serializedPropertyItem = serializedPropertyTrigger.FindPropertyRelative(PropertyNameSpecifiedTargetItem);

                // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Trigger/TriggerTarget.cs
                // TriggerTarget.SpecifiedItemのとき
                if (serializedPropertyTarget.enumValueIndex == 1)
                    AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
            }
        }
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/VariableTriggerParam.cs
    /// </summary>
    internal class SearchLogicTriggerVariable : SearchOptionTrigger
    {
        public SearchLogicTriggerVariable(Type type) : base(type) { }
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);
            var serializedPropertyTriggers = serializedObject.FindProperty(PropertyNameTriggers);

            for (int i = 0; i < serializedPropertyTriggers.arraySize; ++i)
            {
                var serializedPropertyTrigger = serializedPropertyTriggers.GetArrayElementAtIndex(i);
                var serializedPropertyTarget = serializedPropertyTrigger.FindPropertyRelative(PropertyNameTarget);
                var serializedPropertyItem = serializedPropertyTrigger.FindPropertyRelative(PropertyNameSpecifiedTargetItem);

                // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Trigger/TriggerTarget.cs
                // TriggerTarget.SpecifiedItemのとき
                if (serializedPropertyTarget.enumValueIndex == 1)
                    AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
            }
        }
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/SteerItemTrigger.cs
    /// </summary>
    internal class SearchLogicTriggerSteerItem : SearchOptionTrigger
    {
        public SearchLogicTriggerSteerItem(Type type) : base(type) {}
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);

            string[] searchPropertyNames = { "moveInputTriggers", "additionalAxisInputTriggers" };

            foreach (var propertyName in searchPropertyNames)
            {
                var serializedPropertyTriggers = serializedObject.FindProperty(propertyName);

                for (int i = 0; i < serializedPropertyTriggers.arraySize; ++i)
                {
                    var serializedPropertyTrigger = serializedPropertyTriggers.GetArrayElementAtIndex(i);
                    var serializedPropertyTarget = serializedPropertyTrigger.FindPropertyRelative(PropertyNameTarget);
                    var serializedPropertyItem = serializedPropertyTrigger.FindPropertyRelative(PropertyNameSpecifiedTargetItem);

                    // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Trigger/TriggerTarget.cs
                    // TriggerTarget.SpecifiedItemのとき
                    if (serializedPropertyTarget.enumValueIndex == 1)
                        AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
                }
            }
        }
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/UseItemTrigger.cs
    /// </summary>
    internal class SearchLogicTriggerUseItem : SearchOptionTrigger
    {
        public SearchLogicTriggerUseItem(Type type) : base(type) { }
        override protected void SearchEveryComponent(TreeDataSourceBuilder dataSourceBuilder, TreeData dataParent, Component component)
        {
            var serializedObject = new SerializedObject(component);

            string[] searchPropertyNames = { "downTriggers", "upTriggers" };

            foreach(var propertyName in searchPropertyNames)
            {
                var serializedPropertyTriggers = serializedObject.FindProperty(propertyName);

                for (int i = 0; i < serializedPropertyTriggers.arraySize; ++i)
                {
                    var serializedPropertyTrigger = serializedPropertyTriggers.GetArrayElementAtIndex(i);
                    var serializedPropertyTarget = serializedPropertyTrigger.FindPropertyRelative(PropertyNameTarget);
                    var serializedPropertyItem = serializedPropertyTrigger.FindPropertyRelative(PropertyNameSpecifiedTargetItem);

                    // https://github.com/ClusterVR/ClusterCreatorKit/blob/96ec906c9bfd474c3468787f8ce97784273b64e4/Runtime/Trigger/TriggerTarget.cs
                    // TriggerTarget.SpecifiedItemのとき
                    if (serializedPropertyTarget.enumValueIndex == 1)
                        AddResultIfSpecifiedItem(dataSourceBuilder, dataParent, component, serializedPropertyItem);
                }
            }
        }
    }
}
