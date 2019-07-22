using System;
using System.Collections.Generic;
using UnityEngine;
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
public struct Point :ICloneable
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
    public List<GameObject> Tiles = new List<GameObject>();
    public List<GameObject> RoadTiles = new List<GameObject>();
    public List<GameObject> Monsters = new List<GameObject>();
    public GameObject TestMon;

    public static readonly Vector2 START_POINT = new Vector2(-1.3f, 0);
    public static readonly Vector2 REVISE = new Vector2(-4f, -4f) + START_POINT;

    private GameBoard GameMap;
    private int Stage;
    private int Gold;
    private int BuiltTower;
    private List<GameObject> LiveMonsters=new List<GameObject>();

    private void MakeGameMap()
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
        GameMap = new GameBoard(path);
    }
    private void MakeMap()
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
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 0));
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 90f));
                continue;
            }
            var delta = next - prev;
            var same = mapPath.FindAll(p => p.Equals(cur));
            if (delta.x == 0)
            {
                if (same.Count > 1)
                {
                    Instantiate(RoadTiles[(int)ROAD_TYPE.CROSS], location, Quaternion.Euler(0, 0, 0));
                    continue;
                }
                Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 0));
                continue;
            }
            if (delta.y == 0)
            {
                if (same.Count > 1)
                    continue;
                Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], location, Quaternion.Euler(0, 0, 90f));
                continue;
            }
            var before = cur - prev;
            var after = next - cur;
            if (before.x == 1)
            {
                if (after.y == 1)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 0));
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 90f));
            }
            else if (before.x == -1)
            {
                if (after.y == 1)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 270f));
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 180f));
            }
            else
            {
                if (before.y == 1)
                {
                    if (after.x == 1)
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 180f));
                    else
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 90f));
                }
                else
                {
                    if (after.x == 1)
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 270f));
                    else
                        Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 0));
                }
            }
        }
        if ((cur - next).x == 0)
            Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], next.ToVector() + REVISE, Quaternion.Euler(0, 0, 0));
        else
            Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], next.ToVector() + REVISE, Quaternion.Euler(0, 0, 90f));
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (GameMap[i, j] != TILE_TYPE.ROAD)
                {
                    Instantiate(Tiles[(int)GameMap[i, j]], new Vector2(i,j) + REVISE, Quaternion.Euler(0, 0, 0));
                }
            }
        }
    }
    void Start()
    {
        Stage = 1;
        Gold = 10;
        BuiltTower = 0;
        MakeGameMap();
        MakeMap();
        var moncon = TestMon.GetComponent<MonsterController>();
        var start = GameMap.EntryAt(0).ToVector3();
        start.x -= 5.3f;
        start.y -= 4f;
        TestMon.transform.position = start;
        moncon.MovePath=GameMap.DefaultPath;
        moncon.StartCoroutine(moncon.coroutine());
        //coroutine으로 몬스터 스폰 시작
    }

    // Update is called once per frame
    void Update()
    {

    }
}
