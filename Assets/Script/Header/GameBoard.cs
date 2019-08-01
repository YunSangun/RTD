using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Random = UnityEngine.Random;

[Serializable]
public class MapFileInfo
{
    public Point[] path0;
    public Point[] path1;
    public Point[] path2;
    public Point[] path3;
}
public class MapInfo
{
    public List<Point>[] path;
    public List<Point> entry;
    public MapInfo()
    {
        this.path = new List<Point>[4];
        this.entry = new List<Point>();
        for (int i = 0; i < 4; ++i)
            this.path[i] = new List<Point>();
    }
    public MapInfo(MapInfo map)
    {
        this.path = (List<Point>[])map.path.Clone();
        this.entry = new List<Point>(map.entry);
    }
    public MapInfo(List<Point>[] path)
    {
        this.path = new List<Point>[4];
        this.entry = new List<Point>();
        for (int i = 0; i < 4; ++i)
        {
            this.entry.Add(path[i][0]);
            this.entry.Add(path[i][path[i].Count - 1]);
            this.path[i] = new List<Point>(path[i]);
        }

    }
    public MapInfo(MapFileInfo file)
    {
        this.path = new List<Point>[4];
        this.entry = new List<Point>();
        var path = file.path0.ToList();
        this.entry.Add(path[0]);
        this.entry.Add(path[path.Count - 1]);
        this.path[0] = path;
        path = file.path1.ToList();
        this.entry.Add(path[0]);
        this.entry.Add(path[path.Count - 1]);
        this.path[1] = path;
        path = file.path2.ToList();
        this.entry.Add(path[0]);
        this.entry.Add(path[path.Count - 1]);
        this.path[2] = path;
        path = file.path3.ToList();
        this.entry.Add(path[0]);
        this.entry.Add(path[path.Count - 1]);
        this.path[3] = path;
    }
    public List<Point> ToList()
    {
        var tmp = new List<Point>();
        for (int i = 0; i < 4; ++i)
            tmp.AddRange(path[i]);
        return tmp;
    }
    

}

public class GameBoard
{
    private TILE_TYPE[,] board;
    private MapInfo map;
    public TILE_TYPE this[int x, int y]
    {
        get
        {
            return board[x, y];
        }
    }
    public List<Point>[] DefaultPath
    {
        get
        {
            var tmp = new List<Point>[4];
            for (int i = 0; i < 4; ++i)
                tmp[i] = new List<Point>(map.path[i]);
            return tmp;
        }
    }
    public GameBoard(MapInfo info)
    {
        this.board = new TILE_TYPE[9, 9];
        this.map = info;
        foreach (var p in this.map.ToList())
            this.board[p.x, p.y] = TILE_TYPE.ROAD;
        for (int i = 0; i < 9; ++i)
            for (int j = 0; j < 9; ++j)
                if (this.board[i, j] == TILE_TYPE.NONE)
                    this.board[i, j] = (TILE_TYPE)Random.Range(1, 6);
    }
    public GameBoard(List<Point>[] path) : this(new MapInfo(path)) { }
    public GameBoard(TILE_TYPE[,] board, MapInfo map)
    {
        this.board = (TILE_TYPE[,])board.Clone();
        this.map = new MapInfo(map);
    }
    public int EntryIndexOf(Point p)
    {
        return this.map.entry.FindIndex(e => e.Equals(p));
    }
    public Point EntryAt(int n)
    {
        if (n > 7)
            return new Point(-1, -1);
        return this.map.entry[n];
    }
    public List<Point> PathAt(int n)
    //n번째 entry에서 출발한 경로를 반환, n이 8이상이면 null
    {
        var tmp = new List<Point>();
        if (n > 7)
            return null;
        if (n % 2 == 0)
        {
            for (int i = 0; i < 4; ++i)
                tmp.AddRange(map.path[(n / 2 + i) % 4]);
        }
        else
        {
            for (int i = 0; i < 4; ++i)
            {
                var r = new List<Point>(map.path[(4 + n / 2 - i) % 4]);
                r.Reverse();
                tmp.AddRange(r);
            }
        }
        if (tmp[0].x == 0)
        {
            tmp.Insert(0, new Point(-1, tmp[0].y));
            tmp.Add(new Point(9, tmp[1].y));
        }
        else if (tmp[0].x == 8)
        {
            tmp.Insert(0, new Point(9, tmp[0].y));
            tmp.Add(new Point(-1, tmp[1].y));
        }
        else if (tmp[0].y == 0)
        {
            tmp.Insert(0, new Point(tmp[0].x, -1));
            tmp.Add(new Point(tmp[1].x, 9));
        }
        else
        {
            tmp.Insert(0, new Point(tmp[0].x, 9));
            tmp.Add(new Point(tmp[1].x, -1));
        }
        return tmp;
    }
}
