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

    public IShootable Source => _source;

    public Vector3 Target => _target;

    public float Speed => _speed;

    public int Damage => _damageAmount;

    public int RemainAttackCount => _attackCount;

    public void ReduceAttackCount()
    {
        _attackCount--;
    }


    public void Move()
    {
        Vector3 diff = _target - transform.position;
        float distance = diff.magnitude;

        if (distance <= _speed)
        {
            transform.position = _target;
            Hit();
            return;
        }

        Vector3 direction = diff.normalized;
        transform.position += direction * _speed;
    }

    public void Die()
    {
        GameManager.Instance.OnTickEvent -= Move;
        Destroy(gameObject);
    }
    private void Hit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.3f);

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            damageable.TakeDamage(_damageAmount);
            ReduceAttackCount();
            break;
        }

        Die();
    }
}
