using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe
{
    public class BakeCubemapWindow : EditorWindow
    {
        enum Resolution : int
        {
            [InspectorName("16")] Resolution16 = 16,
            [InspectorName("32")] Resolution32 = 32,
            [InspectorName("64")] Resolution64 = 64,
            [InspectorName("128")] Resolution128 = 128,
            [InspectorName("256")] Resolution256 = 256,
            [InspectorName("512")] Resolution512 = 512,
            [InspectorName("1024")] Resolution1024 = 1024,
            [InspectorName("2048")] Resolution2048 = 2048,
        }
        [MenuItem(EditorDefine.MenuNameRoot + "Bake Cubemap")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow<BakeCubemapWindow>("BakeCubemap");
        }

        Resolution _resolution = Resolution.Resolution2048;
        bool _hdr = true;
        LayerMask _cullingMask = -1;
        string[] _layerNames = null;
        float _shadowDistance = 100f;
        bool _useOcclusionCulling = true;
        float _near = 0.3f;
        float _far = 1000f;
        void OnEnable()
        {
            {
                var layerNames = new List<string>();
                for (var i = 0; i < 32; ++i)
                {
                    var layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName))
                        layerNames.Add(layerName);
                }
                _layerNames = layerNames.ToArray();
            }
        }
        void OnGUI()
        {
            using (var verticalScope = new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Bake skybox(cubemap) using reflection probe.", EditorStyles.boldLabel);
                EditorGUILayout.Space(6f);

                var w1 = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 170f;
                using (new EditorGUI.IndentLevelScope())
                {
                    _resolution = (Resolution)EditorGUILayout.EnumPopup("Resolution", _resolution);
                    _hdr = EditorGUILayout.Toggle("HDR", _hdr);
                    _shadowDistance = EditorGUILayout.FloatField("Shadow Distance", _shadowDistance);
                    _cullingMask.value = EditorGUILayout.MaskField("Culling Mask", _cullingMask.value, _layerNames);
                    _useOcclusionCulling = EditorGUILayout.Toggle("Occulusion Culling", _useOcclusionCulling);

                    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/ReflectionProbeEditor.cs
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        const string labelClippingPlanes = "Clipping Planes";
                        EditorGUILayout.LabelField(labelClippingPlanes);
                        using (new EditorGUILayout.VerticalScope())
                        {
                            var w2 = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 50f;
                            _near = EditorGUILayout.FloatField("Near", _near);
                            _far = EditorGUILayout.FloatField("Far", _far);
                            EditorGUIUtility.labelWidth = w2;
                        }
                    }
                }
                EditorGUIUtility.labelWidth = w1;
                EditorGUILayout.Space(5f);
                if (GUILayout.Button("Bake"))
                {
                    CreateCubemap(_resolution, _hdr, _shadowDistance, _cullingMask.value, _useOcclusionCulling, _near, _far);
                }
            }
        }
        /// <summary>
        /// https://light11.hatenadiary.com/entry/2018/07/12/231924
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="hdr"></param>
        void CreateCubemap(Resolution resolution, bool hdr, float shadowDistance, int cullingMask, bool useOcclusionCulling, float near, float far)
        {
            // Reflection Probeを配置
            var gameObject = new GameObject("Cubemap Capture Probe");
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            var reflectionProbe = gameObject.AddComponent<ReflectionProbe>();
            var serializedObject = new SerializedObject(reflectionProbe);
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;

            // 設定したい項目があればここで設定
            reflectionProbe.boxProjection = false;
            reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
            reflectionProbe.resolution = (int)resolution;
            reflectionProbe.hdr = hdr;
            reflectionProbe.shadowDistance = shadowDistance;
            reflectionProbe.clearFlags = UnityEngine.Rendering.ReflectionProbeClearFlags.Skybox;
            reflectionProbe.cullingMask = cullingMask;
            reflectionProbe.nearClipPlane = near;
            reflectionProbe.farClipPlane = far;
            serializedObject.Update();
            serializedObject.FindProperty("m_UseOcclusionCulling").boolValue = useOcclusionCulling;
            serializedObject.FindProperty("m_RenderDynamicObjects").boolValue = true;
            serializedObject.ApplyModifiedProperties();

            // デフォルトパスを生成
            string ext = reflectionProbe.hdr ? "exr" : "png";
            var path = EditorSceneManager.GetActiveScene().path;
            if (!string.IsNullOrEmpty(path))
                path = System.IO.Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            // ディレクトリがなければ作る
            else if (System.IO.Directory.Exists(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
            }

            // ファイル保存パネルを表示
            var fileName = $"Skybox.{ext}";
            fileName = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(path, fileName)));
            path = EditorUtility.SaveFilePanelInProject("Save Cubemap", fileName, ext, "", path);

            if (!string.IsNullOrEmpty(path))
            {
                // ベイク
                EditorUtility.DisplayProgressBar("Cubemap", "Baking...", 0.5f);
                if (!Lightmapping.BakeReflectionProbe(reflectionProbe, path))
                {
                    Debug.LogError("Failed to bake cubemap");
                }
                EditorUtility.ClearProgressBar();
            }

            DestroyImmediate(gameObject);
        }
    }
}
