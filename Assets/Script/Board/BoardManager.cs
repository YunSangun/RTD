using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public TowerManager[] m_TowerPrefabs;
    public GameObject[] m_CubePrefabs;
    public GameObject[] m_MonsterPrefabs;

    private Transform m_TileHolder;
    private Transform m_WallHolder;
    private Transform m_PathHolder;
    private Transform m_TowerHolder;
    private Transform m_MonsterHolder;


    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    var hit = Physics2D.Raycast(pos, Vector2.zero, 0f);
        //    if (hit.collider != null)
        //    {
        //        var obj = hit.collider.gameObject;
        //        if (obj.CompareTag("Tile"))
        //            TileSelect(obj.GetComponent<TileController>());
        //    }
        //}
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                var obj = hit.collider.gameObject;
                //if (obj.CompareTag("Tile"))
                //    TileSelect(obj.GetComponent<TileController>());
            }
        }
    }
    //보드 생성
    public void CreateBoard(GameBoard board)
    {
        var TileMap = new GameObject() { name = "Map" }.transform;
        TileMap.parent = transform;
        m_TileHolder = new GameObject() { name = "Tiles" }.transform;
        m_TileHolder.parent = TileMap;
        m_WallHolder = new GameObject() { name = "Walls" }.transform;
        m_WallHolder.parent = TileMap;
        m_PathHolder = new GameObject() { name = "Paths" }.transform;
        m_PathHolder.parent = TileMap;
        m_TowerHolder = new GameObject() { name = "Towers" }.transform;
        m_MonsterHolder = new GameObject() { name = "Monsters" }.transform;

        foreach (var p in from x in Enumerable.Range(0, 9)
                          from y in Enumerable.Range(0, 9)
                          select new Vector2Int(x, y))
        {
            var cm = board[p.x, p.y];
            switch (cm.m_Type)
            {
                case TILE_TYPE.STRAIGHT:
                case TILE_TYPE.TURN:
                case TILE_TYPE.CROSS:
                    Instantiate(m_CubePrefabs[(int)cm.m_Type], cm.m_Position, cm.m_Rotation, m_PathHolder);
                    break;
                case TILE_TYPE.NONE:
                case TILE_TYPE.WALL:
                    break;
                default:
                    Instantiate(m_CubePrefabs[(int)cm.m_Type], cm.m_Position, cm.m_Rotation, m_TileHolder);
                    break;
            }
        }
        foreach (var p in from x in Enumerable.Range(0, 9)
                          from y in Enumerable.Range(0, 9)
                          where x == 0 || y == 0 || x == 8 || y == 8
                          select new Vector2Int(x, y))
        {
            if (board[p.x, p.y].m_Type == TILE_TYPE.NONE)
            {
                Instantiate(m_CubePrefabs[(int)TILE_TYPE.WALL], new Vector3(p.x, -0.5f, p.y), Quaternion.Euler(0f, 0f, 0f), m_WallHolder);
                Instantiate(m_CubePrefabs[(int)TILE_TYPE.WALL], new Vector3(p.x, 0.5f, p.y), Quaternion.Euler(0f, 0f, 0f), m_WallHolder);
            }
            Instantiate(m_CubePrefabs[(int)TILE_TYPE.WALL], new Vector3(p.x, 1.5f, p.y), Quaternion.Euler(0f, 0f, 0f), m_WallHolder);
        }
    }
    public MonsterController CreateMonster(MONSTER_TYPE type)
    {
        return Instantiate(m_MonsterPrefabs[(int)type], m_MonsterHolder.transform)
        .GetComponent<MonsterController>();
    }
    // type형 타워를 보드에 생성한 후 manager를 반환
    public TowerManager CreateTower(int type)
    {
        return Instantiate(m_TowerPrefabs[type], m_TowerHolder.transform) as TowerManager;
    }
    //entry mark 표시
    //public void DisplayEntryMark(Point entry, Point exit)
    //{

    //    var entryTile = Tiles[entry.x, entry.y];
    //    var exitTile = Tiles[exit.x, exit.y];
    //    float angle = (entry.y == 0) ? 0f :
    //                  (entry.y == 8) ? 180f :
    //                  (entry.x == 0) ? 270f :
    //                                   90f;
    //    if (this.entry != null)
    //        Destroy(this.entry);
    //    this.entry = Instantiate(EntryMark, entryTile.transform.position, Quaternion.Euler(0, 0, angle), entryTile.transform);
    //    if (this.exit != null)
    //        Destroy(this.exit);
    //    this.exit = Instantiate(EntryMark, exitTile.transform.position, Quaternion.Euler(0, 0, angle), exitTile.transform);
    //}
    //타워정보 표시
    //private void DisplayOn(TowerManager tm)
    //{
    //    var ui = UIManager.Inst;
    //    ui.TowerExplainPanel.SetActive(true);
    //    ui.EmptyPanel.SetActive(false);
    //    if (rangeMask != null)
    //        Destroy(rangeMask);
    //    rangeMask = Instantiate(RangeMask, tm.BaseTile.transform);
    //    rangeMask.transform.Translate(0, 1, 0);
    //    int scale = tm.Range * 2 + 1;
    //    rangeMask.transform.localScale = new Vector3(scale, scale);
    //    rangeMask.transform.Rotate(270, 0, 0);
    //    ui.TowerTierText.text = $"Tier : {tm.Tier}";
    //    ui.TowerTypeText.text = $"Element : {tm.Type}";
    //    ui.TowerAttackText.text = $"Attack : {tm.Attack}";
    //    ui.TowerDelayText.text = $"Delay : {tm.Delay}";
    //}
    //타워정보 감춤
    //private void DisplayOff()
    //{
    //    UIManager.Inst.TowerExplainPanel.SetActive(false);
    //    UIManager.Inst.EmptyPanel.SetActive(true);
    //    if (rangeMask != null)
    //        Destroy(rangeMask);
    //}
    //public void TileSelect(TileController tc)
    //{
    //    DisplayOff();
    //    if (SelectedTile != null)
    //        SelectedTile.Selected = false;
    //    if (tc.Type < TILE_TYPE.STRAIGHT)
    //    {
    //        if (!tc.Selected)
    //        {
    //            tc.Selected = true;
    //            SelectedTile = tc;
    //            if (tc.BuiltTower != null)
    //                DisplayOn(tc.BuiltTower);
    //        }
    //    }
    //}
    //public void AddRandomTower()
    //{
    //    if (SelectedTile == null)
    //        return;
    //    if (SelectedTile.BuiltTower != null)
    //        return;
    //    if (!GameManager.Inst.BoughtTower(10))
    //        return;
    //    int type = Random.Range(1, 6);
    //    TowerManager tw = CreateTower(type);
    //    SelectedTile.BuiltTower = tw;
    //    tw.SetStatus(SelectedTile.transform.position, (TOWER_TYPE)type, 1f, 1, 1, 0.5f, SelectedTile);
    //    DisplayOn(tw);
    //}
}
