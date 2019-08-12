using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //singeton
    private static GameManager inst;
    public static GameManager Inst
    {
        get
        {
            return GameManager.inst;

        }
    }

    //외부 변수

    public BoardManager BM;

    public static readonly string PATH = "/Json/MapPath0.json";


    //내부 변수
    private GameBoard gameMap;
    private int playerHP;
    private int round;
    private int gold;
    private int BuiltTowers;   //설치된 타워 수
    private int RemainMonster; //보드에 남은 몬스터
    private int RemainSpan;    //남은 몬스터 스폰 횟수
    private float SpanTime = 0f;       //스폰 시간
    private float IntervalSpan = 0.3f; //스폰 간격
    private bool started = false;
    private bool pauseState = false;
    private List<Point> roundPath;     //이번 라운드의 몬스터 진행 경로

    //외부 속성
    public List<MonsterController> Monsters { get; set; }
    public GameBoard GameMap { get { return gameMap; } }

    //내부 속성
    private int Gold
    {
        get
        {
            return this.gold;
        }
        set
        {
            this.gold = value;
            UIManager.Inst.GoldText.text = $"GOLD : {this.gold:D8}";
        }
    }
    private int Round
    {
        get
        {
            return this.round;
        }
        set
        {
            this.round = value;
            UIManager.Inst.RoundText.text = $"ROUND {this.round:D3}";
        }
    }
    private int PlayerHP
    {
        get
        {
            return this.playerHP;
        }
        set
        {
            this.playerHP = value;
            UIManager.Inst.LifeText.text = $"{this.playerHP:D2}";
        }
    }

    //유니티 이벤트

    private void Awake()
    {
        GameManager.inst = this;//싱글톤 초기화
    }
    private void Start()
    {
        //내부변수 할당
        Monsters = new List<MonsterController>();
        PlayerHP = 50;
        Round = 1;
        Gold = 1000;
        LoadMap();
        BM.MakeBoard();
        SetRandomPath();
        
    }

    private void FixedUpdate()
    {
        //IntervalSpan 마다 몬스터 스폰
        if (started && RemainSpan != 0)
        {
            SpanTime += Time.deltaTime;
            if (SpanTime >= IntervalSpan)
            {
                --RemainSpan;
                SpanTime -= IntervalSpan;
                SpanMonster();
            }
        }
        //
    }
    //void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus)
    //    {
    //        isPaused = true;
    //    }
    //    else
    //    {
    //        if (isPaused)
    //        {
    //            isPaused = false;
    //        }
    //    }
    //}
    private void OnGUI()
    {
        //if (!btnTexture)
        //{
        //    Debug.Log("Check btnTexture");
        //    return;
        //}

        //if (GUI.Button(new Rect(100, 100, 100, 100), btnTexture, "Add Tower"))
        //{
        //    addTower();
        //}
        //if (GUI.Button(new Rect(0, 0, 100, 100), "Add Tower"))
        //{
        //    AddRandomTower(0);
        //}

        //if (isPaused)
        //    GUI.Label(new Rect(100, 100, 50, 30), "Game paused");

        // 테스트용
        if (GUI.Button(new Rect(550, 15, 50, 30), "Restart"))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
    //내부 함수
    private void MakeGameMap()//not use
    {
        var path = new List<Point>[4];
        for (int i = 0; i < 4; ++i)
            path[i] = new List<Point>();
        path[0].Add(new Point(2, 0));
        path[0].Add(new Point(2, 1));
        path[0].Add(new Point(3, 1));
        path[0].Add(new Point(4, 1));
        path[0].Add(new Point(5, 1));
        path[0].Add(new Point(6, 1));
        path[0].Add(new Point(6, 0));

        path[1].Add(new Point(6, 8));
        path[1].Add(new Point(6, 7));
        path[1].Add(new Point(6, 6));
        path[1].Add(new Point(7, 6));
        path[1].Add(new Point(8, 6));

        path[2].Add(new Point(0, 6));
        path[2].Add(new Point(1, 6));
        path[2].Add(new Point(2, 6));
        path[2].Add(new Point(3, 6));
        path[2].Add(new Point(4, 6));
        path[2].Add(new Point(4, 5));
        path[2].Add(new Point(4, 4));
        path[2].Add(new Point(5, 4));
        path[2].Add(new Point(6, 4));
        path[2].Add(new Point(7, 4));
        path[2].Add(new Point(7, 3));
        path[2].Add(new Point(7, 2));
        path[2].Add(new Point(8, 2));

        path[3].Add(new Point(0, 2));
        path[3].Add(new Point(1, 2));
        path[3].Add(new Point(2, 2));
        path[3].Add(new Point(2, 3));
        path[3].Add(new Point(2, 4));
        path[3].Add(new Point(2, 5));
        path[3].Add(new Point(2, 6));
        path[3].Add(new Point(2, 7));
        path[3].Add(new Point(2, 8));
        var info = new MapFileInfo();
        info.path0 = path[0].ToArray();
        info.path1 = path[1].ToArray();
        info.path2 = path[2].ToArray();
        info.path3 = path[3].ToArray();
        var jsonstr = JsonUtility.ToJson(info);
        var file = new FileStream(Application.dataPath + GameManager.PATH, FileMode.OpenOrCreate);
        var sw = new StreamWriter(file);
        sw.Write(jsonstr);
        sw.Close();
        gameMap = new GameBoard(path);
    }
    private void LoadMap()
    {
        var file = new FileStream(Application.dataPath + GameManager.PATH, FileMode.Open);
        var sr = new StreamReader(file);
        var jsonstr = sr.ReadToEnd();
        sr.Close();
        var info = JsonUtility.FromJson<MapFileInfo>(jsonstr);
        gameMap = new GameBoard(new MapInfo(info));
    }
    private void SpanMonster()
    {
        //몬스터 생성 후 정보 할당
        var mt = BM.CreateMonster(MONSTER_TYPE.COMMON, roundPath[0]);
        int hp = 5;
        int speed = 3;
        int attack = 1;
        int reward = 1;
        mt.SetStatus(hp, speed, attack, reward, roundPath);
        Monsters.Add(mt);
        //
    }
    private void SetRandomPath()
    {
        int entryIndex = Random.Range(0, 8);
        roundPath = GameMap.PathAt(entryIndex);
        var entry = roundPath[1];
        var exit = roundPath[roundPath.Count - 2];
        BM.DisplayEntryMark(entry, exit);
    }
    
    //외부 함수

    public void SetPause()
    {
        if (!pauseState)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        pauseState ^= true;
    }
    public void MonsterArrive(int attack)
    {
        PlayerHP -= attack;
        if (--RemainMonster == 0)
            RoundEnd();
    }
    public void RoundStart()
    {
        if (started) return; //라운드가 진행중이면 종료
        //라운드 정보 할당
        started = true;
        int count = 10;
        RemainSpan = count;
        RemainMonster = count;
        SpanTime = 0;
        //
    }
    public void RoundEnd()
    {
        started = false;
        ++Round;
        SetRandomPath();
    }
    public bool BoughtTower(int price)
    {
        if (Gold < price)
            return false;
        Gold -= price;
        return true;
    }
 

    public void AddRewardGold(int reward)
    {
        Gold += reward;
    }
}
