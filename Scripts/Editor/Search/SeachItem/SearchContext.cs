using ClusterVR.CreatorKit.Item.Implements;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchItem
{
    internal sealed class SearchContext
    {
        public Item Item { get; set; }
        public bool ShowDebugLog { get; set; } = false;
    }
}
