using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private GameObject _openDoor;
    [SerializeField] private GameObject _closeDoor;

    public bool IsOpen;

    public void Open()
    {
        IsOpen = true;
        _closeDoor.SetActive(true);
        _openDoor.SetActive(false);
    }

    public void Close()
    {
        IsOpen = false;
        _closeDoor.SetActive(false);
        _openDoor.SetActive(true);
    }

    void Start()
    {
        Close();
    }
}
