using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ShirayuriMeshibe
{
    public static class TransformExtension
    {
        static public Transform GetRoot(this Transform transform)
        {
            if (transform.parent == null)
                return transform;

            return transform.parent.GetRoot();
        }
    }
}
