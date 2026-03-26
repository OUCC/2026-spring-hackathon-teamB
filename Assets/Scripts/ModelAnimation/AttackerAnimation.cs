using UnityEngine;
using UnityEngine.UIElements;

public class AttackerAnimation : MonoBehaviour, IAttackAnimation
{
    [SerializeField]
    private GameObject _normal;

    [SerializeField]
    private GameObject _attacking;

    [field: SerializeField]
    public Image UnitImage { get; }

    private int _attackingLeftFlames = 0;
    private bool _isSubscribed = false;

    void Awake()
    {
        _attacking.SetActive(false);
        _normal.SetActive(true);
    }

    public void AttackAnimation(int flames)
    {
        if (flames <= 0) return;

        _attackingLeftFlames = flames;

        if (!_isSubscribed)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTickEvent += CountFlame;
                _isSubscribed = true;
            }
        }

        if (_normal.activeSelf)
            _normal.SetActive(false);

        if (!_attacking.activeSelf)
            _attacking.SetActive(true);
    }

    public void CountFlame()
    {
        _attackingLeftFlames--;

        if (_attackingLeftFlames <= 0)
        {
            _attackingLeftFlames = 0;

            if (!_normal.activeSelf)
                _normal.SetActive(true);

            if (_attacking.activeSelf)
                _attacking.SetActive(false);

            if (_isSubscribed && GameManager.Instance != null)
            {
                GameManager.Instance.OnTickEvent -= CountFlame;
                _isSubscribed = false;
            }
        }
    }

    private void OnDisable()
    {
        // 念のため解除（破棄・非アクティブ時の事故防止）
        if (_isSubscribed && GameManager.Instance != null)
        {
            GameManager.Instance.OnTickEvent -= CountFlame;
            _isSubscribed = false;
        }
    }
}
