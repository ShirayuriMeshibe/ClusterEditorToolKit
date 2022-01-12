using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe
{
    internal static class EditorIcon
    {
        static public Texture2D GetIcon(UnityObject obj)
        {
            return EditorGUIUtility.IconContent(GetIconName(obj))?.image as Texture2D;
        }
        static public string GetIconName(UnityObject obj)
        {
            if (obj is LineRenderer)
                return EditorIconName.LineRenderer_Icon;
            if (obj is MeshRenderer)
                return EditorIconName.MeshRenderer_Icon;
            if (obj is SkinnedMeshRenderer)
                return EditorIconName.SkinnedMeshRenderer_Icon;
            if (obj is SpriteRenderer)
                return EditorIconName.SpriteRenderer_Icon;
            if (obj is TrailRenderer)
                return EditorIconName.TrailRenderer_Icon;
            if (obj is CustomRenderTexture)
                return EditorIconName.RenderTexture_Icon;
            if (obj is Material)
                return EditorIconName.Material_Icon;
            return EditorIconName.Help;
        }
    }
}
