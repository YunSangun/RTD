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
    public TowerManager tower;
    public Texture btnTexture;

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
            addTower();
        }
    }

    void addTower()
    {
        TowerManager tw = Instantiate(tower) as TowerManager;
        tw.transform.localPosition = new Vector2(0, 0);
    }
}
