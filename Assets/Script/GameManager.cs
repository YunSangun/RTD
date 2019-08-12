using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum MONSTER_TYPE
{
    COMMON
}
public enum TOWER_TYPE
{
    NONE,
    FIRE,
    ICE,
    POISON,
    IRON,
    WIND
}
public enum TILE_TYPE
{
    NONE,
    FIRE,
    ICE,
    POISON,
    IRON,
    WIND,
    ROAD

}
public enum ROAD_TYPE
{
    STRAIGHT,
    TURN,
    CROSS
}

[Serializable]
public struct Point//(x,y)구조체
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Point(Point p)
    {
        this.x = p.x;
        this.y = p.y;
    }
    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.x + p2.x, p1.y + p2.y);

    }
    public static Point operator -(Point p)
    {
        return new Point(-p.x, -p.y);

    }
    public static Point operator -(Point p1, Point p2)
    {
        return p1 + (-p2);

    }
    public static Point operator *(Point p, float f)
    {
        Point tmp;
        tmp.x = (int)(p.x * f);
        tmp.y = (int)(p.y * f);
        return tmp;

    }
    public static Point operator /(Point p, float f)
    {
        Point tmp;
        tmp.x = (int)(p.x / f);
        tmp.y = (int)(p.y / f);
        return tmp;

    }
    public bool Equals(Point p)
    {
        return this.x == p.x && this.y == p.y;
    }
    public override String ToString()
    {
        return $"({x},{y})";
    }
    public Vector2 ToVector()
    {
        return new Vector2(this.x, this.y);
    }
    public Vector3 ToVector3()
    {
        return new Vector3(this.x, this.y, 0);
    }
    public bool Inside(Point LeftTop, Point RightBottom)
    {
        return LeftTop.x <= x && x <= RightBottom.x &&
               LeftTop.y <= y && y <= RightBottom.y;
    }
}

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
    public GameObject BoardArea;
    public GameObject SelectMask;
    public GameObject EntryMark;
    public TowerManager[] TowerPrefabs;
    public GameObject[] TilePrefabs;
    public GameObject[] RoadTilePrefabs;
    public GameObject[] MonsterPrefabs;

    //상수
    public static Vector2 START_POINT; // 보드의 중심
    public static Vector2 REVISE;      // 0,0 타일의 좌표
    public static readonly string PATH = "/Json/MapPath0.json";

    //parent object
    public GameObject TileList { get; set; }
    public GameObject TowerList { get; set; }
    public GameObject MonsterList { get; set; }

    //내부 변수
    private GameBoard GameMap;
    private TileController[,] Tiles = new TileController[9, 9];
    private TileController SelectedTile = null;
    private GameObject entry;
    private GameObject exit;
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
        //parent 객체 설정
        Destroy(BoardArea);
        TileList = new GameObject() { name = "Tiles" };
        TowerList = new GameObject() { name = "Towers" };
        MonsterList = new GameObject() { name = "Monsters" };
        //버튼 이벤트 할당
        UIManager.Inst.StartButton.onClick.AddListener(RoundStart);
        UIManager.Inst.AddTowerButton.onClick.AddListener(delegate { AddRandomTower(1); });
        UIManager.Inst.OptionButton.onClick.AddListener(SetPause);
        //내부변수 할당
        Monsters = new List<MonsterController>();
        START_POINT = BoardArea.transform.position; //중점 설정
        GameManager.REVISE = START_POINT - new Vector2(4f, 4f); //0,0 설정
        PlayerHP = 50;
        Round = 1;
        Gold = 1000;
        LoadMap();
        MakeBoard();
        SetRandomPath();
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(pos, Vector2.zero, 0f);
            if (hit.collider != null)
            {
                var obj = hit.collider.gameObject;
                if (obj.CompareTag("Tile"))
                    TileSelect(obj.GetComponent<TileController>());
            }
        }
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
        GameMap = new GameBoard(path);
    }
    private void LoadMap()
    {
        var file = new FileStream(Application.dataPath + GameManager.PATH, FileMode.Open);
        var sr = new StreamReader(file);
        var jsonstr = sr.ReadToEnd();
        sr.Close();
        var info = JsonUtility.FromJson<MapFileInfo>(jsonstr);
        GameMap = new GameBoard(new MapInfo(info));
    }
    private void MakeBoard()
    {
        //첫번째 경로 타일
        List<Point> mapPath = GameMap.ToList();
        GameObject origin = RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT];
        Vector2 location = mapPath[0].ToVector() + GameManager.REVISE;
        Quaternion rotate = Quaternion.Euler(0, 0, (mapPath[0].x == mapPath[1].x) ? 0 : 90f);
        var tc = Instantiate(origin, location, rotate, TileList.transform).GetComponent<TileController>();
        Tiles[mapPath[0].x, mapPath[0].y] = tc;
        tc.SetStatus(TILE_TYPE.ROAD, mapPath[0]);
        //
        //2~n-1 경로타일
        for (int i = 1; i + 1 < mapPath.Count; ++i)
        {
            var prev = mapPath[i - 1];
            var cur = mapPath[i];
            var next = mapPath[i + 1];
            location = cur.ToVector() + GameManager.REVISE;
            bool overlab = mapPath.FindAll(p => p.Equals(cur)).Count == 2;
            bool straightY = (next - prev).x == 0;
            bool straightX = (next - prev).y == 0;
            bool beginX = (cur - prev).y == 0;
            if (overlab && straightX)
                continue;
            origin = overlab ? RoadTilePrefabs[(int)ROAD_TYPE.CROSS] :
                        straightX || straightY ? RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT] : RoadTilePrefabs[(int)ROAD_TYPE.TURN];
            float angle = 0f;
            angle = straightX ? 90f : straightY ? 0 :
                        beginX ? (float)45 * (3 - (cur - prev).x * ((next - cur).y + 2)) : (float)45 * (3 + (next - cur).x * (2 - (cur - prev).y));
            rotate = Quaternion.Euler(0, 0, angle);

            Tiles[cur.x, cur.y] = Instantiate(origin, location, rotate, TileList.transform).GetComponent<TileController>();
            Tiles[cur.x, cur.y].SetStatus(TILE_TYPE.ROAD, cur);

        }
        //
        //마지막 경로 타일
        origin = RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT];
        location = mapPath[mapPath.Count - 1].ToVector() + GameManager.REVISE;
        rotate = mapPath[mapPath.Count - 1].x == mapPath[mapPath.Count - 2].x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 90f);
        Tiles[mapPath[mapPath.Count - 1].x, mapPath[mapPath.Count - 1].y] = Instantiate(origin, location, rotate, TileList.transform).GetComponent<TileController>();
        Tiles[mapPath[mapPath.Count - 1].x, mapPath[mapPath.Count - 1].y].SetStatus(TILE_TYPE.ROAD, mapPath[mapPath.Count - 1]);
        //
        //경로 외 속성타일
        for (int i = 0; i < 9; ++i)
            for (int j = 0; j < 9; ++j)
                if (GameMap[i, j] != TILE_TYPE.ROAD)
                {
                    Tiles[i, j] = Instantiate(TilePrefabs[(int)GameMap[i, j]], new Vector2(i, j) + GameManager.REVISE, Quaternion.Euler(0, 0, 0), TileList.transform).GetComponent<TileController>();
                    Tiles[i, j].SetStatus(GameMap[i, j], new Point(i, j));
                }
        //
    }
    private void SpanMonster()
    {
        //몬스터 생성 후 정보 할당
        var mt = Instantiate<GameObject>(MonsterPrefabs[(int)MONSTER_TYPE.COMMON], GameMap.EntryAt(0).ToVector3() + (Vector3)GameManager.REVISE, Quaternion.Euler(0, 0, 0), MonsterList.transform)
        .GetComponent<CommonMonsterController>();
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
        var entryTile = Tiles[entry.x, entry.y];
        var exitTile = Tiles[exit.x, exit.y];
        float angle = (entry.y == 0) ? 0f :
                      (entry.y == 8) ? 180f :
                      (entry.x == 0) ? 270f :
                                       90f;
        if (this.entry != null)
            Destroy(this.entry);
        this.entry = Instantiate(EntryMark, entryTile.transform.position, Quaternion.Euler(0,0,angle), entryTile.transform);
        if (this.exit != null)
            Destroy(this.exit);
        this.exit = Instantiate(EntryMark, exitTile.transform.position, Quaternion.Euler(0,0,angle), exitTile.transform);
    }
    //외부 함수
    public void TileSelect(TileController tc)
    {

        if (SelectedTile != null)
            SelectedTile.Selected = false;
        if (tc.Type != TILE_TYPE.ROAD)
        {
            if (!tc.Selected)
            {
                tc.Selected = true;
                SelectedTile = tc;
            }
        }
    }
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
    public void AddRandomTower(int tier)
    {
        if (SelectedTile == null)
            return;
        if (SelectedTile.BuiltTower != null)
            return;
        if (Gold < 10)
            return;
        Gold -= 10;
        TowerManager tw = Instantiate(TowerPrefabs[Random.Range(0, 5)], TowerList.transform) as TowerManager;
        SelectedTile.BuiltTower = tw;
        tw.transform.position = SelectedTile.transform.position;
        tw.SetStatus(1, 1, tier, 1f, SelectedTile);
    }

    public void AddRewardGold(int reward)
    {
        Gold += reward;
    }
}
