using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour
{
    TOWER_TYPE TT = TOWER_TYPE.NONE;

    public MonsterController target;
    //private string monsterTag = "Monster";

    public Sprite[] Texture;
    private float attackPoint = 1.0f; // 타워 공격력
    private int range = 1;
    private float delayTime = 1.0f; // 지연 시간
    private float delayTimeRemain = 0.0f; // 남은 지연 시간(deltatime)
    private int tier = 1;
    public TileController BaseTile { get; set; }

    void Start()
    {
        transform.Translate(Vector3.back);
    }

    void SetTowerType()
    {
        string towerName = this.name;
        int delimiterIdx = towerName.IndexOf("_");
        towerName = towerName.Substring(delimiterIdx + 1);
        delimiterIdx = towerName.IndexOf("(Clone)");
        towerName = towerName.Substring(0, delimiterIdx).ToUpper();
        TT = (TOWER_TYPE)Enum.Parse(typeof(TOWER_TYPE), towerName);
        //Debug.Log(TT);
    }

    public int Tier
    {
        get { return this.tier; }
        set
        {
            GetComponent<SpriteRenderer>().sprite = Texture[value - 1];
            this.tier = value;
        }
    }
    public void SetStatus(int attack, int range,int tier,float delay,TileController basetile)
    {
        SetTowerType();
        this.attackPoint = attack;
        attackPoint *= tier;
        this.range = range;
        this.tier = tier;
        this.delayTime = delay;
        if (TT == TOWER_TYPE.WIND) delayTime *= 0.5f;
        this.BaseTile = basetile;
    }
    public void UpgradeTower()
    {
        var tw = Instantiate(GameManager.Inst.TowerPrefabs[Random.Range(0, 5)], GameManager.Inst.TowerList.transform) as TowerManager;
        BaseTile.BuiltTower = tw;
        tw.transform.position = BaseTile.transform.position;
        tw.Tier = tier + 1;
        tw.BaseTile = BaseTile;
        DestroyObj();
    }
    public void DestroyObj()
    {
        //target.TargetedTowers.Remove(this);
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if(CheckTarget())
            AttackTarget();
    }

    private Vector3 firstPosition;

    private void OnMouseDown()
    {
        //Debug.Log("OnMouseDown");

        firstPosition = this.transform.position;
        GameManager.Inst.TileSelect(BaseTile);
    }

    private void OnMouseUp()
    {
        //Debug.Log("OnMouseUp");

        OnOffCollider();

        Vector2 pos = this.transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(pos, Vector2.zero);

        OnOffCollider();

        if (rayHit.collider != null && rayHit.collider.CompareTag("Tower"))
        {
            var tw = rayHit.collider.gameObject.GetComponent<TowerManager>();
            if (Tier != tw.Tier || Tier > 2)
            {
                this.transform.position = firstPosition;
                return;
            }
            Debug.Log(rayHit.collider.gameObject.name);
            BaseTile.BuiltTower = null;
            DestroyObj();
            tw.UpgradeTower();
        }
        else
        {
            this.transform.position = firstPosition;
        }
    }

    private void OnMouseDrag()
    {
        //Debug.Log("OnMouseDrag");

        float dist = 10.0f;

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void OnOffCollider()
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled ^= true;
    }
    private bool Targeting(MonsterController target)
    {
        if (target != null)
        {
            target.TargetedTowers.Add(this); // 간헐적으로 에러 발생
            this.target = target;
            return true;
        }
        return false;
    }
    private void DisTartgeting()
    {
        target.TargetedTowers.Remove(this);
        target = null;
    }
    private bool CheckTarget()
    {
        var lt = new Point(BaseTile.Position.x - range, BaseTile.Position.y - range);
        var rb = new Point(BaseTile.Position.x + range, BaseTile.Position.y + range);
        if (target != null)
            if (!target.Position.Inside(lt, rb))
                DisTartgeting();
        if (target == null)
            return Targeting(GameManager.Inst.Monsters.Find(mc => mc.Position.Inside(lt, rb)));
        return true;
    }

    void AttackTarget()
    {
        delayTimeRemain -= Time.deltaTime;
        if (delayTimeRemain > 0.0f) return;
        //Debug.Log(delayTimeRemain);
        Debug.DrawLine((Vector2)this.transform.position, (Vector2)target.transform.position, Color.red);

        if(TT == TOWER_TYPE.ICE)
        {
            target.GetComponent<MonsterController>().Iced(tier);
        }

        target.GetComponent<MonsterController>().AttackedByTower(attackPoint);
        delayTimeRemain = delayTime;
    }
}
