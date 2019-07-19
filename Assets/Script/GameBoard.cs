using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Random = UnityEngine.Random;

[Serializable]
public class GameBoard
{
    private TILE_TYPE[,] board;
    private readonly List<Point> path;
    private readonly List<Point> entry;
    public TILE_TYPE this[int x, int y]
    {
        get
        {
            return board[x, y];
        }
    }
    public List<Point> Path
    {
        get
        {
            return this.path;
        }
    }
    public List<Point> Entry
    {
        get
        {
            return this.path;
        }
    }
    public GameBoard(List<Point> path, List<Point> entry)
    {
        this.board = new TILE_TYPE[9, 9];
        this.path = new List<Point>(path);
        this.entry = new List<Point>(entry);
        foreach (var p in path)
            this.board[p.x, p.y] = TILE_TYPE.ROAD;
        for (int i = 0; i < 9; ++i)
            for (int j = 0; j < 9; ++j)
                if (this.board[i, j] == TILE_TYPE.NONE)
                    this.board[i, j] = (TILE_TYPE)Random.Range(1, 6);

    }
    public GameBoard(TILE_TYPE[,] board, List<Point> path, List<Point> entry)
    {
        this.board = (TILE_TYPE[,])board.Clone();
        this.path = new List<Point>(path);
        this.entry = new List<Point>(entry);
    }
}
