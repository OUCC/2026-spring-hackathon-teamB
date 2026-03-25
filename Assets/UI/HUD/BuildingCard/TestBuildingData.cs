using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData/BuildingData")]
public class BuildingData : ScriptableObject
{
    /// <summary>
    /// ���j�b�g�̎�ޖ�
    /// </summary>
    public string UnitName;

    /// <summary>
    /// ���j�b�g�̃v���n�u
    /// </summary>
    public BasicAttackerUnit Prefab;

    /// <summary>
    /// ���j�b�g�̍ő�̗�
    /// </summary>
    public int MaxHealth;

    /// <summary>
    /// �����R�X�g
    /// </summary>
    public int SummonCost;

    /// <summary>
    /// �����̌�A���̏������ł���悤�ɂȂ�܂ł̎���
    /// </summary>
    public int SummonCoolTime;

    /// <summary>
    /// �Ֆʂɏ����ł��郆�j�b�g�̍ő吔
    /// </summary>
    public int SummonLimit;

    /// <summary>
    /// �����̃f�[�^(ScriptableObject)
    /// </summary>
    public MoveStrategyData MoveStrategy;

    /// <summary>
    /// �U���̃f�[�^(ScriptableObject)
    /// </summary>
    public AttackStrategyData AttackStrategy;
}
