using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public float attack = 1.0f;
    public float range = 1.0f;
    public float delayTime = 0.0f;

    public int tier = 1;

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
    }

    private void OnMouseUp()
    {
        //Debug.Log("OnMouseUp");

        //this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        OnOffCollider();

        Vector2 pos = this.transform.position;
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction);

        //this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        OnOffCollider();

        if (rayHit.collider != null && (this.tier == rayHit.collider.gameObject.GetComponent<TowerManager>().tier))
        {
            Debug.Log(rayHit.collider.gameObject.name);
            Destroy(rayHit.collider.gameObject);
            Destroy(this.gameObject);

            GameObject.Find("GameManager").GetComponent<GameManager>().AddRandomTower(tier);
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
        if(tier == 1)
        {
            this.gameObject.GetComponent<CircleCollider2D>().enabled ^= true;
        }
        else if(tier == 2)
        {
            this.gameObject.GetComponent<BoxCollider2D>().enabled ^= true;
        }
    }

    //void AddRandomTower(int tier)
    //{
    //    int beg = 0, end = 0;

    //    if (tier == 0) {
    //        beg = 1;
    //        end = 5;
    //    }
    //    else if(tier == 1)
    //    {
    //        beg = 6;
    //        end = 6;
    //    }

    //    TowerManager tw = Instantiate(towers[Random.Range(beg, end)]) as TowerManager;
    //    tw.transform.localPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    //}
}
