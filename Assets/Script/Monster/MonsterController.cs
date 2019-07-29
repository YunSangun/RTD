using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterController : MonoBehaviour
{
    public GameManager manager;
    public float MaxHP { get; set; }
    public float HP { get; set; }
    public float Speed { get; set; }
    public int Attack { get; set; }
    public List<Point>[] MovePath { get; set; }

    public void SetStatus(float maxHP, float speed, int attack, List<Point>[] path)
    {
        this.MaxHP = maxHP;
        this.HP = maxHP;
        this.Speed = speed;
        this.Attack = attack;
        this.MovePath = new List<Point>[4];
        for (int i = 0; i < 4; ++i)
            this.MovePath[i] = new List<Point>(path[i]);
    }

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
        manager.MonsterArrive(this.Attack);
        Destroy(gameObject);
    }
}