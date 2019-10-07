using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject BoardArea;
    public GameObject SelectMask;
    public GameObject EntryMark;
    public GameObject RangeMask;
    public TowerManager[] TowerPrefabs;
    public GameObject[] TilePrefabs;
    public GameObject[] RoadTilePrefabs;
    public GameObject[] MonsterPrefabs;
    private TileController[,] Tiles = new TileController[9, 9];
    private TileController SelectedTile = null;
    private GameObject entry;
    private GameObject exit;
    private GameObject rangeMask;
    private GameObject TileHolder;
    private GameObject TowerHolder;
    private GameObject MonsterHolder;
    public static Vector2 REVISE;
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

            if(Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                var obj = hit.collider.gameObject;
                if (obj.CompareTag("Tile"))
                    TileSelect(obj.GetComponent<TileController>());
            }
        }
    }
    public void MakeBoard()
    {
        BoardManager.REVISE = transform.position + new Vector3(-4f, -4f);
        //parent 객체 설정
        //Destroy(BoardArea);
        TileHolder = Instantiate(new GameObject() { name = "Tiles" }, transform);
        TowerHolder = Instantiate(new GameObject() { name = "Towers" }, transform);
        MonsterHolder = Instantiate(new GameObject() { name = "Monsters" }, transform);
        //첫번째 경로 타일
        var GameMap = GameManager.Inst.GameMap;
        List<Point> mapPath = GameMap.ToList();
        GameObject origin = RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT];
        Vector3 location = mapPath[0].ToVector() + BoardManager.REVISE;
        location.z = 1;
        //Quaternion rotate = Quaternion.Euler(0, 0, (mapPath[0].x == mapPath[1].x) ? 0 : 90f);
        Quaternion rotate = Quaternion.identity;

        if (mapPath[0].x == mapPath[1].x)
            rotate = Quaternion.Euler(-90, 0, 0);
        else
            rotate = Quaternion.Euler(0, 90, -90);

        //var angles = transform.rotation.eulerAngles;
        //Quaternion rotate = Quaternion.Euler(angles);
        var tc = Instantiate(origin, location, rotate, TileHolder.transform).GetComponent<TileController>();
        Tiles[mapPath[0].x, mapPath[0].y] = tc;
        tc.SetStatus(TILE_TYPE.ROAD, mapPath[0]);
        //
        //2~n-1 경로타일
        for (int i = 1; i + 1 < mapPath.Count; ++i)
        {
            var prev = mapPath[i - 1];
            var cur = mapPath[i];
            var next = mapPath[i + 1];
            location = cur.ToVector() + BoardManager.REVISE;
            location.z = 1;
            bool overlab = mapPath.FindAll(p => p.Equals(cur)).Count == 2;
            bool straightY = (next - prev).x == 0;
            bool straightX = (next - prev).y == 0;
            bool beginX = (cur - prev).y == 0;
            if (overlab && straightX)
                continue;
            origin = overlab ? RoadTilePrefabs[(int)ROAD_TYPE.CROSS] :
                        straightX || straightY ? RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT] : RoadTilePrefabs[(int)ROAD_TYPE.TURN];
            //float angle = 0f;
            //angle = straightX ? 90f : straightY ? 0 :
            //            beginX ? (float)45 * (3 - (cur - prev).x * ((next - cur).y + 2)) : (float)45 * (3 + (next - cur).x * (2 - (cur - prev).y));
            //rotate = Quaternion.Euler(0, 0, angle);
            if (straightX)
            {
                rotate = Quaternion.Euler(0, 90, -90);
            }
            else if (straightY)
            {
                rotate = Quaternion.Euler(-90, 0, 0);
            }
            else
            {
                if (beginX)
                {
                    if((next - cur).y >= 0) rotate = Quaternion.Euler(90, 90, -90);
                    else rotate = Quaternion.Euler(0, 90, -90);
                }
                else
                {
                    if ((cur - prev).y < 0) rotate = Quaternion.Euler(0, -90, 90);
                    else rotate = Quaternion.Euler(-90, 90, -90);
                }
            }
            Tiles[cur.x, cur.y] = Instantiate(origin, location, rotate, TileHolder.transform).GetComponent<TileController>();
            Tiles[cur.x, cur.y].SetStatus(TILE_TYPE.ROAD, cur);
        }
        //
        //마지막 경로 타일
        origin = RoadTilePrefabs[(int)ROAD_TYPE.STRAIGHT];
        location = mapPath[mapPath.Count - 1].ToVector() + BoardManager.REVISE;
        location.z = 1;
        //rotate = mapPath[mapPath.Count - 1].x == mapPath[mapPath.Count - 2].x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 90f);
        rotate = mapPath[mapPath.Count - 1].x == mapPath[mapPath.Count - 2].x ? Quaternion.Euler(-90, 0, 0) : Quaternion.Euler(0, 90, -90);
        Tiles[mapPath[mapPath.Count - 1].x, mapPath[mapPath.Count - 1].y] = Instantiate(origin, location, rotate, TileHolder.transform).GetComponent<TileController>();
        Tiles[mapPath[mapPath.Count - 1].x, mapPath[mapPath.Count - 1].y].SetStatus(TILE_TYPE.ROAD, mapPath[mapPath.Count - 1]);
        //
        //경로 외 속성타일
        for (int i = 0; i < 9; ++i)
            for (int j = 0; j < 9; ++j)
                if (GameMap[i, j] != TILE_TYPE.ROAD)
                {
                    Tiles[i, j] = Instantiate(TilePrefabs[(int)GameMap[i, j]], new Vector3(i, j, 1.0f) + (Vector3)BoardManager.REVISE, Quaternion.Euler(270, 0, 0), TileHolder.transform).GetComponent<TileController>();
                    Tiles[i, j].SetStatus(GameMap[i, j], new Point(i, j));
                }
        //
    }
    public MonsterController CreateMonster(MONSTER_TYPE type, Point pos)
    {
        return Instantiate(MonsterPrefabs[(int)type], MonsterHolder.transform)
        .GetComponent<MonsterController>();
    }
    public TowerManager CreateTower(int type)
    {
        return Instantiate(TowerPrefabs[type], TowerHolder.transform) as TowerManager;
    }
    public void DisplayEntryMark(Point entry, Point exit)
    {

        var entryTile = Tiles[entry.x, entry.y];
        var exitTile = Tiles[exit.x, exit.y];
        float angle = (entry.y == 0) ? 0f :
                      (entry.y == 8) ? 180f :
                      (entry.x == 0) ? 270f :
                                       90f;
        if (this.entry != null)
            Destroy(this.entry);
        this.entry = Instantiate(EntryMark, entryTile.transform.position, Quaternion.Euler(0, 0, angle), entryTile.transform);
        if (this.exit != null)
            Destroy(this.exit);
        this.exit = Instantiate(EntryMark, exitTile.transform.position, Quaternion.Euler(0, 0, angle), exitTile.transform);
    }
    private void DisplayOn(TowerManager tm)
    {
        var ui = UIManager.Inst;
        ui.TowerExplainPanel.SetActive(true);
        ui.EmptyPanel.SetActive(false);
        if (rangeMask != null)
            Destroy(rangeMask);
        rangeMask = Instantiate(RangeMask, tm.BaseTile.transform);
        rangeMask.transform.Translate(0, 1, 0);
        int scale = tm.Range * 2 + 1;
        rangeMask.transform.localScale = new Vector3(scale, scale);
        rangeMask.transform.Rotate(270, 0, 0);
        ui.TowerTierText.text = $"Tier : {tm.Tier}";
        ui.TowerTypeText.text = $"Element : {tm.Type}";
        ui.TowerAttackText.text = $"Attack : {tm.Attack}";
        ui.TowerDelayText.text = $"Delay : {tm.Delay}";
    }
    private void DisplayOff()
    {
        UIManager.Inst.TowerExplainPanel.SetActive(false);
        UIManager.Inst.EmptyPanel.SetActive(true);
        if (rangeMask != null)
            Destroy(rangeMask);
    }
    public void TileSelect(TileController tc)
    {
        DisplayOff();
        if (SelectedTile != null)
            SelectedTile.Selected = false;
        if (tc.Type != TILE_TYPE.ROAD)
        {
            if (!tc.Selected)
            {
                tc.Selected = true;
                SelectedTile = tc;
                if (tc.BuiltTower != null)
                    DisplayOn(tc.BuiltTower);
            }
        }
    }
    public void AddRandomTower()
    {
        if (SelectedTile == null)
            return;
        if (SelectedTile.BuiltTower != null)
            return;
        if (!GameManager.Inst.BoughtTower(10))
            return;
        int type = Random.Range(1, 6);
        TowerManager tw = CreateTower(type);
        SelectedTile.BuiltTower = tw;
        tw.transform.position = SelectedTile.transform.position;
        tw.SetStatus((TOWER_TYPE)type, 1f, 1, 1, 0.5f, SelectedTile);
        DisplayOn(tw);
    }
}
