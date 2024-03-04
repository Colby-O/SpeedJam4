using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TMP_Text _timerText;

    private float _time;
    
    public float GetTimeInSeconds()
    {
        return _time;
    }

    public void ResetTimer()
    {
        _time = 0.0f;
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Awake()
    {
        _timerText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        DisplayTime(_time);
    }
}
