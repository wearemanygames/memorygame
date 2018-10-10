using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {    
    private bool _init = false;
    private int _matchesGoal = 0;

    public GameObject winMenu;
    public Sprite[] memosFace;
    public GameObject[] memos;
    public Text matches;
    public GameObject memoPrefab;
    public int boardSize = 20;
    public GameObject clock;
    public string sheetToLoad;

    private LevelResult _levelResult;
    private Sprite memosBack;

    private int boardCurrentXSpot = 0;
    private int boardCurrentYSpot = 0;
    private int boardBeginX = -250;
    private int boardEndX = 260;
    private int boardFirstLineY = 250;

    private int winCondition = 0;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            checkMemos();
    }

    private void initializeMemos()
    {
        for (int id = 0; id < 2; id++)
        {
            for (int i = 0; i < boardSize/2; i++)
            {
                bool test = false;
                int choice = 0;

                while (!test)
                {
                    choice = UnityEngine.Random.Range(0, memos.Length);
                    test = !(memos[choice].GetComponent<Memo>().initialized);
                }
                memos[choice].GetComponent<Memo>().emoteValue = i;
                memos[choice].GetComponent<Memo>().initialized = true;
            }

            foreach (GameObject c in memos)
            {
                c.GetComponent<Memo>().setupGraphics();
            }

            if (!_init)
                _init = true;
        }
    }

    public Sprite getEmoteBack()
    {
        return memosBack;
    }

    public Sprite getEmoteFace(int position)
    {
        return memosFace[position];
    }

    void checkMemos()
    {
        List<int> c = new List<int>();
        for (int i = 0; i < memos.Length; i++)
        {
            if (memos[i].GetComponent<Memo>().state == 1)
            {
                c.Add(i);
                memos[i].GetComponent<Memo>().chosen = true;
            }
        }

        if (c.Count == 2)
            emoteComparison(c);
    }

    private void emoteComparison(List<int> c)
    {
        Memo.freezed = true;

        int x = 0;

        Memo memo1 = memos[c[0]].GetComponent<Memo>();
        Memo memo2 = memos[c[1]].GetComponent<Memo>();

        memo1.chosen = false;
        memo2.chosen = false;

        if (memo1.emoteValue == memo2.emoteValue)
        {
            x = 2;
            _matchesGoal--;
            matches.text = "Matches Lefting: " + _matchesGoal;

            if (_matchesGoal == winCondition)
            {
                //Winner!
                clock.GetComponent<Clock>().turnOff();
                StartCoroutine(SendScore());
            }
        }

        for (int i = 0; i < c.Count; i++)
        {
            memos[c[i]].GetComponent<Memo>().state = x;
            memos[c[i]].GetComponent<Memo>().falseCheck();
        }
    }

    private IEnumerator SendScore()
    {
        int score = ComputeScore();
        winMenu.SetActive(true);
        winMenu.transform.Find("FinalScore").GetComponent<Text>().text = "Score: " + score;

        string json = createResultJsonUsing(score);

        UnityWebRequest request = createApiRequest(json);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.responseCode);
        }
    }

    private int ComputeScore()
    {
        int timeScore = clock.GetComponent<Clock>().elapsedTime;
        return timeScore * boardSize;
    }

    private static UnityWebRequest createApiRequest(string json)
    {
        UnityWebRequest www = UnityWebRequest.Put("https://us-central1-huddle-team.cloudfunctions.net/api/memory/weberdls@gmail.com", json);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        return www;
    }

    private string createResultJsonUsing(int score)
    {
        _levelResult = new LevelResult(score, SceneManager.GetActiveScene().name);
        String json = JsonUtility.ToJson(_levelResult);
        return json;
    }

    // Board Setup
    void Start ()
    {
        Sprite[] allMemosSheet = loadAllSprites();

        setMemosSprites(allMemosSheet);
        setMemoDefaultBack(allMemosSheet);

        memos = new GameObject[boardSize];
        createAndPlotMemos();
        drawMatchesFeedbackOnScreen();
        initializeMemos();
    }

    private void drawMatchesFeedbackOnScreen()
    {
        _matchesGoal = memos.Length / 2;
        matches.text = "Matches Lefting: " + _matchesGoal;
    }

    private void createAndPlotMemos()
    {
        boardCurrentXSpot = boardBeginX;
        boardCurrentYSpot = boardFirstLineY;
        for (int i = 0; i < boardSize; i++)
        {
            Vector3 currentPosition = getCoordinatesXY();
            memos[i] = Instantiate(memoPrefab, Vector2.zero, Quaternion.identity);
            setPosition(memos[i], currentPosition);
        }
    }

    private Sprite[] loadAllSprites()
    {
        if (sheetToLoad == null || sheetToLoad == "")
            sheetToLoad = "animals";

        Sprite[] allMemosSheet;

        allMemosSheet = Resources.LoadAll<Sprite>(sheetToLoad);
        return allMemosSheet;
    }

    private void setMemoDefaultBack(Sprite[] allMemosSheet)
    {
        memosBack = allMemosSheet[0];
    }

    private void setMemosSprites(Sprite[] allMemosSheet)
    {      
        memosFace = new Sprite[boardSize/2];
        for (int i = 0; i < boardSize/2; i++)
        {
            int position = i + 1;
            memosFace[i] = allMemosSheet[position];
        }
    }

    private void setPosition(GameObject go, Vector3 vec3)
    {
        GameObject table = GameObject.Find("Board");
        go.transform.SetParent(table.transform);
        go.transform.position = table.transform.position;
        go.transform.localPosition = vec3;
    }

    private Vector3 getCoordinatesXY()
    {
        if (boardCurrentXSpot > boardEndX)
        {
            boardCurrentXSpot = boardBeginX;
            boardCurrentYSpot -= 125;
        }

        Vector3 vec = new Vector3(boardCurrentXSpot, boardCurrentYSpot);
        boardCurrentXSpot += 170;
        return vec;
    }
}
