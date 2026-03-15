/// <summary>
/// 盤面上でダメージを受けることができるユニットのインターフェース
/// </summary>
public interface IDamageable : ITarget
{
    void TakeDamage(int damageAmount);

    void Die();
}

