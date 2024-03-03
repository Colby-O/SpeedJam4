using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _disolveTime = 100.0f;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            StartCoroutine(Dissolve());
            Die();
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }
}
