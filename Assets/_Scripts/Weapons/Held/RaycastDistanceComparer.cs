using System.Collections.Generic;
using UnityEngine;

/* Utility comparer so we can Array.Sort() RaycastHit[] without a lambda
   (avoids an allocation each fire). */
public sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
{
    /* static singleton instance to avoid allocating new comparers */
    public static readonly RaycastHitDistanceComparer Instance
        = new RaycastHitDistanceComparer();

    private RaycastHitDistanceComparer() { }          // private ctor

    public int Compare(RaycastHit a, RaycastHit b)
    {
        return a.distance.CompareTo(b.distance);
    }
}