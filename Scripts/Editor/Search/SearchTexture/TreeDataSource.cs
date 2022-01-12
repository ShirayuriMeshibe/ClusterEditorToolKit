using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.SearchTexture
{
    internal abstract class TreeDataSource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<TreeDataSource> Children { get; protected set; } = new List<TreeDataSource>();
    }
}
