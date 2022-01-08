using ClusterVR.CreatorKit.Item.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchItem
{
    abstract public class SearchOptionTrigger : SearchOption
    {
        protected const string PropertyNameTriggers = "triggers";
        protected const string PropertyNameTarget = "target";
        protected const string PropertyNameSpecifiedTargetItem = "specifiedTargetItem";
        public SearchOptionTrigger(Type type) : base(type) { }
        public override SearchKind SearchKind => SearchKind.Trigger;
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/ConstantTriggerParam.cs
    /// </summary>
    public class SearchOptionTriggerConstant : SearchOptionTrigger
    {
        public SearchOptionTriggerConstant(Type type) : base(type) {}
        override protected int SearchEveryComponent(Context context, Component component)
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
                    context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            }
            return context.StartId;
        }
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/VariableTriggerParam.cs
    /// </summary>
    public class SearchOptionTriggerVariable : SearchOptionTrigger
    {
        public SearchOptionTriggerVariable(Type type) : base(type) { }
        override protected int SearchEveryComponent(Context context, Component component)
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
                    context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
            }
            return context.StartId;
        }
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/SteerItemTrigger.cs
    /// </summary>
    public class SearchOptionTriggerSteerItem : SearchOptionTrigger
    {
        public SearchOptionTriggerSteerItem(Type type) : base(type) {}
        override protected int SearchEveryComponent(Context context, Component component)
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
                        context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
                }
            }
            return context.StartId;
        }
    }

    /// <summary>
    /// https://github.com/ClusterVR/ClusterCreatorKit/blob/master/Runtime/Trigger/Implements/UseItemTrigger.cs
    /// </summary>
    public class SearchOptionTriggerUseItem : SearchOptionTrigger
    {
        public SearchOptionTriggerUseItem(Type type) : base(type) { }
        override protected int SearchEveryComponent(Context context, Component component)
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
                        context.StartId = AddResultIfSpecifiedItem(context, component, serializedPropertyItem);
                }
            }
            return context.StartId;
        }
    }
}
