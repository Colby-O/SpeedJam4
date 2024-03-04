using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _disolveTime = 100.0f;
    [SerializeField] private TMP_Text _balloonCountText;

    private GameManager _gameManager;

    private int _balloonCount = 0;

    private bool _atDoor = false;

    public bool GotBalloon;

    private IEnumerator Dissolve()
    {
        Material playerMat = transform.GetComponent<SpriteRenderer>().material;

        for (float i = 0.0f; i < 1.0f; i += 1.0f / _disolveTime)
        {
            playerMat.SetFloat("_DissolveAmount", i);
            yield return null;
        }
        _gameManager.RestartCurrentLevel();
    }

    private void Die()
    {
        _rb.bodyType = RigidbodyType2D.Static;
        Debug.Log("You have died!");
    }

    public void DecreaseBallonCount()
    {
        _balloonCount--;
        if (_balloonCountText != null) _balloonCountText.text = _balloonCount.ToString();
    }

    public int GetNumberOfBalloons() 
    { 
        return _balloonCount; 
    }

    public void ResetNumberOfBalloons()
    {
        _balloonCount = 0;
        if (_balloonCountText != null) _balloonCountText.text = _balloonCount.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava") || collision.gameObject.CompareTag("Trap"))
        {
            if (collision.gameObject.CompareTag("Lava"))
            {
                _gameManager.PlaySound(0);
            }
            _gameManager.PlaySound(2);
            StartCoroutine(Dissolve());
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            _atDoor = true;
        }
        else if (collision.gameObject.CompareTag("Key"))
        {
            _gameManager.GetCurrentLevel().gameObject.GetComponentInChildren<DoorManager>().Open();
            collision.gameObject.SetActive(false);
            _gameManager.PlaySound(4);
            _gameManager.PlaySound(6);
        }
        else if (collision.gameObject.CompareTag("Balloon"))
        {
            _balloonCount += 1;
            GotBalloon = true;
            if (_balloonCountText != null) _balloonCountText.text = _balloonCount.ToString();
            collision.gameObject.SetActive(false);
            _gameManager.PlaySound(3);
        }
        else if (collision.gameObject.CompareTag("Button"))
        {
            if (!collision.gameObject.GetComponent<ButtonManager>().IsPressed)
            {
                collision.gameObject.GetComponent<ButtonManager>().Press();
                _gameManager.PlaySound(5);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            _atDoor = false;
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gameManager = FindAnyObjectByType<GameManager>();

        _balloonCount = 0;
        if (_balloonCountText != null) _balloonCountText.text = _balloonCount.ToString();
    }

    private void Update()
    {
        if (_atDoor && _gameManager.GetCurrentLevel().gameObject.GetComponentInChildren<DoorManager>().IsOpen)
        {
            _gameManager.NextLevel();
        }
    }
}
