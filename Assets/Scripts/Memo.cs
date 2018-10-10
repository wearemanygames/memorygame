using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Memo : MonoBehaviour {

    public static bool freezed = false;
    public bool chosen = false;

    private int _state;
    private int _memoValue;
    private bool _initialized;

    private Sprite _memoBack;
    private Sprite _memoFace;

    public int state
    {
        get {return _state;}
        set {_state = value;}
    }

    public int memoValue
    {
        get {return _memoValue;}
        set {_memoValue = value;}
    }

    public bool initialized
    {
        get { return _initialized; }
        set { _initialized = value; }
    }

    void Start()
    {
        _state = 0;
    }

    public void setupGraphics(GameManager manager)
    {
        _memoBack = manager.getEmoteBack();
        _memoFace = manager.getEmoteFace(_memoValue);

        flipMemo();
    }

    public void flipMemo()
    {
        if (!freezed && !chosen)
        {
            if (_state == 0)
                _state = 1;
            else if (_state == 1)
                _state = 0;

            if (_state == 0 && !freezed)
                GetComponent<Image>().sprite = _memoBack;
            else if (_state == 1 && !freezed)
                GetComponent<Image>().sprite = _memoFace;
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
            GetComponent<Image>().sprite = _memoBack;
        else if (_state == 1)
            GetComponent<Image>().sprite = _memoFace;
        freezed = false;
    }
}
