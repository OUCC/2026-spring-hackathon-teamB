using UnityEngine;
using UnityEngine.InputSystem;

public class DefencerSpawnTest : MonoBehaviour
{
    [SerializeField] private DefencerUnitSpawner spawner;
    [SerializeField] private string unitName = "yattyo";

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            pos.z = 0f;
            spawner.Spawn(pos, unitName);
        }
    }
}