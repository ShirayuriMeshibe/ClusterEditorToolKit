using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchTexture
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
}
