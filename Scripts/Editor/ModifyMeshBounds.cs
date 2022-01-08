using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe
{
    public sealed class ModifyMeshBounds : EditorWindow
    {
        const float LINE_MARGIN = 0.8f;

        [MenuItem("Cluster/Assets/Modify Mesh Bounds")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<ModifyMeshBounds>("MeshBounds");
        }

        Mesh _mesh = null;
        Vector3 _center;
        Vector3 _extents;
        Vector3 _max;
        Vector3 _min;
        bool _isChanged = false;

        void OnGUI()
        {
            EditorGUILayout.LabelField("Modify mesh bounds.", EditorStyles.boldLabel);
            EditorGUILayout.Space(10f);

            using (new EditorGUI.IndentLevelScope())
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _mesh = EditorGUILayout.ObjectField("Mesh", _mesh, typeof(Mesh), false) as Mesh;

                    using (new EditorGUI.DisabledScope(_mesh == null))
                    {
                        _isChanged = _isChanged || check.changed;
                        EditorGUILayout.Space(10f);
                        if (GUILayout.Button("Refresh") || _isChanged)
                        {
                            var bounds = _mesh.bounds;
                            _center = bounds.center;
                            _extents = bounds.extents;
                            _max = bounds.max;
                            _min = bounds.min;
                        }

                        if (_mesh == null)
                        {
                            _center = Vector3.zero;
                            _extents = Vector3.zero;
                            _max = Vector3.zero;
                            _min = Vector3.zero;
                        }
                    }
                }

                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("Bounds", EditorStyles.boldLabel);
                using (new EditorGUI.IndentLevelScope())
                {

                    var isWideMode = EditorGUIUtility.wideMode;
                    EditorGUIUtility.wideMode = true;

                    _center = EditorGUILayout.Vector3Field($"Center", _center);
                    EditorGUILayout.Space(LINE_MARGIN);
                    _extents = EditorGUILayout.Vector3Field($"Extents", _extents);
                    EditorGUILayout.Space(LINE_MARGIN);
                    EditorGUILayout.Vector3Field($"Max", _max);
                    EditorGUILayout.Space(LINE_MARGIN);
                    EditorGUILayout.Vector3Field($"Min", _min);
                    EditorGUILayout.Space(LINE_MARGIN);

                    EditorGUIUtility.wideMode = isWideMode;

                    using (new EditorGUI.DisabledScope(_mesh == null))
                    {
                        EditorGUILayout.Space(10f);
                        if (GUILayout.Button("Update"))
                        {
                            var bounds = _mesh.bounds;
                            bounds.center = _center;
                            bounds.extents = _extents;
                            _mesh.bounds = bounds;

                            _isChanged = true;

                            // MEMO: _isChangedを更新した後にフォーカスを外さないと反映されなくなる
                            GUI.FocusControl("");
                            GUIUtility.ExitGUI();
                        }
                        else
                            _isChanged = false;
                    }
                }
            }
        }
    }
}
