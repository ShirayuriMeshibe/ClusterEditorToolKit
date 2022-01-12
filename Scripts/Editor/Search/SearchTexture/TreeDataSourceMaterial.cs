using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe.SearchTexture
{
    internal class TreeDataSourceMaterialRoot : TreeDataSource, ITreeDataSourceCount, ITreeDataSourceIcon
    {
        public TreeDataSourceMaterialRoot()
        {
            Name = "Material";
        }
        public int Count { get; set; }
        public Texture2D Icon { get; set; }
    }
    internal class TreeDataSourceMaterial : TreeDataSource, ITreeDataSourceCount, ITreeDataSourceObject, ITreeDataSourceIcon
    {
        WeakReference<UnityObject> _weakReference = null;
        public int Count { get; set; }
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
    internal class TreeDataSourceRenderer : TreeDataSource, ITreeDataSourceObject, ITreeDataSourceIcon
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
