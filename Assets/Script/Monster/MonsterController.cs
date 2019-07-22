using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MOVE_TYPE
{
    STOP,
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public abstract class MonsterController : MonoBehaviour
{
    private GameBoard Map;
    private Rigidbody2D rb2d;
    private float maxHP;
    private float HP;
    public float speed;
    public List<Point>[] MovePath { get; set; }
    private MOVE_TYPE directrion;
    private Vector3 Position;

    public IEnumerator coroutine()
    {

        rb2d = GetComponent<Rigidbody2D>();
        for(int i = 0; i < 4; ++i)
        {
            var start = MovePath[i][0].ToVector()+GameManager.REVISE;
            var forward = ((MovePath[i][1] - MovePath[i][0])).ToVector() / 2;
            Vector2 delta;
            transform.position = (Vector3)(start - forward);
            float distance = 0f;
            var exceedTime = 0f;
            while (true)
            {
                delta = forward.normalized * speed * Time.deltaTime;
                distance += delta.magnitude;
                transform.position = transform.position+(Vector3)(delta);
                if (distance > 0.5f)
                {
                    exceedTime = (distance - 0.5f) / speed;
                    transform.position = (Vector3)start;
                    transform.position = transform.position - (Vector3)(delta*exceedTime*speed);
                    break;
                }
                yield return null;
            }
            for (int j = 0; j + 1 < MovePath[i].Count; ++j)
            {
                distance = 0f;
                forward = ((MovePath[i][j + 1] - MovePath[i][j])).ToVector();
                delta = forward.normalized * speed * exceedTime;
                transform.position = transform.position + (Vector3)(delta);
                distance += delta.magnitude;
                yield return null;
                while (true)
                {
                    delta = forward.normalized * speed * Time.deltaTime;
                    transform.position = transform.position + (Vector3)(delta);
                    distance += delta.magnitude;
                    if (distance > 1f)
                    {
                        exceedTime = (distance - 1f) / speed;
                        transform.position = (Vector3)(MovePath[i][j + 1].ToVector() + GameManager.REVISE);
                        break;
                    }
                    yield return null;
                }
            }
            distance = 0f;
            delta = forward.normalized * speed * exceedTime;
            transform.position = transform.position + (Vector3)(delta);
            distance += delta.magnitude;
            yield return null;
            while (true)
            {
                delta = forward.normalized * speed * Time.deltaTime;
                transform.position = transform.position + (Vector3)(delta);
                distance += delta.magnitude;
                if (distance > 0.5f)
                    break;
                yield return null;
            }
        }
        
        
    }
    private void Start()
    {
    }
}