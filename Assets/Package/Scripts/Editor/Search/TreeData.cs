using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe.Search
{
    internal abstract class TreeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<TreeData> Children { get; protected set; } = new List<TreeData>();

    }
}
