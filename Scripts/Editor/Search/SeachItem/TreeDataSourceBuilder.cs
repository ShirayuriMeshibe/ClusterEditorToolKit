using ShirayuriMeshibe.Search.TreeDataExtension;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    internal class TreeDataSourceBuilder : TreeDataSourceBuilderBase<SearchContext>
    {
        public TreeDataSourceBuilder(SearchContext context) : base(context)
        {
        }
        public HashSet<GameObject> RecordedObjects { get; } = new HashSet<GameObject>();
        public override TreeDataSource Build()
        {
            var dataSource = DataSource;

            // クラス名を除いた実際の検索結果のみをカウントする
            foreach(var root in dataSource.RootObjects)
            {
                int count = 0;
                foreach(var type in root.Children)
                    count += type.Children.Count;
                root.SetCount(count);
            }

            return dataSource;
        }
    }
}
