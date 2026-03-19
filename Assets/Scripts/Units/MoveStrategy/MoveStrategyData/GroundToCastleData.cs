
using UnityEngine;

[CreateAssetMenu(fileName = "GroundToCastleData", menuName = "ScriptableObjects/MoveStrategyData/GroundToCastleData")]
public class GroundToCastleData : MoveStrategyData
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _moveRequiredTime;

    public override IMoveStrategy CreateInstance()
    {
        return new GroundToCastle(_moveSpeed, _moveRequiredTime);
    }
}
