using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Monster : MonoBehaviour
{
    private float maxHP;
    private float HP;
    private float speed;
    private Vector2 Position;
    public abstract void Move();
}
public class CommonMonster : Monster
{
    public override void Move()
    {
    }
}