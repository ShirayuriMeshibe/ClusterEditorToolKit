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

namespace ShirayuriMeshibe.SearchItem
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


        ReadOnlyCollection<SearchOption> _searchOptions = new List<SearchOption>()
        {
            // Trigger
            { new SearchOptionTriggerConstant(typeof(InitializePlayerTrigger)) }, // Specified Itemが選択できない
            { new SearchOptionTriggerConstant(typeof(InteractItemTrigger)) },
            { new SearchOptionTriggerVariable(typeof(IsGroundedCharacterItemTrigger)) },
            { new SearchOptionTriggerVariable(typeof(OnAngularVelocityChangedItemTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnCollideItemTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnCreateItemTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnGetOffItemTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnGrabItemTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnJoinPlayerTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnReceiveOwnershipItemTrigger)) },
            { new SearchOptionTriggerConstant(typeof(OnReleaseItemTrigger)) },
            { new SearchOptionTriggerVariable(typeof(OnVelocityChangedItemTrigger)) },
            { new SearchOptionTriggerSteerItem(typeof(SteerItemTrigger)) },
            { new SearchOptionTriggerUseItem(typeof(UseItemTrigger)) },
            // Operation
            { new SearchOptionOperationGlobal(typeof(GlobalTriggerLottery)) },
            { new SearchOptionOperationLocal(typeof(ItemTriggerLottery)) },
            { new SearchOptionOperationPlayer(typeof(PlayerTriggerLottery)) },
            // Gimmick
            { new SearchOptionGimmickLocalMovable(typeof(AddContinuousForceItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(AddContinuousTorqueItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(AddInstantForceItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(AddInstantTorqueItemGimmick)) },
            { new SearchOptionGimmickLocal(typeof(CreateItemGimmick)) },
            { new SearchOptionGimmickLocal(typeof(DestroyItemGimmick)) },
            { new SearchOptionGimmickLocalRidable(typeof(GetOffItemGimmick)) },
            { new SearchOptionGimmickLocalCharacter(typeof(JumpCharacterItemGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(PlayAudioSourceGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(PlayTimelineGimmick)) },
            { new SearchOptionGimmickPlayer(typeof(RespawnPlayerGimmick)) },
            { new SearchOptionGimmickLocalCharacter(typeof(SetAngularVelocityCharacterItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(SetAngularVelocityItemGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetAnimatorValueGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetFillAmountGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetGameObjectActiveGimmick)) },
            { new SearchOptionGimmickPlayer(typeof(SetJumpHeightRatePlayerGimmick)) },
            { new SearchOptionGimmickPlayer(typeof(SetMoveSpeedRatePlayerGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetSliderValueGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(SetTextGimmick)) },
            { new SearchOptionGimmickLocalCharacter(typeof(SetVelocityCharacterItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(SetVelocityItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(SetWheelColliderBrakeTorqueItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(SetWheelColliderMotorTorqueItemGimmick)) },
            { new SearchOptionGimmickLocalMovable(typeof(SetWheelColliderSteerAngleItemGimmick)) },
            { new SearchOptionGimmickGlobal(typeof(StopTimelineGimmick)) },
            { new SearchOptionWarpItemGimmick(typeof(WarpItemGimmick)) },
            { new SearchOptionGimmickPlayer(typeof(WarpPlayerGimmick)) },
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
                                    foreach (var option in _searchOptions)
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
                                    foreach (var option in _searchOptions)
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
                                    foreach (var option in _searchOptions)
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
                        // TreeViewItemのidからObjectを検索するためのDictionary
                        var dictionaryObjects = new Dictionary<int, SearchResultObject>();
                        int id = 0;

                        var scene = EditorSceneManager.GetActiveScene();
                        var rootObjects = scene.GetRootGameObjects();

                        GameObject[] prefabObjects = new GameObject[0];

                        if(_findPrefabs)
                        {
                            prefabObjects = AssetDatabaseUtils.LoadAllAssets<GameObject>();

                            if(_showDebugLog)
                                Debug.Log($"prefabObjects:{prefabObjects.Length}");
                        }

                        foreach (var option in _searchOptions)
                            id = option.Search(rootObjects, prefabObjects, _item, id, dictionaryObjects, _showDebugLog);

                        // TreeViewに表示するデータの生成
                        {
                            var list = new List<SearchResultRoot>(_searchOptions.Count);
                            foreach (var option in _searchOptions)
                            {
                                if (option.IncludeSearch)
                                    list.Add(option.SearchResultRoot);
                            }

                            _searchResultTreeView.Setup(list, dictionaryObjects);
                        }
                    }
                }
            }

            EditorGUILayout.Space(10f);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);
                var r1 = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));
                _searchResultTreeView.OnGUI(r1);
            }
        }
    }
}
