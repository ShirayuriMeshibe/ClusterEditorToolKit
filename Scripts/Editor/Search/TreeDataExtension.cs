using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityObject = UnityEngine.Object;

namespace ShirayuriMeshibe.Search.TreeDataExtension
{
    internal static class TreeDataExtension
    {
        static public int GetCount(this TreeData treeData)
        {
            var c = treeData as ITreeDataCount;
            if (c == null)
                return 0;
            return c.Count;
        }
        static public void SetCount(this TreeData treeData, int value)
        {
            var c = treeData as ITreeDataCount;
            if (c == null)
                return;
            c.Count = value;
        }
        static public bool HasCount(this TreeData treeData)
        {
            return (treeData as ITreeDataCount) != null;
        }
        //---------------------------------------------------------------------
        // Icon
        //---------------------------------------------------------------------
        static public Texture2D GetIcon(this TreeData treeData)
        {
            return (treeData as ITreeDataIcon)?.Icon;
        }
        static public void SetIcon(this TreeData treeData, Texture2D value)
        {
            var i = treeData as ITreeDataIcon;
            if (i == null)
                return;
            i.Icon = value;
        }
        static public bool HasIcon(this TreeData treeData)
        {
            return (treeData as ITreeDataIcon) != null;
        }
        //---------------------------------------------------------------------
        // Object
        //---------------------------------------------------------------------
        static public UnityObject GetObject(this TreeData treeData)
        {
            return (treeData as ITreeDataObject)?.Object;
        }
        static public void SetObject(this TreeData treeData, UnityObject value)
        {
            var i = treeData as ITreeDataObject;
            if (i == null)
                return;
            i.Object = value;
        }
        static public bool HasObject(this TreeData treeData)
        {
            return (treeData as ITreeDataObject) != null;
        }
        //---------------------------------------------------------------------
        // Type
        //---------------------------------------------------------------------
        static public Type GetType(this TreeData treeData)
        {
            return (treeData as ITreeDataType)?.Type;
        }
        static public void SetType(this TreeData treeData, Type value)
        {
            var i = treeData as ITreeDataType;
            if (i == null)
                return;
            i.Type = value;
        }
        static public bool HasType(this TreeData treeData)
        {
            return (treeData as ITreeDataType) != null;
        }
    }
}
