using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _pressed;
    [SerializeField] private GameObject _unpressed;

    [SerializeField] private List<HiddenObject> _hiddenObjects;

    public bool IsPressed = false;

    public void Press()
    {
        IsPressed = true;
        _unpressed.SetActive(false);
        _pressed.SetActive(true);

        foreach (HiddenObject _obj in _hiddenObjects)
        {
            _obj.Show();
        }
    }

    public void Unpress()
    {
        IsPressed = false;
        _unpressed.SetActive(true);
        _pressed.SetActive(false);

        foreach (HiddenObject _obj in _hiddenObjects)
        {
            _obj.Hide();
        }
    }

    void Start()
    {
        Unpress();
    }
}
