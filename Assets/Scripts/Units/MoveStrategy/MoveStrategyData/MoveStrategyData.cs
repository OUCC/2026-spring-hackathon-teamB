using System;

using UnityEngine;

[Serializable]
public abstract class MoveStrategyData : ScriptableObject
{
    public abstract IMoveStrategy CreateInstance();
}
