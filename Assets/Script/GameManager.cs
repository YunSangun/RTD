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
public enum ELEMENT_TYPE
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
    public UIManager GameUI;
    public GameObject BoardArea;
    public Texture btnTexture;
    public TowerManager[] towers;
    public GameObject[] Tiles;
    public GameObject[] RoadTiles;
    public GameObject[] Monsters;

    public static Vector2 START_POINT;
    public static Vector2 REVISE;
    public static readonly string PATH = "/Json/MapPath0.json";

    private GameBoard GameMap;
    private GameObject Board;
    private GameObject TowerList;
    private GameObject MonsterList;
    private int PlayerHP = 50;
    private int Stage=1;
    private int Gold=10;
    private int BuiltTower;
    private int RemainMonster;
    private bool beStarted=false;
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
        Point prev = new Point();
        Point cur = new Point();
        Point next = new Point();
        List<Point> mapPath=new List<Point>();
        foreach (var path in GameMap.DefaultPath)
            mapPath.AddRange(path);
        for (int i = 0; i + 1 < mapPath.Count; ++i)
        {
            prev = cur;
            cur = mapPath[i];
            next = mapPath[i + 1];
            var location = cur.ToVector()+REVISE;
            if (prev.x == 0 && prev.y == 0)
            {
                if (cur.x - next.x == 0)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 0),Board.transform);
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 90f), Board.transform);
                continue;
            }
            var delta = next - prev;
            var same = mapPath.FindAll(p => p.Equals(cur));
            if (delta.x == 0)
            {
                if (same.Count > 1)
                {
                    Instantiate(RoadTiles[(int)ROAD_TYPE.CROSS], location, Quaternion.Euler(0, 0, 0), Board.transform);
                    continue;
                }
                Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 0), Board.transform);
                continue;
            }
            if (delta.y == 0)
            {
                if (same.Count > 1)
                    continue;
                Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 90f), Board.transform);
                continue;
            }
            var before = cur - prev;
            var after = next - cur;
            if (before.x == 1)
            {
                if (after.y == 1)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 0), Board.transform);
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 90f), Board.transform);
            }
            else if (before.x == -1)
            {
                if (after.y == 1)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 270f), Board.transform);
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 180f), Board.transform);
            }
            else
            {
                if (before.y == 1)
                {
                    if (after.x == 1)
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 180f), Board.transform);
                    else
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 90f), Board.transform);
                }
                else
                {
                    if (after.x == 1)
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 270f), Board.transform);
                    else
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 0), Board.transform);
                }
            }
        }
        if ((cur - next).x == 0)
            Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], next.ToVector() + REVISE, Quaternion.Euler(0, 0, 0), Board.transform);
        else
            Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], next.ToVector() + REVISE, Quaternion.Euler(0, 0, 90f), Board.transform);
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (GameMap[i, j] != TILE_TYPE.ROAD)
                {
                    Instantiate(Tiles[(int)GameMap[i, j]], new Vector2(i,j) + REVISE, Quaternion.Euler(0, 0, 0), Board.transform);
                }
            }
        }
    }
    void Start()
    {
        START_POINT = BoardArea.transform.position;
        REVISE = START_POINT - new Vector2(4f,4f);
        Destroy(BoardArea);
        Board = new GameObject() { name = "Tiles" };
        TowerList = new GameObject() { name = "Towers" };
        MonsterList = new GameObject() { name = "Monsters" };
        LoadMap();
        MakeBoard();
        GameUI.StartButton.onClick.AddListener(RoundStart);
        GameUI.AddTowerButton.onClick.AddListener(delegate { AddRandomTower(0); });
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
            var monster = Instantiate<GameObject>(Monsters[(int)MONSTER_TYPE.COMMON], GameMap.EntryAt(0).ToVector3() + (Vector3)REVISE, Quaternion.Euler(0, 0, 0), MonsterList.transform);
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
    }

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

        TowerManager tw = Instantiate(towers[Random.Range(beg, end)]) as TowerManager;
        tw.transform.localPosition = new Vector2(x, y);
        //tw.transform.localPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
}
