using ClusterVR.CreatorKit.Gimmick;
using ClusterVR.CreatorKit.Gimmick.Implements;
using ClusterVR.CreatorKit.Operation;
using ClusterVR.CreatorKit.Operation.Implements;
using ClusterVR.CreatorKit.Trigger;
using ClusterVR.CreatorKit.Trigger.Implements;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
//using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace ShirayuriMeshibe.Search.SearchItem
{
    public static class SeachItemEditorWindowTest
    {
        const string MenuName = "Assets/Testing/Test SeachItemEditorWindow";

        [MenuItem(MenuName)]
        static void CheckClass()
        {
            var window = ScriptableObject.CreateInstance<SeachItemEditorWindow>();
            try
            {
                var t = window.GetType();
                var logicsField = t.GetField("_searchLogics", BindingFlags.NonPublic | BindingFlags.Instance);
                var logics = logicsField.GetValue(window) as ReadOnlyCollection<SearchLogic>;
                Assert.IsNotNull(logics, "Not found field. _searchLogics");
                CheckClass(logics);
            }
            finally
            {
                Editor.DestroyImmediate(window);
            }
        }

        static void CheckClass(IList<SearchLogic> searchLogics)
        {
            var assemblyTrigger = Assembly.GetAssembly(typeof(IGlobalTrigger));
            var assemblyOperation = Assembly.GetAssembly(typeof(IGlobalLogic));
            var assemblyGimmick = Assembly.GetAssembly(typeof(ICreateItemGimmick));

            var types = new List<Type>();
            types.AddRange(assemblyTrigger.GetTypes());
            types.AddRange(assemblyOperation.GetTypes());
            types.AddRange(assemblyGimmick.GetTypes());

            var interfaces = GetInterfaces(types);

            CheckClass<InitializePlayerTrigger>(searchLogics, interfaces); // Trigger
            CheckClass<GlobalLogic>(searchLogics, interfaces); // Operation
            CheckClass<AddContinuousForceItemGimmick>(searchLogics, interfaces); // Gimmick
        }
        static List<Type> GetInterfaces(List<Type> types)
        {
            var list = new List<Type>();

            if (types == null)
                return list;

            foreach (var type in types)
            {
                if (!type.IsInterface)
                    continue;

                if (!type.IsPublic)
                    continue;
                list.Add(type);
            }

            return list;
        }
        static void CheckClass<TImplement>(IList<SearchLogic> searchLogics, List<Type> interfaces)
            where TImplement : class
        {
            var assembly = Assembly.GetAssembly(typeof(TImplement));
            var implements = GetClasses(interfaces, assembly);
            CheckClass(searchLogics, implements);
        }

        static List<Type> GetClasses(List<Type> interfaces, Assembly assembly)
        {
            var list = new List<Type>();

            if (interfaces == null)
                return list;

            if (assembly == null)
                return list;

            var hash = new HashSet<Type>();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsPublic)
                    continue;

                foreach (var i in interfaces)
                {
                    if(!hash.Contains(type) && type.GetInterfaces().Contains(i))
                    {
                        hash.Add(type);
                        list.Add(type);
                    }
                }
            }

            return list;
        }
        static void CheckClass(IList<SearchLogic> searchLogics, List<Type> implements)
        {
            if(searchLogics==null)
            {
                Debug.LogError($"Invalid param. searchLogics is null.");
                return;
            }

            if (implements == null)
            {
                Debug.LogError($"Invalid param. implements is null.");
                return;
            }

            var hash = new HashSet<Type>();
            var list = new List<SearchLogic>(searchLogics);

            foreach(var t in implements)
            {
                if (!hash.Contains(t))
                {
                    if (list.Find((logic) => logic.Type == t)==null)
                    {
                        hash.Add(t);
                        Debug.LogError($"Not implement class. Type:{t.Name}, {new Uri(t.Assembly.CodeBase).AbsolutePath}, {t.AssemblyQualifiedName}");
                    }
                }
            }
        }
    }
}
