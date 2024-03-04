using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _disolveTime = 100.0f;

    private int _balloonCount = 0;

    private bool _atDoor = false;
    private DoorManager _doorManager;

    private IEnumerator Dissolve()
    {
        Material playerMat = transform.GetComponent<SpriteRenderer>().material;

        for (float i = 0.0f; i < 1.0f; i += 1.0f / _disolveTime)
        {
            playerMat.SetFloat("_DissolveAmount", i);
            yield return null;
        }
    }

    private void Die()
    {
        _rb.bodyType = RigidbodyType2D.Static;
        Debug.Log("You have died!");
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
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
            _doorManager.Open();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Balloon"))
        {
            _balloonCount += 1;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Button"))
        {
            Debug.Log("Here!");
            if (!collision.gameObject.GetComponent<ButtonManager>().IsPressed) collision.gameObject.GetComponent<ButtonManager>().Press();
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
        _doorManager = FindObjectOfType<DoorManager>();
    }

    private void Update()
    {
        if (_atDoor && _doorManager.IsOpen)
        {
            Debug.Log("Go To Next Level");
        }
    }
}
