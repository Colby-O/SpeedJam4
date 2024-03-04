using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level : MonoBehaviour
{
    public Vector3 startingPosition;
    public LavaManager _lavaManager;
    public GameObject _balloon;
    public GameObject _key;
    public List<ButtonManager> _buttons;
    public DoorManager _doorManager;
    public float _lavaRaiseSpeed = 0.01f;
    public float _lavaLowerSpeed = 0.1f;

    public void StartLevel(PlayerManager _player)
    {
        _doorManager.Close();
        _lavaManager.SetRaiseLavaSpeed(_lavaRaiseSpeed);
        _lavaManager.SetLowerLavaSpeed(_lavaLowerSpeed);
        _player.gameObject.transform.position = startingPosition;
        gameObject.SetActive(true);
        _balloon.SetActive(true);
        _key.SetActive(true);
        foreach (var button in _buttons) button.Unpress();
        _lavaManager.LavaReset();
    }

    public void EndLevel()
    {
        gameObject.SetActive(false);
    }

    public void Awake()
    {
        gameObject.SetActive(false);
    }
}
