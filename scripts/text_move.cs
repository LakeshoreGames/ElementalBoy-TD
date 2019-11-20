using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text_move : MonoBehaviour {

    public Vector3 last_position;
    public Vector3 new_position;
    public float timer;
    // Use this for initialization
    void Update()
    {
        if (timer < 1f)
        {
            timer += Time.deltaTime * 5;
            transform.position = Vector3.Lerp(last_position, new_position, timer);
        }

    }

    public void advance(Vector3 l, Vector3 n)
    {
        transform.position = new Vector3(((l.x/12)*580) + 42, ((l.y / 12) * 580) + 45 , 0);
        last_position = new Vector3(((l.x / 12) * 580) + 42, ((l.y / 12) * 580) + 45, 0);
        new_position = new Vector3(((n.x / 12) * 580) + 42, ((n.y / 12) * 580) + 45, 0);
        timer = 0;
    }
}
