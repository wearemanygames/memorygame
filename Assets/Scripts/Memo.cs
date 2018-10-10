using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Memo : MonoBehaviour {

    public static bool freezed = false;

    private int _state;
    private int _emoteValue;
    private bool _initialized;

    private Sprite _emoteBack;
    private Sprite _emoteFace;

    private static GameObject _manager;

    public int state
    {
        get {return _state;}
        set {_state = value;}
    }

    public int emoteValue
    {
        get {return _emoteValue;}
        set {_emoteValue = value;}
    }

    public bool initialized
    {
        get { return _initialized; }
        set { _initialized = value; }
    }

    void Start()
    {
        _state = 0;
        _manager = GameObject.FindGameObjectWithTag("Manager");
    }

    public void setupGraphics()
    {
        _emoteBack = _manager.GetComponent<GameManager>().getEmoteBack();
        _emoteFace = _manager.GetComponent<GameManager>().getEmoteFace(_emoteValue);

        flipEmote();
    }

    public void flipEmote()
    {
        if (!freezed)
        {
            if (_state == 0)
                _state = 1;
            else if (_state == 1)
                _state = 0;

            if (_state == 0 && !freezed)
                GetComponent<Image>().sprite = _emoteBack;
            else if (_state == 1 && !freezed)
                GetComponent<Image>().sprite = _emoteFace;
        }
    }

    public void falseCheck()
    {
        StartCoroutine(pause());
    }

    private IEnumerator pause()
    {
        yield return new WaitForSeconds(1);
        if (_state == 0)
            GetComponent<Image>().sprite = _emoteBack;
        else if (_state == 1)
            GetComponent<Image>().sprite = _emoteFace;
        freezed = false;
    }
}
