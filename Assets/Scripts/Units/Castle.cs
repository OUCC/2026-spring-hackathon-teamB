using System;

using UnityEngine;

public class Castle : MonoBehaviour, IDamageable, ITarget
{
    [SerializeField] private int _health = 100;
    public GameManager.TeamType Team => throw new NotImplementedException();

    public event Action<ITarget> OnDied;

    public void Die()
    {
        OnDied?.Invoke(this);
        GameManager.Instance.NotifyWhiteCoreDestroyed();
    }

    public Vector3 GetTargetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(int damageAmount)
    {
        _health -= damageAmount;
        if (_health <= 0)
        {
            Die();
        }
    }
}
