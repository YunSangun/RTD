using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ELEMENT_TYPE
{
    FIRE,
    ICE,
    POISON,
    IRON,
    WIND
}
enum TILE_TYPE
{

}
public class GameBoard
{

}

public class GameManager : MonoBehaviour
{
    public Texture btnTexture;
    public TowerManager []towers;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnGUI() {
        //if (!btnTexture)
        //{
        //    Debug.Log("Check btnTexture");
        //    return;
        //}

        //if (GUI.Button(new Rect(100, 100, 100, 100), btnTexture, "Add Tower"))
        //{
        //    addTower();
        //}
        if (GUI.Button(new Rect(0, 0, 100, 100), "Add Tower"))
        {
            AddRandomTower(0);
        }
    }

    public void AddRandomTower(int tier, float x = 0, float y = 0)
    {
        int beg = 0, end = 0;

        if (tier == 0)
        {
            beg = 0;
            end = 5;
        }
        else if (tier == 1)
        {
            beg = 5;
            end = 10;
        }
        else if (tier == 2)
        {
            beg = 10;
            end = 15;
        }

        TowerManager tw = Instantiate(towers[Random.Range(beg, end)]) as TowerManager;
        tw.transform.localPosition = new Vector2(x, y);
        //tw.transform.localPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
}