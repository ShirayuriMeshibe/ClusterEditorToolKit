using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe.SearchTexture
{
    internal abstract class SearchLogic
    {
        public SearchLogic(Type type)
        {
            Assert.IsNotNull(type, "Invalid param. type is null.");
            Type = type;
            Name = type.Name;
        }
        public string Name { get; protected set; }
        public Type Type { get; protected set; }
        public bool IncludeSearch { get; set; } = true;
        public abstract void Search(TreeDataSourceBuilder dataSourceBuilder, GameObject[] gameObjects, GameObject[] prefabObjects);
    }

    internal abstract class SearchLogic<T> : SearchLogic where T : UnityObject
    {
        public SearchLogic() : base(typeof(T)) {}

        protected T[] GetComponents(GameObject gameObject)
        {
            return gameObject.GetComponentsInChildren<T>(true);
        }
    }
}
