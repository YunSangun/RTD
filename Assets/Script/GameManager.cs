using System.Collections;
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
public struct Point
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
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
        GameMap = new GameBoard(path, entry);
        Point prev = new Point();
        Point cur = new Point();
        Point next = new Point();
        for(int i = 0; i+1 < GameMap.Path.Count; ++i)
        {
            prev = cur;
            cur = GameMap.Path[i];
            next = GameMap.Path[i + 1];
            if (prev.x == 0 && prev.y == 0)
            {
                if(cur.x-next.x==0)
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT],new Vector3((float)cur.x-4, (float)cur.y - 4, 0)+START_POINT,Quaternion.Euler(0,0,0));
                else
                    Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], new Vector3((float)cur.x - 4, (float)cur.y - 4, 0) + START_POINT, Quaternion.Euler(0, 0, 90f));
                continue;
            }
            int d_x = next.x - prev.x;
            int d_y = next.y - prev.y;
            if (d_x == 0)
            {
                //겹치는곳 크로싱
                Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], new Vector3((float)cur.x - 4, (float)cur.y - 4, 0) + START_POINT, Quaternion.Euler(0, 0, 0));
                continue;
            }
            if (d_y == 0)
            {
                Instantiate(RoadTiles[(int)ROAD_TYPE.STRAIGHT], new Vector3((float)cur.x - 4, (float)cur.y - 4, 0) + START_POINT, Quaternion.Euler(0, 0, 90f));
                continue;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
