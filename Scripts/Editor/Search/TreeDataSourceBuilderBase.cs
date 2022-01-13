using ShirayuriMeshibe.Search.TreeDataExtension;
using System;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search
{
    internal abstract class TreeDataSourceBuilderBase<ContextType>
        where ContextType : class
    {
        readonly ContextType _context = null;
        readonly TreeDataSource _dataSource = new TreeDataSource();
        int _id = 0;

        public TreeDataSourceBuilderBase(ContextType context)
        {
            _context = context;
        }
        public ContextType Context => _context;
        protected TreeDataSource DataSource => _dataSource;
        public virtual TreeDataSource Build() => _dataSource;
        protected int SumCount(TreeData dataSource, int count)
        {
            foreach (var child in dataSource.Children)
            {
                count += child.GetCount();
                count = SumCount(child, count);
            }
            return count;
        }
        protected void UpdateCount(TreeData treeData)
        {
            treeData.SetCount(treeData.Children.Count);
            foreach (var child in treeData.Children)
                UpdateCount(child);
        }
        public T CreateOrGetRoot<T>() where T : TreeData, new()
        {
            var dataSource = _dataSource.GetRoot(typeof(T));
            var dataSourceType = dataSource as T;
            //Assert.IsNotNull(dataSourceType, $"DataSource don't cast {typeof(T).Name}");

            if (dataSource != null)
                return dataSourceType;

            dataSourceType = new T();
            dataSourceType.Id = ++_id;
            _dataSource.AddRoot(typeof(T), dataSourceType);
            return dataSourceType;
        }

        public T CreateAndAddChild<T>(TreeData parent) where T : TreeData, new()
        {
            var dataSource = new T();
            dataSource.Id = ++_id;
            _dataSource.RegisterChildDataSource(dataSource);

            if (parent != null)
            {
                parent.Children.Add(dataSource);
                parent.SetCount(parent.Children.Count);
            }

            return dataSource;
        }
        public TreeData GetTreeDataType<T>(TreeData parent, Type type) where T : TreeData, new()
        {
            if (parent == null)
                return null;
            foreach (var treeData in parent.Children)
            {
                var t = treeData as ITreeDataType;
                if (t == null)
                    continue;
                if (t.Type == type)
                    return treeData;
            }
            var treeDataNew = CreateAndAddChild<T>(parent);
            treeDataNew.SetType(type);
            return treeDataNew;
        }
    }
}
