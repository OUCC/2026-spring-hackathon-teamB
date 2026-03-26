using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class ITargetExtensions
{
    public static IOrderedEnumerable<ITarget> OrderByDistance(this IEnumerable<ITarget> source, Vector3 sourcePositon)
    {
        return source.OrderBy(t => (t.gameObject.transform.position - sourcePositon).sqrMagnitude);
    }
}