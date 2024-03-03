using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveManager : MonoBehaviour
{

    [SerializeField] LavaManager _lavaManager;

    private void StopLavaFlow()
    {
        _lavaManager.StopLavaFlow();
        _lavaManager.StartLavaDrain();
        Debug.Log("Level Completed!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopLavaFlow();
        }
    }

    private void Start()
    {
        if (_lavaManager == null) _lavaManager = FindAnyObjectByType<LavaManager>();
    }
}
