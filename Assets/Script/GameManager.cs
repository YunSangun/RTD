using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public bool Equals(Point p)
    {
        return this.x == p.x && this.y == p.y;
    }
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> Tiles = new List<GameObject>();
    public List<GameObject> RoadTiles = new List<GameObject>();
    private readonly Vector3 START_POINT = new Vector3(-1.3f, 0, 0);
    private GameBoard GameMap;
    // Start is called before the first frame update
    void Start()
    {
        var path = new List<Point>();
        var entry = new List<Point>();
        {
            entry.Add(new Point(2, 0));
            entry.Add(new Point(2, 8));
            entry.Add(new Point(6, 8));
            entry.Add(new Point(6, 0));
            entry.Add(new Point(0, 2));
            entry.Add(new Point(8, 2));
            entry.Add(new Point(0, 6));
            entry.Add(new Point(8, 6));
            path.Add(new Point(2, 0));
            path.Add(new Point(2, 1));
            path.Add(new Point(3, 1));
            path.Add(new Point(4, 1));
            path.Add(new Point(5, 1));
            path.Add(new Point(6, 1));
            path.Add(new Point(6, 0));
            path.Add(new Point(6, 8));
            path.Add(new Point(6, 7));
            path.Add(new Point(6, 6));
            path.Add(new Point(7, 6));
            path.Add(new Point(8, 6));
            path.Add(new Point(0, 6));
            path.Add(new Point(1, 6));
            path.Add(new Point(2, 6));
            path.Add(new Point(3, 6));
            path.Add(new Point(4, 6));
            path.Add(new Point(4, 5));
            path.Add(new Point(4, 4));
            path.Add(new Point(5, 4));
            path.Add(new Point(6, 4));
            path.Add(new Point(7, 4));
            path.Add(new Point(7, 3));
            path.Add(new Point(7, 2));
            path.Add(new Point(8, 2));
            path.Add(new Point(0, 2));
            path.Add(new Point(1, 2));
            path.Add(new Point(2, 2));
            path.Add(new Point(2, 3));
            path.Add(new Point(2, 4));
            path.Add(new Point(2, 5));
            path.Add(new Point(2, 6));
            path.Add(new Point(2, 7));
            path.Add(new Point(2, 8));
        }
        GameMap = new GameBoard(path, entry);
        Point prev = new Point();
        Point cur = new Point();
        Point next = new Point();
        var mapPath = GameMap.Path;
        for (int i = 0; i+1 < mapPath.Count; ++i)
        {
            prev = cur;
            cur = mapPath[i];
            next = mapPath[i + 1];
            var location = new Vector3((float)cur.x - 4, (float)cur.y - 4, 0) + START_POINT;
            if (prev.x == 0 && prev.y == 0)
            {
                if(cur.x-next.x==0)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT],location,Quaternion.Euler(0,0,0));
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
                if(after.y==1)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 0));
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.TURN], location, Quaternion.Euler(0, 0, 90f));
            }
            else if(before.x==-1)
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
        if((cur-next).x==0)
            Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], new Vector3((float)next.x - 4, (float)next.y - 4, 0) + START_POINT, Quaternion.Euler(0, 0, 0));
        else
            Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], new Vector3((float)next.x - 4, (float)next.y - 4, 0) + START_POINT, Quaternion.Euler(0, 0, 90f));
        for(int i = 0; i < 9; ++i)
        {
            for(int j = 0; j < 9; ++j)
            {
                if (GameMap[i, j] != TILE_TYPE.ROAD)
                {
                    Instantiate(Tiles[(int)GameMap[i,j]], new Vector3((float)i - 4, (float)j - 4, 0) + START_POINT, Quaternion.Euler(0, 0, 0));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
