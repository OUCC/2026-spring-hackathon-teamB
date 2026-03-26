using UnityEngine;

[CreateAssetMenu(fileName = "NoAttackingData", menuName = "ScriptableObjects/AttackStrategyData/NoAttackingData")]
public class NoAttackingData : AttackStrategyData
{

    public override IAttackStrategy CreateInstance()
    {
        return new NoAttacking();
    }
}