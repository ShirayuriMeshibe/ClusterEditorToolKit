using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchItem
{
    // https://github.com/rngtm/UnityEditor-TextureViewer
    class SearchResultTreeView : TreeView
    {
        class RootTreeViewItem : TreeViewItem
        {
            public int Count { get; set; }
        }
        class SearchResultObjectViewItem : TreeViewItem
        {
            public string Path { get; set; }
        }

        List<SearchResultRoot> _searchResultRoot = null;
        Dictionary<int, SearchResultObject> _dictionaryObjects = null;

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
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1, displayName = "Results" };
        }

        public void Setup(List<SearchResultRoot> searchResultRoot, Dictionary<int, SearchResultObject> dictionaryObjects)
        {
            _searchResultRoot = searchResultRoot;
            _dictionaryObjects = dictionaryObjects;
            state.expandedIDs.Clear();
            state.selectedIDs.Clear();
            state.lastClickedID = -1;
            Reload();
        }

        public void Clear()
        {
            _searchResultRoot = null;
            Reload();
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            //var rows = base.BuildRows(root) ?? new List<TreeViewItem>();
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();

            if (_searchResultRoot != null)
            {
                foreach (var searchResultRoot in _searchResultRoot)
                {
                    if (0 < searchResultRoot.Count)
                    {
                        var rootTreeViewItem = new RootTreeViewItem();
                        rootTreeViewItem.id = searchResultRoot.Id;
                        rootTreeViewItem.displayName = searchResultRoot.Name;
                        rootTreeViewItem.Count = searchResultRoot.Count;
                        rootTreeViewItem.icon = EditorGUIUtility.IconContent(GetIconName(searchResultRoot.SearchKind)).image as Texture2D;

                        root.AddChild(rootTreeViewItem);
                        rows.Add(rootTreeViewItem);

                        if (IsExpanded(searchResultRoot.Id))
                        {
                            foreach (var searchResultObject in searchResultRoot.Objects)
                            {
                                var childItem = CreateFromSearchResultObject(searchResultObject);
                                rootTreeViewItem.AddChild(childItem);
                                rows.Add(childItem);
                            }
                        }
                        else
                        {
                            rootTreeViewItem.children = CreateChildListForCollapsedParent();
                        }
                    }
                }
                SetupDepthsFromParentsAndChildren(root);
            }

            return rows;
        }

        /// <summary>
        /// https://baba-s.hatenablog.com/entry/2017/12/01/164517
        /// https://github.com/halak/unity-editor-icons
        /// </summary>
        /// <param name="searchKind"></param>
        /// <returns></returns>
        string GetIconName(SearchKind searchKind)
        {
            switch(searchKind)
            {
                case SearchKind.Gimmick:
                    return "d_UnityEditor.Graphs.AnimatorControllerTool";
                case SearchKind.Trigger:
                    return "AvatarPivot";
                case SearchKind.Operation:
                    return "preAudioAutoPlayOff";
            }

            return "_Help@2x";
        }

        SearchResultObjectViewItem CreateFromSearchResultObject(SearchResultObject searchResultObject)
        {
            return new SearchResultObjectViewItem()
            {
                id = searchResultObject.Id,
                Path = searchResultObject.Path,
                icon = searchResultObject.SearchObjectKind==SearchObjectKind.SceneObject ? EditorGUIUtility.IconContent("GameObject Icon").image as Texture2D : EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D,
            };
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            for (var i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var rect = args.GetCellRect(i);
                var columnIndex = args.GetColumn(i);
                var item = args.item;
                var rootTreeViewItem = item as RootTreeViewItem;

                switch (columnIndex)
                {
                    // Name
                    case 0:
                        if (rootTreeViewItem != null)
                        {
                            var r = rect;
                            r.x += 18f;
                            EditorGUI.LabelField(r, new GUIContent(args.label, args.item.icon));
                        }
                        else
                        {
                            var searchResultObjectViewItem = item as SearchResultObjectViewItem;
                            if (searchResultObjectViewItem != null)
                            {
                                var r = rect;
                                r.x += 30f;
                                EditorGUI.LabelField(r, new GUIContent(searchResultObjectViewItem.Path, args.item.icon, searchResultObjectViewItem.Path));
                            }
                            else
                                base.RowGUI(args);
                        }
                        break;
                    // Count
                    case 1:
                        if (rootTreeViewItem != null)
                        {
                            var style = EditorStyles.label;
                            style.alignment = TextAnchor.MiddleRight;
                            var count = rootTreeViewItem.Count.ToString("D");
                            EditorGUI.LabelField(rect, count, style);
                            // MEMO: 元に戻さないと他のコントロールに影響を与える。謎。
                            style.alignment = TextAnchor.MiddleLeft;
                        }
                        else
                            base.RowGUI(args);
                        break;
                    default:
                        base.RowGUI(args);
                        break;
                }
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            if (_dictionaryObjects.TryGetValue(id, out SearchResultObject result))
            {
                var gameObject = result.GameObject;

                if (gameObject == null)
                    return;

                if(PrefabUtility.IsPartOfAnyPrefab(gameObject))
                    EditorGUIUtility.PingObject(GetParent(gameObject.transform).gameObject);
                else
                    Selection.activeGameObject = result.GameObject;
            }
        }

        Transform GetParent(Transform transform)
        {
            if (transform.parent == null)
                return transform;

            return GetParent(transform.parent);
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;
        protected override float GetCustomRowHeight(int row, TreeViewItem item) => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            //Debug.Log($"GetDescendantsThatHaveChildren() {id}");

            //if (_searchResultRoot == null)
            //{
            //    Debug.LogError($"GetDescendantsThatHaveChildren() id:{id} _searchResultRoot is null.");
            //    return base.GetDescendantsThatHaveChildren(id);
            //}

            //var findItems = FindRows(new List<int>() { id });

            //if (findItems == null || findItems.Count == 0)
            //{
            //    Debug.LogError($"GetDescendantsThatHaveChildren() id:{id} Not found rows.");
            //    return base.GetDescendantsThatHaveChildren(id);
            //}

            //var rootTreeViewItem = findItems[0];
            //var list = new List<int>();

            //if (!IsExpanded(id))
            //{
            //}
            //else
            //{
            //    foreach (var result in _searchResultRoot)
            //    {
            //        if (result.Id == id)
            //        {
            //            Debug.Log($"GetDescendantsThatHaveChildren() Find id:{id}");
            //            foreach (var searchResultObject in result.Objects)
            //            {
            //                var childItem = CreateFromSearchResultObject(searchResultObject);
            //                rootTreeViewItem.AddChild(childItem);
            //            }
            //        }
            //    }
            //}

            //return list;
            // MEMO: ALTキーを押したときに異常終了するのを防ぐ
            return new List<int>(0);
        }

        //protected override bool CanChangeExpandedState(TreeViewItem item)
        //{
        //    Debug.Log($"CanChangeExpandedState() {item.id}");
        //    return base.CanChangeExpandedState(item);
        //}
    }
}
