using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchTexture
{
    internal class SearchResultTreeView : TreeView
    {
        const float IndentValue = 16f;
        class SearchResultTreeViewItem : TreeViewItem
        {
            public int IndentLevel { get; set; }
            public TreeDataSource DataSource { get; set; }
            public int Count { get; set; }
        }

        TreeDataSourceRoot _dataSourceRoot = null;

        public SearchResultTreeView(TreeViewState treeViewState) : base(treeViewState)
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
                autoResize = true,
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
        public void Clear()
        {
            _dataSourceRoot = null;
            Reload();
        }
        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1, displayName = "Results" };
        }
        public void SetDataSource(TreeDataSourceRoot dataSourceRoot)
        {
            _dataSourceRoot = dataSourceRoot;
            state.expandedIDs.Clear();
            state.selectedIDs.Clear();
            state.lastClickedID = -1;
            Reload();
        }
        public bool HasDataSource => _dataSourceRoot != null;
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();
            if(_dataSourceRoot!=null)
            {
                int indentLevel = 1;

                foreach(var dataSourceRoot in _dataSourceRoot.RootObjects)
                {
                    var count = dataSourceRoot as ITreeDataSourceCount;

                    if (count != null && 0 < count.Count)
                    {
                        var item = CreateFromDataSource(dataSourceRoot);
                        item.IndentLevel = indentLevel;
                        //item.icon = EditorGUIUtility.IconContent(GetIconName(searchResultRoot.SearchKind)).image as Texture2D;

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
        void AddRecursiveDataSource(IList<TreeViewItem> rows, TreeDataSource dataSourceParent, SearchResultTreeViewItem parentItem, int indentLevel)
        {
            int level = indentLevel + 1;
            foreach (var dataSource in dataSourceParent.Children)
            {
                var childItem = CreateFromDataSource(dataSource);
                childItem.IndentLevel = level;
                parentItem.AddChild(childItem);
                rows.Add(childItem);

                var count = childItem.DataSource.Children.Count;
                if (0 < count)
                {
                    if (IsExpanded(childItem.id))
                        AddRecursiveDataSource(rows, dataSource, childItem, level);
                    else
                        childItem.children = CreateChildListForCollapsedParent();
                }
            }
        }
        SearchResultTreeViewItem CreateFromDataSource(TreeDataSource dataSource)
        {
            Assert.IsNotNull(dataSource, "Invalid param. dataSource is null.");
            var item = new SearchResultTreeViewItem()
            {
                id = dataSource.Id,
                displayName = dataSource.Name,
                DataSource = dataSource,
                //icon = EditorGUIUtility.IconContent(GetIconName(dataSource)).image as Texture2D,
            };

            var icon = dataSource as ITreeDataSourceIcon;
            item.icon = icon != null ? icon.Icon : null;

            var count = dataSource as ITreeDataSourceCount;
            item.Count = count!=null ? count.Count : 0;

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
                            EditorGUI.LabelField(r, new GUIContent(args.label, args.item.icon));
                        }
                        break;
                    // Count
                    case 1:
                        if (searchResultTreeViewItem != null)
                        {
                            var count = searchResultTreeViewItem.DataSource as ITreeDataSourceCount;
                            if (count != null)
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
        protected override void DoubleClickedItem(int id)
        {
            var dataSource = _dataSourceRoot.GetDataSource(id);
            if (dataSource == null)
                return;

            var dataSourceObject = dataSource as ITreeDataSourceObject;

            if (dataSourceObject == null)
                return;

            var obj = dataSourceObject.Object;

            if (obj == null)
                return;

            if (!PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                Selection.activeObject = obj;
                return;
            }

            var nearest = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
            if(nearest!=null)
            {
                Selection.activeObject = obj;
                return;
            }

            GameObject gameObject = null;
            var component = obj as Component;

            if (component != null)
                gameObject = component.gameObject;

            if(gameObject==null)
                gameObject = obj as GameObject;

            Assert.IsNotNull(gameObject, $"Faild cast. {obj.GetType().Name}");
            if (gameObject != null)
            {
                if(ShowDebugLog)
                    Debug.Log($"PingObject:{gameObject.transform.GetRoot()}");
                EditorGUIUtility.PingObject(gameObject.transform.GetRoot());
            }
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

                if(searchResultTreeViewItem!=null)
                {
                    var list = new List<int>();
                    // TreeViewItemのchildrenはおかしな挙動になるためDataSourceで対応する
                    CollectBelowItem(searchResultTreeViewItem.DataSource, list);
                    return list;
                }
                // ExplandAll()メソッドがコールされたときはルートオブジェクトのidが送信されてくる
                else if(item == rootItem)
                {
                    if (_dataSourceRoot != null)
                    {
                        var list = new List<int>();
                        foreach (var dataSource in _dataSourceRoot.RootObjects)
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
        void CollectBelowItem(TreeDataSource parent, List<int> list)
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
