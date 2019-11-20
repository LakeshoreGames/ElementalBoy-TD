using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_visual : MonoBehaviour {

    public Animator anim;
    public int size;
    public int type;
    public Vector3Int base_types;
    public Vector3 last_position;
    public Vector3 new_position;
    public int lp;
    public int np;
    public float timer;
    // Use this for initialization
    void Start () {
      

    }

    // Update is called once per frame
    void Update () {
        if (timer < 1f)
        {
            timer += Time.deltaTime*5;
            transform.position = Vector3.Lerp(last_position, new_position, timer);
        }

	}

    public void advance(Vector3 l, Vector3 n)
    {
        transform.position = l;
        last_position = l;
        new_position = n;
        timer = 0;
    }
    public void find_type()
    {
        if(base_types.x > base_types.y && base_types.x > base_types.z)
        {
            type = 1;
        }
        if (base_types.y > base_types.x && base_types.y > base_types.z)
        {
            type = 2;
        }
        if (base_types.z >= base_types.y && base_types.z >= base_types.x)
        {
            type = 3;
        }

        anim.SetInteger("entype", type);
    }
}
