using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
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

[Serializable]public struct Point :ICloneable
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public object Clone()
    {
        return new Point(this.x, this.y);
    }
    public static Point operator+(Point p1, Point p2)
    {
        return new Point(p1.x + p2.x, p1.y + p2.y);

    }
    public static Point operator -(Point p)
    {
        return new Point(-p.x, -p.y);

    }
    public static Point operator -(Point p1,Point p2)
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
    public Vector2 ToVector()
    {
        return new Vector2(this.x, this.y);
    }
    public Vector3 ToVector3()
    {
        return new Vector3(this.x, this.y,0);
    }
}

public class GameManager : MonoBehaviour
{
    //외부 변수
    public UIManager GameUI;
    public GameObject BoardArea;
    public Texture btnTexture;
    public TowerManager[] TowerPrefabs;
    public GameObject[] TilePrefabs;
    public GameObject[] RoadTilePrefabs;
    public GameObject[] MonsterPrefabs;
    //
    //상수
    public static Vector2 START_POINT;
    public static Vector2 REVISE;
    public static readonly string PATH = "/Json/MapPath0.json";
    //
    //parent object
    private GameObject TileList;
    private GameObject TowerList;
    private GameObject MonsterList;
    //
    //내부 변수
    private GameBoard GameMap;
    private GameObject[,] Tiles=new GameObject[9,9];
    private int PlayerHP = 50;
    private int Stage = 1;
    private int Gold = 10;
    private int BuiltTower;
    private int RemainMonster;
    private bool beStarted = false;
    private bool pauseState = false;
    private List<GameObject> LiveMonsters=new List<GameObject>();

    private void MakeGameMap()//don't use
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
        var info = new MapInfo();
        info.path0 = path[0].ToArray();
        info.path1 = path[1].ToArray();
        info.path2 = path[2].ToArray();
        info.path3 = path[3].ToArray();
        var jsonstr = JsonUtility.ToJson(info);
        var file = new FileStream(Application.dataPath + PATH,FileMode.OpenOrCreate);
        var sw = new StreamWriter(file);
        sw.Write(jsonstr);
        sw.Close();
        GameMap = new GameBoard(path);
    }
    private void LoadMap()
    {
        var file = new FileStream(Application.dataPath + PATH, FileMode.Open);
        var sr = new StreamReader(file);
        var jsonstr = sr.ReadToEnd();
        sr.Close();
        var info = JsonUtility.FromJson<MapInfo>(jsonstr);
        var path = new List<Point>[4];
        path[0] = new List<Point>(info.path0);
        path[1] = new List<Point>(info.path1);
        path[2] = new List<Point>(info.path2);
        path[3] = new List<Point>(info.path3);
        GameMap = new GameBoard(path);
    }
    private void MakeBoard()
    {
        List<Point> mapPath=new List<Point>();
        foreach (var path in GameMap.DefaultPath)
            mapPath.AddRange(path);

        GameObject origin = RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT];
        Vector2 location=mapPath[0].ToVector()+REVISE;
        Quaternion rotate = Quaternion.Euler(0, 0, (mapPath[0].x == mapPath[1].x) ? 0 : 90f);
        Tiles[mapPath[0].x, mapPath[0].y] = Instantiate(origin,location,rotate, TileList.transform);//첫번째 경로타일

        for (int i = 1; i + 1 < mapPath.Count; ++i)
            //2~n-1 경로타일
        {
            var prev = mapPath[i-1];
            var cur = mapPath[i];
            var next = mapPath[i + 1];
            location = cur.ToVector()+REVISE;
            bool overlab = mapPath.FindAll(p => p.Equals(cur)).Count==2;
            bool straightY = (next - prev).x == 0 ;
            bool straightX = (next - prev).y == 0;
            bool beginX = (cur - prev).y == 0;
            if (overlab && straightX)
                continue;
            origin = overlab ? RoadTilePrefabs[(int)ROAD_TYPE.CROSS] :
                        straightX || straightY ? RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT] : RoadTilePrefabs[(int)ROAD_TYPE.TURN];
            float angle = 0f;
            angle = straightX ? 90f : straightY? 0:
                        beginX ? (float)45*(3- (cur - prev).x*((next - cur).y + 2)): (float)45 * (3 + (next - cur).x * (2- (cur - prev).y));
            rotate = Quaternion.Euler(0, 0, angle);

            Tiles[cur.x, cur.y] = Instantiate(origin, location, rotate, TileList.transform);

        }

        origin = RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT];
        location = mapPath[mapPath.Count - 1].ToVector() + REVISE;
        rotate = mapPath[mapPath.Count - 1].x == mapPath[mapPath.Count - 2].x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 90f);
        Tiles[mapPath[mapPath.Count - 1].x, mapPath[mapPath.Count - 1].y] = Instantiate(origin, location, rotate, TileList.transform);//마지막 경로 타일
       
        for (int i = 0; i < 9; ++i)//경로 외 속성타일
            for (int j = 0; j < 9; ++j)
                if (GameMap[i, j] != TILE_TYPE.ROAD)
                    Tiles[i,j]=Instantiate(TilePrefabs[(int)GameMap[i, j]], new Vector2(i,j) + REVISE, Quaternion.Euler(0, 0, 0), TileList.transform);
    }
    void Start()
    {
        START_POINT = BoardArea.transform.position;
        REVISE = START_POINT - new Vector2(4f,4f);
        Destroy(BoardArea);
        TileList = new GameObject() { name = "Tiles" };
        TowerList = new GameObject() { name = "Towers" };
        MonsterList = new GameObject() { name = "Monsters" };
        LoadMap();
        MakeBoard();
        GameUI.StartButton.onClick.AddListener(RoundStart);
        GameUI.AddTowerButton.onClick.AddListener(delegate { AddRandomTower(0); });
        GameUI.OptionButton.onClick.AddListener(SetPause);
    }
    public void RoundStart()
    {
        if (beStarted)
            return;
        beStarted = true;
        int count = 10;
        RemainMonster = count;
        StartCoroutine(SpanMonster(count));
    }
    public IEnumerator SpanMonster(int count)
    {
        yield return new WaitForSeconds(0.2f);
        for(int i = 0; i < count; ++i)
        {
            var monster = Instantiate<GameObject>(MonsterPrefabs[(int)MONSTER_TYPE.COMMON], GameMap.EntryAt(0).ToVector3() + (Vector3)REVISE, Quaternion.Euler(0, 0, 0), MonsterList.transform);
            var monsterController = monster.GetComponent<CommonMonsterController>();
            monsterController.SetStatus(100, 3, 1, GameMap.DefaultPath);
            monsterController.manager = this;
            StartCoroutine(monsterController.Move());
            yield return new WaitForSeconds(0.3f);
        }
    }

    void Update()
    {

    }

    void OnGUI()
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

    public void MonsterArrive(int attack)
    {
        PlayerHP -= attack;
        GameUI.LifeText.text = $"{PlayerHP}";
        if (--RemainMonster == 0)
            RoundEnd();
    }
    public void RoundEnd()
    {
        beStarted = false;
        ++Stage;
        GameUI.RoundText.text = $"ROUND {Stage:D3}";

    }
    public void AddRandomTower(int tier, float x = 0, float y = 0)
    {
        int beg = 0, end = 0;

        if (tier == 0)
        {
            beg = 0;
            end = 5;
        }
        else if (tier == 1)
        {
            beg = 5;
            end = 10;
        }
        else if (tier == 2)
        {
            beg = 10;
            end = 15;
        }

        TowerManager tw = Instantiate(TowerPrefabs[Random.Range(beg, end)]) as TowerManager;
        tw.transform.localPosition = new Vector2(x, y);
        //tw.transform.localPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
}
