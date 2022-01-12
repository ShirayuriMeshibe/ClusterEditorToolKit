using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe.SearchTexture
{
    public sealed class SearchTextureWindow : EditorWindow
    {
        [MenuItem(EditorDefine.MenuNameRoot + "Search/Search texture")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<SearchTextureWindow>("SearchTexture");
        }

        Texture _texture = null;
        bool _searchUnusedProperties = false;
        bool _searchRenderers = true;
        bool _searchCustomRenderTextures = true;
        bool _searchPrefabs = false;
        bool _showDebugLog = false;

        [SerializeField] TreeViewState _treeViewState = null;
        SearchResultTreeView _searchResultTreeView = null;

        ReadOnlyCollection<SearchLogic> _searchLogics = new List<SearchLogic>()
        {
            new SearchLogicMaterial(),
            new SearchLogicCamera(),
        }.AsReadOnly();

        void OnEnable()
        {
            if (_treeViewState == null)
                _treeViewState = new TreeViewState();

            _searchResultTreeView = new SearchResultTreeView(_treeViewState);
            _searchResultTreeView.Clear();
        }

        void OnGUI()
        {
            using (var verticalScope = new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.HelpBox("Find references in sceneでは検索があいまいなため厳密な検索を行います", MessageType.Info);
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _texture = EditorGUILayout.ObjectField("Texture", _texture, typeof(Texture), false) as Texture;

                    if (check.changed)
                        _searchResultTreeView.Clear();
                }
                EditorGUILayout.Space(6f);

                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

                    using (new EditorGUI.IndentLevelScope())
                    {
                        foreach(var logic in _searchLogics)
                        {
                            logic.IncludeSearch = EditorGUILayout.ToggleLeft(logic.Name, logic.IncludeSearch);

                            if(logic is SearchLogicMaterial)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    _searchRenderers = EditorGUILayout.ToggleLeft("Search Renderers", _searchRenderers);
                                    _searchCustomRenderTextures = EditorGUILayout.ToggleLeft("Search Custom Render Textures", _searchCustomRenderTextures);
                                    _searchUnusedProperties = EditorGUILayout.ToggleLeft("Search unused properties", _searchUnusedProperties);
                                }
                            }
                            EditorGUILayout.Space(3f);
                        }
                        _searchPrefabs = EditorGUILayout.ToggleLeft("Search Prefabs(very slow)", _searchPrefabs);
                        _showDebugLog = EditorGUILayout.ToggleLeft("Show Debug Log", _showDebugLog);
                    }
                }

                using (new EditorGUI.DisabledScope(_texture == null))
                {
                    EditorGUILayout.Space(6f);
                    if (GUILayout.Button("Search"))
                    {
                        var context = new SearchContext()
                        {
                            Texture = _texture,
                            SearchUnusedProperties = _searchUnusedProperties,
                            SearchRenderers = _searchRenderers,
                            SearchCustomRenderTextures = _searchCustomRenderTextures,
                            ShowDebugLog = _showDebugLog,
                        };

                        var dataSourceBuilder = new TreeDataSourceBuilder(context);

                        if(_showDebugLog)
                            Debug.Log($"[Start] Find texture in scene. name:{_texture.name}");

                        var scene = EditorSceneManager.GetActiveScene();
                        var sceneObject = scene.GetRootGameObjects();

                        GameObject[] prefabObjects = null;
                        if (_searchPrefabs)
                            prefabObjects = AssetDatabaseUtils.LoadAllAssets<GameObject>();

                        foreach (var logic in _searchLogics)
                        {
                            if (logic.IncludeSearch)
                                logic.Search(dataSourceBuilder, sceneObject, prefabObjects);
                        }

                        _searchResultTreeView.SetDataSource(dataSourceBuilder.Build());
                    }
                }
            }

            EditorGUILayout.Space(10f);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    using (new EditorGUI.DisabledScope(!_searchResultTreeView.HasDataSource))
                    {
                        if (GUILayout.Button("Expand All"))
                            _searchResultTreeView.ExpandAll();
                        if (GUILayout.Button("Collapse All"))
                            _searchResultTreeView.CollapseAll();
                    }
                }
                var r1 = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));
                _searchResultTreeView.ShowDebugLog = _showDebugLog;
                _searchResultTreeView.OnGUI(r1);
            }
        }
    }
}
