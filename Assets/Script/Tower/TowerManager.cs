using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour
{

    public MonsterController target;
    //private string monsterTag = "Monster";

    public Sprite[] Texture;
    private TOWER_TYPE type;
    private float attackPoint; // 타워 공격력
    private float attackRate = 1f;
    private int range;
    private float rangeRate = 1f;
    private float delay; // 지연 시간
    private float delayRate = 1f;
    private float delayRemain = 0.0f; // 남은 지연 시간(deltatime)
    private int tier;
    public TileController BaseTile { get; set; }
    public float Attack { get { return attackPoint * attackRate; } }
    public int Range { get { return (int)(range * rangeRate); } }
    public float Delay { get { return delay * delayRate; } }

    void Start()
    {
        transform.Translate(Vector3.back);
    }
    private int Tier
    {
        get { return this.tier; }
        set
        {
            GetComponent<SpriteRenderer>().sprite = Texture[value - 1];
            this.tier = value;
        }
    }
    private void OnSameTile()
    {

    }
    public void SetStatus(TOWER_TYPE type, float attack, int range, int tier, float delay, TileController basetile)
    {
        this.type = type;
        this.attackPoint = attack;
        this.range = range;
        Tier = tier;
        this.delay = delay;
        if (type == TOWER_TYPE.WIND) delay *= 0.5f;
        if ((int)type == (int)basetile.Type)
            OnSameTile();
        this.BaseTile = basetile;
    }
    public void UpgradeTower()
    {
        int type = Random.Range(1, 6);
        var tw = GameManager.Inst.BM.CreateTower(type);
        BaseTile.BuiltTower = tw;
        tw.transform.position = BaseTile.transform.position;
        tw.SetStatus((TOWER_TYPE)type, attackPoint + 1f, range + 1, tier + 1, 0.5f, BaseTile);
        GameManager.Inst.BM.TileSelect(tw.BaseTile);
        DestroyObj();
    }
    public void DestroyObj()
    {
        //target.TargetedTowers.Remove(this);
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if (CheckTarget())
            AttackTarget();
    }

    private Vector3 firstPosition;

    private void OnMouseDown()
    {
        //Debug.Log("OnMouseDown");

        firstPosition = this.transform.position;
        GameManager.Inst.BM.TileSelect(BaseTile);
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
        var lt = new Point(BaseTile.Position.x - Range, BaseTile.Position.y - Range);
        var rb = new Point(BaseTile.Position.x + Range, BaseTile.Position.y + Range);
        if (target != null)
            if (!target.Position.Inside(lt, rb))
                DisTartgeting();
        if (target == null)
            return Targeting(GameManager.Inst.Monsters.Find(mc => mc.Position.Inside(lt, rb)));
        return true;
    }

    void AttackTarget()
    {
        delayRemain -= Time.deltaTime;
        if (delayRemain > 0.0f) return;
        //Debug.Log(delayTimeRemain);
        Debug.DrawLine((Vector2)this.transform.position, (Vector2)target.transform.position, Color.red);

        //private LineRenderer attackLine;
        //attackLine = GetComponent<LineRenderer>();
        //attackLine.SetColors(Color.red);
        //attackLine.SetWidth(0.1f, 0.1f);
        //attackLine.SetPosition(0, (Vector2)this.transform.position);
        //attackLine.SetPosition(1, (Vector2)target.transform.position);

        if(type == TOWER_TYPE.FIRE)
        {
            Collider2D[] MonsterColl = Physics2D.OverlapBoxAll(target.transform.position, new Vector2(tier, tier), 0);

            foreach (Collider2D i in MonsterColl)
            {
                if (i.gameObject.tag == "Monster")
                {
                    i.gameObject.GetComponent<MonsterController>().AttackedByTower(Attack);
                }
            }
        }

        if (type == TOWER_TYPE.ICE)
        {
            target.GetComponent<MonsterController>().Iced(tier);
        }

        //if(target != null) target.GetComponent<MonsterController>().AttackedByTower(Attack);
        delayRemain = Delay;
    }
}
