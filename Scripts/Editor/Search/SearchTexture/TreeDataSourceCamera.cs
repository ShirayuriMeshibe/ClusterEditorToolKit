using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe.SearchTexture
{
    internal class TreeDataSourceCameraRoot : TreeDataSource, ITreeDataSourceCount, ITreeDataSourceIcon
    {
        public TreeDataSourceCameraRoot()
        {
            Name = "Camera";
        }
        public int Count { get; set; }
        public Texture2D Icon { get; set; }
    }
    internal class TreeDataSourceCamera : TreeDataSource, ITreeDataSourceObject, ITreeDataSourceIcon
    {
        WeakReference<UnityObject> _weakReference = null;
        public UnityObject Object
        {
            get
            {
                if (_weakReference == null)
                    return null;
                if (!_weakReference.TryGetTarget(out UnityObject camera))
                    return null;
                return camera;
            }
            set
            {
                if (value == null)
                    return;
                _weakReference = new WeakReference<UnityObject>(value);
            }
        }
        public Texture2D Icon { get; set; }
    }
}
