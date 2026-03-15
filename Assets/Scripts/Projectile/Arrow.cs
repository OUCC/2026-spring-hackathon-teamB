using UnityEngine;

public class Arrow : MonoBehaviour, IProjectile
{
    private IShootable _source;
    private Vector3 _target;
    private float _speed;
    private int _damageAmount;
    private int _attackCount;

    public void Initialize(IShootable source, Vector3 target,float speed, int damageAmount, int attackCount)
    {
        _source = source;
        _target = target;
        _speed = speed;
        _damageAmount = damageAmount;
        _attackCount = attackCount;

        GameManager.Instance.OnTickEvent += Move;
    }

    public IShootable GetSource()
    {
        return _source;
    }

    public Vector3 GetTarget()
    {
        return _target;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public int GetDamage()
    {
        return _damageAmount;
    }

    public int GetRemainAttackCount()
    {
        return _attackCount;
    }

    public void ReduceAttackCount()
    {
        _attackCount--;
    }


    public void Move()
    {
        if(transform.position != _target)
        {
            var direction = (_target - transform.position).normalized;
            transform.position += direction * _speed;
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        GameManager.Instance.OnTickEvent -= Move;
        Destroy(gameObject);
    }
}
