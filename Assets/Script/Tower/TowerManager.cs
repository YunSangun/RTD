using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public Sprite[] Texture;
    public float attack = 1.0f;
    public float range = 1.0f;
    public float delayTime = 0.0f;
    private int tier = 1;
    public TileController BaseTile { get; set; }
    public int Tier
    {
        get { return this.tier; }
        set
        {
            GetComponent<SpriteRenderer>().sprite = Texture[value - 1];
            this.tier = value;
        }
    }

    public void UpgradeTower()
    {
        var tw = Instantiate(GameManager.Inst.TowerPrefabs[Random.Range(0, 5)]) as TowerManager;
        BaseTile.BuiltTower = tw;
        tw.transform.position = BaseTile.transform.position;
        tw.Tier = tier+1;
        tw.BaseTile = BaseTile;
        Destroy(gameObject);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    Vector3 firstPosition;

    private void OnMouseDown()
    {
        //Debug.Log("OnMouseDown");

        firstPosition = this.transform.position;
        GameManager.Inst.TileSelect(BaseTile);
    }

    private void OnMouseUp()
    {
        //Debug.Log("OnMouseUp");

        //this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        OnOffCollider();

        Vector2 pos = this.transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(pos, Vector2.zero);

        //this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        OnOffCollider();

        if (rayHit.collider != null&&rayHit.collider.CompareTag("Tower"))
        {
            var tw = rayHit.collider.gameObject.GetComponent<TowerManager>();
            if (Tier != tw.Tier || Tier > 2)
            {
                this.transform.position = firstPosition;
                return;
            }
            Debug.Log(rayHit.collider.gameObject.name);
            BaseTile.BuiltTower = null;
            Destroy(this.gameObject);
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

    void OnOffCollider()
    {
            this.gameObject.GetComponent<BoxCollider2D>().enabled ^= true;
    }
}
