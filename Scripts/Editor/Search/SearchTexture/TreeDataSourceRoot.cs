using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchTexture
{
    internal class TreeDataSourceRoot
    {
        Dictionary<Type, TreeDataSource> _dictionaryRoot = new Dictionary<Type, TreeDataSource>();
        Dictionary<int, TreeDataSource> _reverseIdDictionay = new Dictionary<int, TreeDataSource>();
        public IList<TreeDataSource> RootObjects { get; } = new List<TreeDataSource>();
        public TreeDataSource GetRoot(Type type)
        {
            if (_dictionaryRoot.TryGetValue(type, out TreeDataSource dataSource))
                return dataSource;

            return null;
        }
        public void AddRoot(Type type, TreeDataSource dataSource)
        {
            Assert.IsNotNull(type, "Invalid param. type is null.");
            Assert.IsNotNull(dataSource, "Invalid param. dataSource is null.");
            if (type==null || dataSource == null)
                return;
            if (!_dictionaryRoot.ContainsKey(type))
                _dictionaryRoot.Add(type, dataSource);
            if (!_reverseIdDictionay.ContainsKey(dataSource.Id))
                _reverseIdDictionay.Add(dataSource.Id, dataSource);
            RootObjects.Add(dataSource);
        }
        public void RegisterChildDataSource(TreeDataSource dataSource)
        {
            Assert.IsNotNull(dataSource, "Invalid param. dataSource is null.");
            if (!_reverseIdDictionay.ContainsKey(dataSource.Id))
                _reverseIdDictionay.Add(dataSource.Id, dataSource);
        }
        public TreeDataSource GetDataSource(int id)
        {
            if (!_reverseIdDictionay.TryGetValue(id, out TreeDataSource dataSource))
                return null;
            return dataSource;
        }
    }
}
