using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchTexture
{
    internal class SearchResultTreeView : SearchResultTreeViewBase
    {
        public SearchResultTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
        }
        protected override void AddRecursiveDataSource(IList<TreeViewItem> rows, TreeData dataParent, SearchResultTreeViewItem parentItem, int indentLevel)
        {
            int level = indentLevel + 1;
            foreach (var treeData in dataParent.Children)
            {
                var childItem = CreateFromDataSource(treeData);
                childItem.IndentLevel = level;
                parentItem.AddChild(childItem);
                rows.Add(childItem);

                var count = childItem.DataSource.Children.Count;
                if (0 < count)
                {
                    if (IsExpanded(childItem.id))
                        AddRecursiveDataSource(rows, treeData, childItem, level);
                    else
                        childItem.children = CreateChildListForCollapsedParent();
                }
            }
        }
    }
}
