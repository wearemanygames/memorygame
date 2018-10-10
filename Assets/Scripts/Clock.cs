using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour {

    private float _timeInMillis = 0.0f;
    [SerializeField]
    private Text _timeText;
    private int _timeInSeconds = 1;
    private bool _isOn = true;

    public int elapsedTime
    { get { return _timeInSeconds; } }


    public void turnOff()
    {
        _isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOn)
        {
            if (_timeInMillis >= _timeInSeconds) //update text counter per second, not per frame
            {
                _timeText.text = "Elapsed Time: " + _timeInSeconds;
                _timeInSeconds++;
            }
            _timeInMillis += Time.deltaTime;
        }
    }
}
