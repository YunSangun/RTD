using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterController : MonoBehaviour
{
    public GameManager manager;
    private float MaxHP;
    private float HP;
    private float Speed;
    private int Attack;
    private List<Point> MovePath;
    private float dist = 0f;
    private float Alldist = 0f;
    private int index = 1;

    public void SetStatus(float maxHP, float speed, int attack, List<Point> path)
    {
        this.MaxHP = maxHP;
        this.HP = maxHP;
        this.Speed = speed;
        this.Attack = attack;
        this.MovePath = new List<Point>(path);
    }
    public void Start()
    {
        var startPoint = this.MovePath[0];
        transform.position = startPoint.ToVector() + (this.MovePath[1] - startPoint).ToVector() / 2 + GameManager.REVISE;
    }
    public void Update()
    {
        dist += this.Speed * Time.deltaTime;
        Alldist += this.Speed * Time.deltaTime;
        if (dist >= 1f)
        {
            if (++index == MovePath.Count - 1)
            {
                manager.MonsterArrive(this.Attack);
                Destroy(gameObject);
                return;
            }
            dist -= 1f;
        }
        Point prevDir=MovePath[index]-MovePath[index-1];
        Point nextDir=MovePath[index+1]-MovePath[index];
        if (Math.Abs(prevDir.x) > 1)
            prevDir.x /= -8;
        else if(Math.Abs(prevDir.y) > 1)
            prevDir.y /= -8;
        if (Math.Abs(nextDir.x) > 1)
            nextDir.x /= -8;
        else if (Math.Abs(nextDir.y) > 1)
            nextDir.y /= -8;
        Vector3 position= MovePath[index].ToVector() + GameManager.REVISE;
        if (dist <= 0.5f)
            position -= (0.5f - dist) * prevDir.ToVector3();
        else
            position += (dist - 0.5f) * nextDir.ToVector3();
        transform.position = position;
    }
    /*
    public IEnumerator Move()
    {

        for(int i = 0; i < 4; ++i)
        {
            var start = GameManager.REVISE + MovePath[i][0].ToVector();   //화면상 시작점
            var direction = ((MovePath[i][1] - MovePath[i][0])).ToVector();   //이동 방향
            Vector3 delta;           //프레임당 이동거리
            float distance = 0f;     //현재 이동 거리
            float exceed = 0f;   //도착 후 초과한 거리

            start -= direction / 2;
            transform.position = start; //화면 끝점에서 출발
            while (true)
            {
                delta = direction * Speed * Time.deltaTime;
                distance += delta.magnitude;
                transform.position = transform.position+delta;
                if (distance > 0.5f)
                {
                    exceed = distance - 0.5f;
                    transform.position = start+direction/2;
                    break;
                }
                yield return null;
            }

            for (int j = 0; j + 1 < MovePath[i].Count; ++j)
            {
                start = GameManager.REVISE + MovePath[i][j].ToVector();
                direction = ((MovePath[i][j + 1] - MovePath[i][j])).ToVector();
                transform.position = transform.position + (Vector3)(direction)*exceed;
                distance = exceed;
                yield return null;

                while (true)
                {
                    delta = direction * Speed * Time.deltaTime;
                    transform.position = transform.position + delta;
                    distance += delta.magnitude;
                    if (distance > 1f)
                    {
                        exceed = distance - 1f;
                        transform.position =start+direction;
                        break;
                    }
                    yield return null;
                }
            }
            start += direction;
            transform.position = transform.position + (Vector3)(direction) * exceed;
            distance = exceed;
            yield return null;

            while (true)
            {
                delta = direction * Speed * Time.deltaTime;
                transform.position = transform.position + delta;
                distance += delta.magnitude;
                if (distance > 0.5f)
                    break;
                yield return null;
            }
        }
        
    }*/
}