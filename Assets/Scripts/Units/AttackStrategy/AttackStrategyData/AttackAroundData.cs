using UnityEngine;

[CreateAssetMenu(fileName = "AttackAroundData", menuName = "ScriptableObjects/AttackStrategyData/AttackAroundData")]
public class AttackAroundData : AttackStrategyData
{
    [SerializeField]
    private int _cooldownTime;

    [SerializeField]
    private float _attackRange;

    [SerializeField]
    private int _attackDamage;

    public override IAttackStrategy CreateInstance()
    {
        return new AttackAround(_cooldownTime, _attackRange, _attackDamage);
    }
}