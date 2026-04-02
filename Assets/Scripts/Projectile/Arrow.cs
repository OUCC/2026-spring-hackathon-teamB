using UnityEngine;

public class Arrow : MonoBehaviour, IProjectile
{
    private const float BoardMargin = 1f;

    private IShootable _source;
    private Vector3 _target;
    private Vector3 _direction;
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

        Vector3 diff = _target - transform.position;
        _direction = diff.sqrMagnitude > Mathf.Epsilon ? diff.normalized : Vector3.zero;

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
        if (_direction == Vector3.zero)
        {
            Die();
            return;
        }

        transform.position += _direction * _speed;
        Hit();

        if (IsOutsideBoard())
        {
            Die();
        }
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
            if (_source is Component sourceComponent && hit.transform.IsChildOf(sourceComponent.transform)) continue;

            damageable.TakeDamage(_damageAmount);
            ReduceAttackCount();
            break;
        }

        if (_attackCount <= 0)
        {
            Die();
        }
    }

    private bool IsOutsideBoard()
    {
        GridManager gridManager = GameManager.Instance != null ? GameManager.Instance.GridManager : null;
        if (gridManager == null)
        {
            return false;
        }

        float halfCellSize = gridManager.CellSize * 0.5f;
        float minX = -halfCellSize - BoardMargin;
        float maxX = ((gridManager.Width - 1) * gridManager.CellSize) + halfCellSize + BoardMargin;
        float minZ = -halfCellSize - BoardMargin;
        float maxZ = ((gridManager.Height - 1) * gridManager.CellSize) + halfCellSize + BoardMargin;

        Vector3 position = transform.position;
        return position.x < minX || position.x > maxX || position.z < minZ || position.z > maxZ;
    }
}
