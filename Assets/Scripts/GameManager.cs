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
    public Sprite[] emotesFace;
    public Sprite emoteBack;
    public GameObject[] emotes;
    public Text matches;
    public GameObject emotePrefab;
    public int boardSize = 20;

    private Sprite[] allEmotesSheet;
    private List<Sprite> emotesSprites;

    private LevelResult _levelResult;

    int boardCurrentXSpot = 0;
    int boardCurrentYSpot = 0;
    int boardBeginX = -250;
    int boardEndX = 260;
    int boardFirstLineY = 250;
    int boardSecondLineY = -90;

    private void Update()
    {
        //if (_init)
            //initializeCards();

        if (Input.GetMouseButtonUp(0))
            checkEmotes();
    }

    private void initializeCards()
    {
        for (int id = 0; id < 2; id++)
        {
            for (int i = 0; i < boardSize/2; i++)
            {
                bool test = false;
                int choice = 0;

                while (!test)
                {
                    choice = UnityEngine.Random.Range(0, emotes.Length);
                    test = !(emotes[choice].GetComponent<Emote>().initialized);
                }
                emotes[choice].GetComponent<Emote>().emoteValue = i;
                emotes[choice].GetComponent<Emote>().initialized = true;
            }

            foreach (GameObject c in emotes)
            {
                c.GetComponent<Emote>().setupGraphics();
            }

            if (!_init)
                _init = true;
        }
    }

    public Sprite getEmoteBack()
    {
        return emoteBack;
    }

    public Sprite getEmoteFace(int position)
    {
        return emotesFace[position];
    }

    void checkEmotes()
    {
        List<int> c = new List<int>();
        for (int i = 0; i < emotes.Length; i++)
        {
            if (emotes[i].GetComponent<Emote>().state == 1)
                c.Add(i);
        }

        if (c.Count == 2)
            emoteComparison(c);
    }

    private void emoteComparison(List<int> c)
    {
        Emote.freezed = true;
        int x = 0;

        Emote emote1 = emotes[c[0]].GetComponent<Emote>();
        Emote emote2 = emotes[c[1]].GetComponent<Emote>();

        if (emote1.emoteValue == emote2.emoteValue)
        {
            x = 2;
            _matchesGoal--;
            matches.text = "Matches Lefting: " + _matchesGoal;

            if (_matchesGoal == 9)
            {
                //Winner!
                StartCoroutine(SendScore());
            }
        }

        for (int i = 0; i < c.Count; i++)
        {
            emotes[c[i]].GetComponent<Emote>().state = x;
            emotes[c[i]].GetComponent<Emote>().falseCheck();
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
        return boardSize;
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

    // Use this for initialization
    void Start ()
    {

        allEmotesSheet = Resources.LoadAll<Sprite>("animals");

        setEmotesSprites(allEmotesSheet);
        setDefaultBackSprite();

        emotes = new GameObject[boardSize];
       
        boardCurrentXSpot = boardBeginX;
        boardCurrentYSpot = boardFirstLineY;
        for (int i = 0; i < boardSize; i++)
        {
            Vector3 currentPosition = getCoordinatesXY();
            emotes[i] = Instantiate(emotePrefab, Vector2.zero, Quaternion.identity);
            setPosition(emotes[i], currentPosition);
        }
        _matchesGoal = emotes.Length / 2;
        matches.text = "Matches Lefting: " + _matchesGoal;
        initializeCards();
    }

    private void setDefaultBackSprite()
    {
        emoteBack = allEmotesSheet[0];
    }

    private void setEmotesSprites(Sprite[] allEmotesSheet)
    {      
        emotesFace = new Sprite[boardSize/2];
        for (int i = 0; i < boardSize/2; i++)
        {
            int position = i + 1;
            emotesFace[i] = allEmotesSheet[position];
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
