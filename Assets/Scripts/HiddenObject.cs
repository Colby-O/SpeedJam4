using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    [SerializeField] private Sprite _hiddenSprite;
    [SerializeField] private Sprite _showSprite;

    private SpriteRenderer _renderer;
    private BoxCollider2D _coli;

    public void Hide()
    {
        _renderer.sprite = _hiddenSprite;
        _coli.isTrigger = true;
    }

    public void Show()
    {
        _renderer.sprite = _showSprite;
        _coli.isTrigger = false;
    }

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _coli = GetComponent<BoxCollider2D>();
        Hide();
    }
}
