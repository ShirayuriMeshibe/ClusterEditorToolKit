using ClusterVR.CreatorKit.Gimmick.Implements;
using ClusterVR.CreatorKit.Item.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchItem
{
    public abstract class SearchOption
    {
        protected const string PropertyNameGimmickKey = "key";
        protected const string PropertyNameGimmickTarget = "target";
        protected const string PropertyNameItem = "item";

        protected class Context
        {
            public Item SearchItem { get; set; }
            public SearchResultRoot SearchResultRoot { get; set; }
            public int StartId { get; set; }
            public Dictionary<int, SearchResultObject> DictionaryObjects { get; set; }
            public HashSet<GameObject> RecordedObjects { get; set; }
            public bool IsPrefab { get; set; }
            public bool ShowDebugLog { get; set; }
        }

        public SearchOption(Type type)
        {
            Assert.IsNotNull(type, "Invalid param. type is null.");
            Type = type;
            Name = type.Name;
        }

        public Type Type { get; protected set; }
        public string Name { get; protected set; }
        public bool IncludeSearch { get; set; } = true;
        public SearchResultRoot SearchResultRoot { get; protected set; } = new SearchResultRoot();
        virtual public int Search(GameObject[] rootObjects, GameObject[] prefabObjects, Item searchItem, int startId, Dictionary<int, SearchResultObject> dictionaryObjects, bool showDebugLog)
        {
            Assert.IsNotNull(Type, "Type is null.");
            var recordedObjects = new HashSet<GameObject>();
            var result = new SearchResultRoot();
            result.SearchKind = SearchKind;
            result.Id = startId++;
            result.Name = Name;

            var context = new Context()
            {
                SearchItem = searchItem,
                SearchResultRoot = result,
                StartId = startId,
                DictionaryObjects = dictionaryObjects,
                RecordedObjects = recordedObjects,
                ShowDebugLog = showDebugLog,
            };

            try
            {
                context.IsPrefab = false;
                foreach (var gameObject in rootObjects)
                {
                    var componets = gameObject.GetComponentsInChildren(Type, true);
                    foreach (var component in componets)
                        context.StartId = SearchEveryComponent(context, component);
                }

                context.IsPrefab = true;
                foreach (var gameObject in prefabObjects)
                {
                    var componets = gameObject.GetComponentsInChildren(Type, true);
                    foreach (var component in componets)
                        context.StartId = SearchEveryComponent(context, component);
                }
            }
            catch(Exception ex)
            {
                if (context.ShowDebugLog)
                {
                    Debug.LogError($"Failed search. Type:{Type.Name}");
                    Debug.LogException(ex);
                    throw ex;
                }
            }

            SearchResultRoot = result;
            return context.StartId;
        }
        abstract protected int SearchEveryComponent(Context context, Component component);
        protected int AddResultIfSpecifiedItem(Context context, Component component, SerializedProperty serializedProperty)
        {
            if (serializedProperty == null)
            {
                if(context.ShowDebugLog)
                    Debug.LogError($"Invalid param . serializedProperty is null. Type:{Name}, SearchOption:{GetType().Name}");
                return context.StartId;
            }

            var startId = context.StartId;
            var item = serializedProperty.objectReferenceValue as Item;

            if (item != null && item.gameObject == context.SearchItem.gameObject && !context.RecordedObjects.Contains(context.SearchItem.gameObject))
            {
                var resultObject = new SearchResultObject();
                resultObject.Id = startId++;
                resultObject.Path = Utils.GetPath(component.transform);
                resultObject.GameObject = component.gameObject;
                resultObject.SearchObjectKind = context.IsPrefab ? SearchObjectKind.Prefab : SearchObjectKind.SceneObject;
                context.SearchResultRoot.Add(resultObject);

                context.DictionaryObjects.Add(resultObject.Id, resultObject);
                context.RecordedObjects.Add(context.SearchItem.gameObject);
            }
            return startId;
        }
        abstract public SearchKind SearchKind { get; }
    }
}
