using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search.SearchTexture
{
    internal sealed class SearchContext
    {
        public Texture Texture { get; set; }
        public bool SearchUnusedProperties { get; set; } = false;
        public bool SearchRenderers { get; set; } = false;
        public bool SearchCustomRenderTextures { get; set; } = false;
        public bool ShowDebugLog { get; set; } = false;
    }
}
