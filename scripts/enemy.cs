using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy  {
    public float health = 4;
    public int movespeed = 1;
    public float armor;
    public int position = 0;
    public bool slowed;
    public bool halfmoved;
    public float value = 5;
    public int entype;
    public bool flying;
    public Vector2Int pos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void set_en(int type, int level)
    {
        entype = type;
        if(type == 1)
        {
            flying = false;
            movespeed = 1;
            health = (1) * level;
            value = 10 + 5 * level;
        }
        if (type == 2)
        {
            flying = false;
            movespeed = 1;
            armor = 1;
            health = (4)*level;
            value = 10 + 5 * level;

        }
        if (type == 3)
        {
            movespeed = 2;
            flying = true;
            health = (1) * level;
            value = 10 + 5 * level;

        }
    }

    public void takedamage(float damage, int type, bool weak)
    {
        
        if (flying && type == 2)
        {

        }
        else
        {
            if (weak)
            {
                armor--;
                if (armor < 0)
                {
                    armor = 0;
                }
            }
            health -= damage - (armor*0.05f);
        }
    }

}
