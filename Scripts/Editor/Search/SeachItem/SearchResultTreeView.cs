using ShirayuriMeshibe.Search.TreeDataExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    internal class SearchResultTreeView : SearchResultTreeViewBase
    {
        public SearchResultTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
        }
        //protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        //{
        //    var rows = GetRows() ?? new List<TreeViewItem>();
        //    rows.Clear();
        //    if (HasDataSource)
        //    {
        //        int indentLevel = 1;

        //        foreach (var dataSourceRoot in DataSource.RootObjects)
        //        {
        //            if (0 < dataSourceRoot.GetCount())
        //            {
        //                var item = CreateFromDataSource(dataSourceRoot);
        //                item.IndentLevel = indentLevel;

        //                root.AddChild(item);
        //                rows.Add(item);

        //                AddRecursiveDataSource(rows, dataSourceRoot, item, indentLevel);
        //            }
        //        }
        //    }
        //    return rows;
        //}
        protected override void AddRecursiveDataSource(IList<TreeViewItem> rows, TreeData dataParent, SearchResultTreeViewItem parentItem, int indentLevel)
        {
            int level = indentLevel + 1;
            foreach (var treeData in dataParent.Children)
            {
                var hasChildren = 0 < treeData.Children.Count;
                var hasCountProperty = treeData.HasCount();
                if (hasChildren || !hasCountProperty)
                {
                    var childItem = CreateFromDataSource(treeData);
                    childItem.IndentLevel = level;
                    parentItem.AddChild(childItem);
                    rows.Add(childItem);

                    if (hasChildren)
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
}
