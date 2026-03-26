using UnityEngine;
using UnityEngine.UIElements;

public class DefenderArcherAnimation : MonoBehaviour, IAttackAnimation
{
    [field: SerializeField]
    public Image UnitImage { get; }

    [SerializeField]
    private GameObject _archer;

    public void AttackAnimation(int flames, Quaternion quaternion)
    {
        _archer.transform.localRotation = quaternion;
    }
}
