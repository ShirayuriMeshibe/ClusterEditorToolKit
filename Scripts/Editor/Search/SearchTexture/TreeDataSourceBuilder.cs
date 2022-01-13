using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchTexture
{
    internal class TreeDataSourceBuilder : TreeDataSourceBuilderBase<SearchContext>
    {
        public TreeDataSourceBuilder(SearchContext context) : base(context)
        {
        }
        public override TreeDataSource Build()
        {
            foreach(var root in DataSource.RootObjects)
            {
                if (root is TreeDataMaterialRoot)
                {
                    int count = 0;
                    var c = root as ITreeDataCount;
                    if(c!=null)
                        c.Count = SumCount(root, count);
                }
            }

            return DataSource;
        }
    }
}
