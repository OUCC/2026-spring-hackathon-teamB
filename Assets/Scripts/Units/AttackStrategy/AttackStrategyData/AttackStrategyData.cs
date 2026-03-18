using System;

using UnityEngine;

[Serializable]
public abstract class AttackStrategyData : ScriptableObject
{
    public abstract IAttackStrategy CreateInstance();
}
