using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe
{
    public static class ContextMenuTransform
    {
        const string MENU_PATH_COPY_WORLD_POSITION = "CONTEXT/Transform/Copy World Position";

        [MenuItem(MENU_PATH_COPY_WORLD_POSITION, true)]
        private static bool MenuCopyWorldPositionValidator(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            return transform != null;
        }

        [MenuItem(MENU_PATH_COPY_WORLD_POSITION)]
        private static void MenuCopyWorldPosition(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            var p = transform.position;
            GUIUtility.systemCopyBuffer = string.Format($"{p.x}, {p.y}, {p.z}");
        }

        const string MENU_PATH_COPY_WORLD_ROTATION = "CONTEXT/Transform/Copy World Rotation";

        [MenuItem(MENU_PATH_COPY_WORLD_ROTATION, true)]
        private static bool MenuCopyWorldRotationValidator(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            return transform != null;
        }

        [MenuItem(MENU_PATH_COPY_WORLD_ROTATION)]
        private static void MenuCopyWorldRotation(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            var p = transform.rotation.eulerAngles;
            GUIUtility.systemCopyBuffer = string.Format($"{p.x}, {p.y}, {p.z}");
        }

        const string MENU_PATH_COPY_WORLD_ROTATION_QUATERNION = "CONTEXT/Transform/Copy World Rotation(Quaternion)";

        [MenuItem(MENU_PATH_COPY_WORLD_ROTATION_QUATERNION, true)]
        private static bool MenuCopyWorldRotationQuaternionValidator(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            return transform != null;
        }

        [MenuItem(MENU_PATH_COPY_WORLD_ROTATION_QUATERNION)]
        private static void MenuCopyWorldRotationQuaternion(MenuCommand menuCommand)
        {
            var transform = menuCommand.context as Transform;
            var q = transform.rotation;
            GUIUtility.systemCopyBuffer = string.Format($"{q.x}, {q.y}, {q.z}, {q.w}");
        }
    }
}
