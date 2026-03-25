using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerAssignManager : MonoBehaviour
{
    [Header("シーンに配置したPlayerInputをセット")]
    [SerializeField] private PlayerInput _attackPlayerInput;
    [SerializeField] private PlayerInput _defensePlayerInput;

    // GamepadとJoystickの両方を保存できるように、共通の型「InputDevice」に変更
    private List<InputDevice> _assignedDevices = new List<InputDevice>();

    private void Start()
    {
        if (_attackPlayerInput != null) _attackPlayerInput.DeactivateInput();
        if (_defensePlayerInput != null) _defensePlayerInput.DeactivateInput();
        
        Debug.Log("参加受付中... コントローラーのボタン（A/×/トリガーなど）を押してください");
    }

    private void Update()
    {
        if (_assignedDevices.Count >= 2) return;

        // PCに繋がっている「すべての入力デバイス」をチェック
        foreach (InputDevice device in InputSystem.devices)
        {
            if (_assignedDevices.Contains(device)) continue;

            bool buttonPressed = false;
            string controlScheme = ""; // どの操作設定を使うか

            // ① デバイスが標準的なゲームパッドの場合
            if (device is Gamepad gamepad)
            {
                if (gamepad.buttonSouth.wasPressedThisFrame || gamepad.startButton.wasPressedThisFrame)
                {
                    buttonPressed = true;
                    controlScheme = "Gamepad";
                }
            }
            // ② デバイスが汎用ジョイスティック（あなたのHIDコントローラー）の場合
            else if (device is Joystick joystick)
            {
                if (joystick.trigger.wasPressedThisFrame) // HIDコントローラーの1番ボタン（大抵はAや×にあたる）
                {
                    buttonPressed = true;
                    controlScheme = "Joystick";
                }
            }

            // どちらかのボタンが押されていたら、割り当てを実行
            if (buttonPressed)
            {
                AssignPlayer(device, controlScheme);
            }
        }
    }

    private void AssignPlayer(InputDevice device, string controlScheme)
    {
        if (_assignedDevices.Count == 0)
        {
            // 1人目：攻め側に割り当て
            _attackPlayerInput.SwitchCurrentControlScheme(controlScheme, device);
            _attackPlayerInput.ActivateInput();
            _assignedDevices.Add(device);
            
            Debug.Log($"1Pが参加しました：攻め側 (認識名: {device.name})");
        }
        else if (_assignedDevices.Count == 1)
        {
            // 2人目：守り側に割り当て
            _defensePlayerInput.SwitchCurrentControlScheme(controlScheme, device);
            _defensePlayerInput.ActivateInput();
            _assignedDevices.Add(device);
            
            Debug.Log($"2Pが参加しました：守り側 (認識名: {device.name})");
        }
    }
}