using ClusterVR.CreatorKit.Gimmick;
using ClusterVR.CreatorKit.Gimmick.Implements;
using ClusterVR.CreatorKit.Item.Implements;
using ClusterVR.CreatorKit.Operation.Implements;
using ClusterVR.CreatorKit.Trigger.Implements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    public sealed class SeachItemEditorWindow : EditorWindow
    {
        [MenuItem(EditorDefine.MenuNameRoot + "Search/Search item")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<SeachItemEditorWindow>("SearchItem");
        }

        Item _item = null;

        bool _isFoldOutTrigger = false;
        bool _isFoldOutOperation = false;
        bool _isFoldOutGimmick = false;

        Vector2 _scrollPositionTrigger;
        Vector2 _scrollPositionOperation;
        Vector2 _scrollPositionGimmick;

        bool _findPrefabs = false;
        bool _showDebugLog = false;

        [SerializeField] TreeViewState _treeViewState = null;
        SearchResultTreeView _searchResultTreeView = null;

        ReadOnlyCollection<SearchLogic> _searchLogics = new List<SearchLogic>()
        {
            // Trigger
            { new SearchLogicTriggerConstant(typeof(InitializePlayerTrigger)) }, // Specified Itemが選択できない
            { new SearchLogicTriggerConstant(typeof(InteractItemTrigger)) },
            { new SearchLogicTriggerVariable(typeof(IsGroundedCharacterItemTrigger)) },
            { new SearchLogicTriggerVariable(typeof(OnAngularVelocityChangedItemTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnCollideItemTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnCreateItemTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnGetOffItemTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnGrabItemTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnJoinPlayerTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnReceiveOwnershipItemTrigger)) },
            { new SearchLogicTriggerConstant(typeof(OnReleaseItemTrigger)) },
            { new SearchLogicTriggerVariable(typeof(OnVelocityChangedItemTrigger)) },
            { new SearchLogicTriggerSteerItem(typeof(SteerItemTrigger)) },
            { new SearchLogicTriggerUseItem(typeof(UseItemTrigger)) },
            // Operation
            { new SearchLogicOperationGlobal(typeof(GlobalTriggerLottery)) },
            { new SearchLogicOperationLocal(typeof(ItemTriggerLottery)) },
            { new SearchLogicOperationPlayer(typeof(PlayerTriggerLottery)) },
            // Gimmick
            { new SearchLogicGimmickLocalMovable(typeof(AddContinuousForceItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(AddContinuousTorqueItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(AddInstantForceItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(AddInstantTorqueItemGimmick)) },
            { new SearchLogicGimmickLocal(typeof(CreateItemGimmick)) },
            { new SearchLogicGimmickLocal(typeof(DestroyItemGimmick)) },
            { new SearchLogicGimmickLocalRidable(typeof(GetOffItemGimmick)) },
            { new SearchLogicGimmickLocalCharacter(typeof(JumpCharacterItemGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(PlayAudioSourceGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(PlayTimelineGimmick)) },
            { new SearchLogicGimmickPlayer(typeof(RespawnPlayerGimmick)) },
            { new SearchLogicGimmickLocalCharacter(typeof(SetAngularVelocityCharacterItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(SetAngularVelocityItemGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetAnimatorValueGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetFillAmountGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetGameObjectActiveGimmick)) },
            { new SearchLogicGimmickPlayer(typeof(SetJumpHeightRatePlayerGimmick)) },
            { new SearchLogicGimmickPlayer(typeof(SetMoveSpeedRatePlayerGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetSliderValueGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetTextGimmick)) },
            { new SearchLogicGimmickLocalCharacter(typeof(SetVelocityCharacterItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(SetVelocityItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(SetWheelColliderBrakeTorqueItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(SetWheelColliderMotorTorqueItemGimmick)) },
            { new SearchLogicGimmickLocalMovable(typeof(SetWheelColliderSteerAngleItemGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(StopTimelineGimmick)) },
            { new SearchLogicWarpItemGimmick(typeof(WarpItemGimmick)) },
            { new SearchLogicGimmickPlayer(typeof(WarpPlayerGimmick)) },
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
                EditorGUILayout.LabelField("Search gimmicks and triggers that references specified item.", EditorStyles.boldLabel);
                EditorGUILayout.Space(6f);

                using (new EditorGUI.IndentLevelScope())
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        _item = EditorGUILayout.ObjectField("Item", _item, typeof(Item), true) as Item;

                        if (check.changed)
                            _searchResultTreeView.Clear();
                    }
                    EditorGUILayout.Space(6f);

                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
                        _isFoldOutTrigger = EditorGUILayout.Foldout(_isFoldOutTrigger, "Triggers");
                        if (_isFoldOutTrigger)
                        {
                            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPositionTrigger, GUILayout.Height(100f)))
                            {
                                _scrollPositionTrigger = scrollView.scrollPosition;
                                using (new EditorGUI.IndentLevelScope(2))
                                {
                                    foreach (var option in _searchLogics)
                                    {
                                        if (option.SearchKind == SearchKind.Trigger)
                                            option.IncludeSearch = EditorGUILayout.ToggleLeft(option.Name, option.IncludeSearch);
                                    }
                                }
                            }
                        }

                        _isFoldOutOperation = EditorGUILayout.Foldout(_isFoldOutOperation, "Operations");
                        if (_isFoldOutOperation)
                        {
                            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPositionOperation, GUILayout.Height(100f)))
                            {
                                _scrollPositionOperation = scrollView.scrollPosition;
                                using (new EditorGUI.IndentLevelScope(2))
                                {
                                    foreach (var option in _searchLogics)
                                    {
                                        if (option.SearchKind == SearchKind.Operation)
                                            option.IncludeSearch = EditorGUILayout.ToggleLeft(option.Name, option.IncludeSearch);
                                    }
                                }
                            }
                        }

                        _isFoldOutGimmick = EditorGUILayout.Foldout(_isFoldOutGimmick, "Gimmicks");
                        if (_isFoldOutGimmick)
                        {
                            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPositionGimmick, GUILayout.Height(100f)))
                            {
                                _scrollPositionGimmick = scrollView.scrollPosition;
                                using (new EditorGUI.IndentLevelScope(2))
                                {
                                    foreach (var option in _searchLogics)
                                    {
                                        if(option.SearchKind == SearchKind.Gimmick)
                                            option.IncludeSearch = EditorGUILayout.ToggleLeft(option.Name, option.IncludeSearch);
                                    }
                                }
                            }
                        }

                        EditorGUILayout.Space(3f);
                        _findPrefabs = EditorGUILayout.ToggleLeft("Search prefabs(Very Slow)", _findPrefabs);
                        _showDebugLog = EditorGUILayout.ToggleLeft("Show Debug Log", _showDebugLog);
                    }
                }

                using (new EditorGUI.DisabledScope(_item == null))
                {
                    EditorGUILayout.Space(5f);
                    if (GUILayout.Button("Search"))
                    {
                        var context = new SearchContext()
                        {
                            Item = _item,
                            ShowDebugLog = _showDebugLog,
                        };
                        var dataSourceBuilder = new TreeDataSourceBuilder(context);
                        var scene = EditorSceneManager.GetActiveScene();
                        var rootObjects = scene.GetRootGameObjects();

                        GameObject[] prefabObjects = new GameObject[0];

                        if(_findPrefabs)
                        {
                            prefabObjects = AssetDatabaseUtils.LoadAllAssets<GameObject>();

                            if(_showDebugLog)
                                Debug.Log($"prefabObjects:{prefabObjects.Length}");
                        }

                        foreach (var logic in _searchLogics)
                            logic.Search(dataSourceBuilder, rootObjects, prefabObjects);

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
                _searchResultTreeView.OnGUI(r1);
            }
        }
    }
}
