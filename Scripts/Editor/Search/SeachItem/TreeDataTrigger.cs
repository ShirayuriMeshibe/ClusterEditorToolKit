using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe.Search.SearchItem
{
    internal class TreeDataTriggerRoot : TreeData, ITreeDataCount, ITreeDataIcon
    {
        public TreeDataTriggerRoot()
        {
            Name = "Trigger";
        }
        public int Count { get; set; }
        public Texture2D Icon { get; set; }
    }
    internal class TreeDataTrigger : TreeData, ITreeDataCount, ITreeDataIcon, ITreeDataType
    {
        public TreeDataTrigger() {}
        public int Count { get; set; }
        public Texture2D Icon { get; set; }
        public Type Type { get; set; }
    }
    internal class TreeDataTriggerData : TreeData, ITreeDataObject, ITreeDataIcon
    {
        WeakReference<UnityObject> _weakReference = null;
        public UnityObject Object
        {
            get
            {
                if (_weakReference == null)
                    return null;
                if (!_weakReference.TryGetTarget(out UnityObject obj))
                    return null;
                return obj;
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
