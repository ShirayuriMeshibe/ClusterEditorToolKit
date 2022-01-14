using ShirayuriMeshibe.Search.TreeDataExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search
{
    /// <summary>
    /// 検索結果を表示するウィンドウ
    /// Name,Countのみを表示する
    /// 検索は対応しない
    /// </summary>
    internal abstract class SearchResultTreeViewBase : TreeView
    {
        const float IndentValue = 14f;
        protected class SearchResultTreeViewItem : TreeViewItem
        {
            public int IndentLevel { get; set; }
            public TreeData DataSource { get; set; }
            public int Count { get; set; }
        }

        TreeDataSource _dataSource = null;
        protected TreeDataSource DataSource => _dataSource;
        public SearchResultTreeViewBase(TreeViewState treeViewState) : base(treeViewState)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;

            var nameColumn = new MultiColumnHeaderState.Column()
            {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                canSort = false,
                width = 400f,
                minWidth = 50f,
                autoResize = false,
                allowToggleVisibility = false
            };
            var countColumn = new MultiColumnHeaderState.Column()
            {
                headerContent = new GUIContent("Count"),
                headerTextAlignment = TextAlignment.Left,
                canSort = false,
                width = 50f,
                minWidth = 50f,
                autoResize = true,
                allowToggleVisibility = false
            };

            var headerState = new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] { nameColumn, countColumn });
            this.multiColumnHeader = new MultiColumnHeader(headerState);
            this.foldoutOverride = DoFoldoutButtonOverride;
        }
        public virtual void Clear()
        {
            _dataSource = null;
            Reload();
        }
        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1, displayName = "Results" };
        }
        public virtual void SetDataSource(TreeDataSource dataSourceRoot)
        {
            _dataSource = dataSourceRoot;
            state.expandedIDs.Clear();
            state.selectedIDs.Clear();
            state.lastClickedID = -1;
            Reload();
        }
        public virtual bool HasDataSource => _dataSource != null;
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();
            if (_dataSource != null)
            {
                int indentLevel = 1;

                foreach (var dataSourceRoot in _dataSource.RootObjects)
                {
                    if (0 < dataSourceRoot.GetCount())
                    {
                        var item = CreateFromDataSource(dataSourceRoot);
                        item.IndentLevel = indentLevel;

                        root.AddChild(item);
                        rows.Add(item);

                        if (IsExpanded(item.id))
                            AddRecursiveDataSource(rows, dataSourceRoot, item, indentLevel);
                        else
                            item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
            return rows;
        }
        protected virtual void AddRecursiveDataSource(IList<TreeViewItem> rows, TreeData dataParent, SearchResultTreeViewItem parentItem, int indentLevel)
        {
            int level = indentLevel + 1;
            foreach (var treeData in dataParent.Children)
            {
                if (0 < treeData.GetCount())
                {
                    var childItem = CreateFromDataSource(treeData);
                    childItem.IndentLevel = level;
                    parentItem.AddChild(childItem);
                    rows.Add(childItem);

                    if (IsExpanded(childItem.id))
                        AddRecursiveDataSource(rows, treeData, childItem, level);
                    else
                        childItem.children = CreateChildListForCollapsedParent();
                }
            }
        }
        protected virtual SearchResultTreeViewItem CreateFromDataSource(TreeData treeData)
        {
            Assert.IsNotNull(treeData, "Invalid param. dataSource is null.");
            var item = new SearchResultTreeViewItem()
            {
                id = treeData.Id,
                displayName = treeData.Name,
                DataSource = treeData,
            };

            item.icon = treeData.GetIcon();
            item.Count = treeData.GetCount();
            return item;
        }
        protected override void RowGUI(RowGUIArgs args)
        {
            for (var i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var rect = args.GetCellRect(i);
                var columnIndex = args.GetColumn(i);
                var item = args.item;
                var searchResultTreeViewItem = item as SearchResultTreeViewItem;

                switch (columnIndex)
                {
                    // Name
                    case 0:
                        if (searchResultTreeViewItem != null)
                        {
                            var indentWidth = searchResultTreeViewItem.IndentLevel * IndentValue;
                            _indentLevel = searchResultTreeViewItem.IndentLevel;
                            var r = rect;
                            r.x += indentWidth;
                            EditorGUI.LabelField(r, new GUIContent(args.label, args.item.icon, args.label));
                        }
                        break;
                    // Count
                    case 1:
                        if (searchResultTreeViewItem != null)
                        {
                            var count = searchResultTreeViewItem.DataSource as ITreeDataCount;
                            if (count!=null)
                            {
                                var style = EditorStyles.label;
                                style.alignment = TextAnchor.MiddleRight;
                                var countString = count.Count.ToString("D");
                                EditorGUI.LabelField(rect, countString, style);
                                // MEMO: 元に戻さないと他のコントロールに影響を与える。謎。
                                style.alignment = TextAnchor.MiddleLeft;
                            }
                        }
                        break;
                    default:
                        base.RowGUI(args);
                        break;
                }
            }
        }
        int _indentLevel = 0;
        /// <summary>
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/GUI/TreeView/ToggleTreeView.cs#L286
        /// </summary>
        /// <param name="foldoutRect"></param>
        /// <param name="expandedState"></param>
        /// <param name="foldoutStyle"></param>
        /// <returns></returns>
        bool DoFoldoutButtonOverride(Rect foldoutRect, bool expandedState, GUIStyle foldoutStyle)
        {
            var indentLevel = Mathf.Max(0, _indentLevel - 1);
            var rect = foldoutRect;
            rect.x += indentLevel * IndentValue;
            return EditorGUI.Foldout(rect, expandedState, GUIContent.none, foldoutStyle);
        }
        protected override void SingleClickedItem(int id)
        {
            TreeDataUtils.Selection(_dataSource.GetDataSource(id));
        }
        protected override bool CanMultiSelect(TreeViewItem item) => false;
        /// <summary>
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/91bea9bc5698d7b18524f5e5016c980e4cece639/Editor/Mono/GUI/TreeView/TreeViewControl/TreeViewControl.cs#L573
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/GUI/TreeView/TreeViewUtililty.cs#L94
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            // MEMO: Altキーがクリックされたときの挙動
            var item = FindItem(id, rootItem);
            if (item != null)
            {
                var searchResultTreeViewItem = item as SearchResultTreeViewItem;

                if (searchResultTreeViewItem != null)
                {
                    var list = new List<int>();
                    // TreeViewItemのchildrenはおかしな挙動になるためDataSourceで対応する
                    CollectBelowItem(searchResultTreeViewItem.DataSource, list);
                    return list;
                }
                // ExplandAll()メソッドがコールされたときはルートオブジェクトのidが送信されてくる
                else if (item == rootItem)
                {
                    if (_dataSource != null)
                    {
                        var list = new List<int>();
                        foreach (var dataSource in _dataSource.RootObjects)
                            CollectBelowItem(dataSource, list);
                        return list;
                    }
                }
            }
            return base.GetDescendantsThatHaveChildren(id);
        }
        /// <summary>
        /// 表示する用が多い場合はStackOverflowする。
        /// その場合は下記のような実装に変更する。
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/GUI/TreeView/TreeViewUtililty.cs#L94
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="list"></param>
        void CollectBelowItem(TreeData parent, List<int> list)
        {
            if (parent == null)
                return;

            list.Add(parent.Id);

            foreach (var child in parent.Children)
                CollectBelowItem(child, list);
        }
        public bool ShowDebugLog { get; set; }
    }
}
