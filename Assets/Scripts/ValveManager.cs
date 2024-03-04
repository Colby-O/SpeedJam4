using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveManager : MonoBehaviour
{

    [SerializeField] LavaManager _lavaManager;
    private GameManager _gameManager;

    private void StopLavaFlow()
    {
        _lavaManager.StopLavaFlow();
        _lavaManager.StartLavaDrain();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopLavaFlow();
            _gameManager.PlaySound(8);
        }
    }

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        if (_lavaManager == null) _lavaManager = FindAnyObjectByType<LavaManager>();
    }
}
