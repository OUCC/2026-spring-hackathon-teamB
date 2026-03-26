using UnityEngine;

[CreateAssetMenu(fileName = "AttackArrowData", menuName = "ScriptableObjects/AttackStrategyData/AttackArrowData")]
public class AttackArrowData : AttackStrategyData
{
    [SerializeField]
    private int _cooldownTime;
    [SerializeField]
    private float _attackRange;
    [SerializeField]
    private int _attackDamage;

    public override IAttackStrategy CreateInstance()
    {
        return new AttackArrow(_cooldownTime, _attackRange, _attackDamage);
    }
}