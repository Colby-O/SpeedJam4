using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _lavaPool;
    [SerializeField] private GameObject _lavaSurface;

    [SerializeField] float _increaseRate;

    private bool isRising = true;

    public void StartLavaFlow()
    {
        isRising = true;
        _increaseRate = Mathf.Abs(_increaseRate);
    }

    public void StopLavaFlow()
    {
        isRising = false;
    }

    public void StartLavaDrain()
    {
        isRising = true;
        _increaseRate = -Mathf.Abs(_increaseRate);
    }

    private void Start()
    {
        _lavaSurface.GetComponent<SpriteRenderer>().material.EnableKeyword("_ISSURFACE");
    }

    private void FixedUpdate()
    {
        if (isRising && _lavaPool.transform.localScale.y >= 0.0f)
        {
            _lavaSurface.transform.position += transform.up * _increaseRate;
            _lavaPool.transform.position += transform.up * _increaseRate / 2.0f;
            _lavaPool.transform.localScale += transform.up * _increaseRate;
        } else if (isRising)
        {
            gameObject.SetActive(false);
        }
    }
}
