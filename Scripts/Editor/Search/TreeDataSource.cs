using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search
{
    internal class TreeDataSource
    {
        Dictionary<Type, TreeData> _dictionaryRoot = new Dictionary<Type, TreeData>();
        Dictionary<int, TreeData> _reverseIdDictionay = new Dictionary<int, TreeData>();
        public IList<TreeData> RootObjects { get; } = new List<TreeData>();
        public TreeData GetRoot(Type type)
        {
            if (_dictionaryRoot.TryGetValue(type, out TreeData dataSource))
                return dataSource;

            return null;
        }
        public void AddRoot(Type type, TreeData dataSource)
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
        public void RegisterChildDataSource(TreeData dataSource)
        {
            Assert.IsNotNull(dataSource, "Invalid param. dataSource is null.");
            if (!_reverseIdDictionay.ContainsKey(dataSource.Id))
                _reverseIdDictionay.Add(dataSource.Id, dataSource);
        }
        public TreeData GetDataSource(int id)
        {
            if (!_reverseIdDictionay.TryGetValue(id, out TreeData dataSource))
                return null;
            return dataSource;
        }
    }
}
