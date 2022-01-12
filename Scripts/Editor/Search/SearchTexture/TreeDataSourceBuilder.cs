using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchTexture
{
    internal class TreeDataSourceBuilder
    {
        readonly SearchContext _context = null;
        readonly TreeDataSourceRoot _dataSourceRoot = new TreeDataSourceRoot();
        int _id = 0;

        public TreeDataSourceBuilder(SearchContext context)
        {
            _context = context;
        }
        public SearchContext Context => _context;
        public TreeDataSourceRoot Build()
        {
            foreach(var root in _dataSourceRoot.RootObjects)
            {
                if (root is TreeDataSourceMaterialRoot)
                {
                    int count = 0;
                    var c = root as ITreeDataSourceCount;
                    if(c!=null)
                        c.Count = SumCount(root, count);
                }
            }

            return _dataSourceRoot;
        }
        int SumCount(TreeDataSource dataSource, int count)
        {
            foreach (var child in dataSource.Children)
            {
                var c = child as ITreeDataSourceCount;
                count += c!=null? c.Count : 0;
                count = SumCount(child, count);
            }
            return count;
        }
        void UpdateCount(TreeDataSource dataSource)
        {
            var c = dataSource as ITreeDataSourceCount;
            if (c != null)
                c.Count = dataSource.Children.Count;
            foreach (var child in dataSource.Children)
                UpdateCount(child);
        }
        public T CreateOrGetRoot<T>() where T : TreeDataSource, new()
        {
            var dataSource = _dataSourceRoot.GetRoot(typeof(T));
            var dataSourceType = dataSource as T;
            //Assert.IsNotNull(dataSourceType, $"DataSource don't cast {typeof(T).Name}");

            if (dataSource != null)
                return dataSourceType;

            dataSourceType = new T();
            dataSourceType.Id = ++_id;
            _dataSourceRoot.AddRoot(typeof(T), dataSourceType);
            return dataSourceType;
        }

        public T CreateAndAddChild<T>(TreeDataSource parent) where T : TreeDataSource, new()
        {
            var dataSource = new T();
            dataSource.Id = ++_id;
            _dataSourceRoot.RegisterChildDataSource(dataSource);

            if (parent != null)
            {
                parent.Children.Add(dataSource);

                var count = parent as ITreeDataSourceCount;
                if (count != null)
                    count.Count = parent.Children.Count;
            }

            return dataSource;
        }
    }
}
