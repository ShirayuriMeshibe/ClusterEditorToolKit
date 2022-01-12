using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchItem
{
    public class SearchResultRoot
    {
        public SearchKind SearchKind { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count => Objects.Count;
        public void Clear() => Objects.Clear();
        public void Add(SearchResultObject result)
        {
            if (result == null)
                return;
            Objects.Add(result);
        }
        public List<SearchResultObject> Objects { get; private set; } = new List<SearchResultObject>();
    }

    public class SearchResultObject
    {
        System.WeakReference<GameObject> _referenceGameObject = null;

        public SearchObjectKind SearchObjectKind { get; set; }
        public int Id { get; set; }
        public string Path { get; set; }
        public GameObject GameObject
        {
            get
            {
                if (_referenceGameObject == null)
                    return null;
                if (!_referenceGameObject.TryGetTarget(out GameObject gameObject))
                    return null;
                return gameObject;
            }
            set
            {
                _referenceGameObject = new System.WeakReference<GameObject>(value);
            }
        }
    }
}
