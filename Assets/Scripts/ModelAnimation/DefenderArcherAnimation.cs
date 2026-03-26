using UnityEngine;
using UnityEngine.UIElements;

public class DefenderArcherAnimation : MonoBehaviour, IAttackAnimation
{
    [field: SerializeField]
    public Image UnitImage { get; }

    public void AttackAnimation(int flames)
    {
        return;
    }
}
