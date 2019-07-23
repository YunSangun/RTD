using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Random = UnityEngine.Random;

[Serializable]
public class MapInfo
{
    public Point[] path0;
    public Point[] path1;
    public Point[] path2;
    public Point[] path3;
}
public class GameBoard
{
    public TILE_TYPE[,] board;
    public List<Point>[] path;
    public List<Point> entry;
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
                tmp[i] = new List<Point>(path[i]);
            return path;
        }
    }
    public GameBoard(List<Point>[] path)
    {
        this.board = new TILE_TYPE[9, 9];
        this.path = new List<Point>[4];
        this.entry = new List<Point>();
        for (int i = 0; i < 4; ++i) {
            this.entry.Add(path[i][0]);
            this.entry.Add(path[i][path[i].Count-1]);
            this.path[i] = new List<Point>(path[i]);
            foreach (var p in path[i])
                this.board[p.x, p.y] = TILE_TYPE.ROAD;
        }
        for (int i = 0; i < 9; ++i)
            for (int j = 0; j < 9; ++j)
                if (this.board[i, j] == TILE_TYPE.NONE)
                    this.board[i, j] = (TILE_TYPE)Random.Range(1, 6);

    }
    public GameBoard(TILE_TYPE[,] board, List<Point>[] path)
    {
        this.board = (TILE_TYPE[,])board.Clone();
        this.path = new List<Point>[4];
        for (int i = 0; i < 4; ++i)
            this.path[i] = new List<Point>(path[i]);
    }
    public int EntryIndexOf(Point p)
    {
        return this.entry.FindIndex(e=>e.Equals(p));
    }
    public Point EntryAt(int n)
    {
        if (n > 7)
            return new Point(-1, -1);
        return this.entry[n];
    }
    public List<Point>[] PathAt(int n)
        //n번째 entry에서 출발한 경로를 반환, n이 8이상이면 empty
    {
        var tmp = new List<Point>[4];
        if (n > 7)
            return tmp;
        if (n % 2 == 0)
        {
            for(int i = 0; i < 4; ++i)
                tmp[i]=new List<Point>(path[(n/2 + i) % 4]);
            return tmp;
        }
        for(int i = 0; i < 4; ++i)
        {
            var r = new List<Point>(path[(4+ n / 2 - i) % 4]);
            r.Reverse();
            tmp[i] = r;
        }
        return tmp;
    }
}
