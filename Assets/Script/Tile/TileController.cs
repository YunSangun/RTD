using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private bool selected;
    private GameObject mask;
    public TILE_TYPE Type { get; set; }
    public Point Position { get; set; }
    public TowerManager BuiltTower { get; set; }
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            if (value)
                mask = Instantiate(GameManager.Inst.SelectMask, transform);
            else
                Destroy(mask);
            selected = value;
        }
    }

    public void SetStatus(TILE_TYPE type, Point pos)
    {
        this.Type = type;
        this.Position = pos;
        this.Selected = false;
        this.BuiltTower = null;
    }
}
