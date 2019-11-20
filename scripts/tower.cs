using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower {
    public int type;
    public string name;
    public string aim;
    public string range;
    public int attackrate;
    public int turnstilattack;
    public int level;
    public float damage;
    public float basedamage;
    public bool slow;
    public bool weaken;
    public bool singletarget;
    public bool radial;
    public bool directional;
    public bool last;
    public int rangex;
    public int rangey;
    public int facing;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void setup(int towertype, int curface = 0)
    {
        type = towertype;
        switch(towertype)
        {
            case 1:
                rangex = 1;
                rangey = 1;

                radial = true;
                singletarget = false;
                directional = false;

                name = "Fire";
                aim = "Area";
                range = "3/3";

                attackrate = 2;
                damage = 1.5f;
                basedamage = 1.5f;

                slow = false;
                weaken = false;
                break;

            case 2:
                rangex = 1;
                rangey = 1;

                radial = false;
                singletarget = true;
                directional = false;

                name = "Earth";
                aim = "Single";
                range = "1";

                last = false;

                attackrate = 0;
                damage = 2;
                basedamage = 2;

                slow = false;
                weaken = false;
                break;

            case 3:
                rangex = 3;
                rangey = 0;

                radial = false;
                singletarget = false;
                directional = true;

                name = "Air";
                aim = "Line";
                range = "1/3";

                facing = 0;

                attackrate = 1;
                damage = 1;
                basedamage = 1;

                slow = false;
                weaken = false;
                break;

            case 4:
                rangex = 1;
                rangey = 1;

                radial = false;
                singletarget = true;
                directional = false;

                name = "Water";
                aim = "Single";
                range = "1";

                last = false;

                attackrate = 1;
                damage = 1.5f;
                basedamage = 1.5f;

                slow = true;
                weaken = false;
                break;
            case 5:
                rangex = 2;
                rangey = 2;

                radial = true;
                singletarget = false;
                directional = false;

                name = "Bomb";
                aim = "Area";
                range = "5/5";

                attackrate = 2;
                damage = 3;
                basedamage = 3;

                slow = false;
                weaken = false;
                break;
            case 6:
                rangex = 1;
                rangey = 1;

                radial = true;
                singletarget = false;
                directional = false;

                name = "Lava";
                aim = "Area";
                range = "3/3";

                attackrate = 1;
                damage = 2;
                basedamage = 2;

                slow = false;
                weaken = true;
                break;
            case 7:
                rangex = 1;
                rangey = 1;

                radial = false;
                singletarget = false;
                directional = true;

                name = "Electric";
                aim = "Line";
                range = "3/1";

                facing = curface;

                attackrate = 1;
                damage = 4;
                basedamage = 4;

                slow = false;
                weaken = false;
                break;
            case 8:
                rangex = 1;
                rangey = 1;

                radial = false;
                singletarget = true;
                directional = false;

                name = "Acid";
                aim = "Single";
                range = "1";

                last = false;

                attackrate = 0;
                damage = 2.5f;
                basedamage = 2.5f;

                slow = false;
                weaken = true;
                break;
            case 9:
                rangex = 2;
                rangey = 2;

                radial = false;
                singletarget = true;
                directional = false;

                name = "Metal";
                aim = "Single";
                range = "2";

                last = false;

                attackrate = 0;
                damage = 4;
                basedamage = 4;

                slow = false;
                weaken = false;
                break;
            case 10:
                rangex = 3;
                rangey = 0;

                radial = false;
                singletarget = false;
                directional = true;

                name = "Sand";
                aim = "Line";
                range = "1/3";

                facing = curface;

                attackrate = 1;
                damage = 2;
                basedamage = 2;

                slow = false;
                weaken = true;
                break;
            case 11:
                rangex = 2;
                rangey = 2;

                radial = true;
                singletarget = false;
                directional = false;

                name = "Swamp";
                aim = "Area";
                range = "3/3";

                attackrate = 1;
                damage = 2;
                basedamage = 2;

                slow = true;
                weaken = false;
                break;
            case 12:
                rangex = 5;
                rangey = 0;

                radial = false;
                singletarget = false;
                directional = true;

                name = "Tornado";
                aim = "Line";
                range = "1/5";

                facing = curface;

                attackrate = 1;
                damage = 2;
                basedamage = 2;

                slow = true;
                weaken = false;
                break;
            case 13:
                rangex = 1;
                rangey = 1;

                radial = false;
                singletarget = true;
                directional = false;

                name = "Ice";
                aim = "Single";
                range = "1";

                last = false;

                attackrate = 0;
                damage = 2.5f;
                basedamage = 2.5f;

                slow = true;
                weaken = false;
                break;
            case 14:
                rangex = 11;
                rangey = 11;

                radial = true;
                singletarget = false;
                directional = false;

                name = "Ocean";
                aim = "Area";
                range = "All";

                last = false;

                attackrate = 5;
                damage = 5;
                basedamage = 5;

                slow = false;
                weaken = false;
                break;

        }


    }

    public void levelup()
    {
        level++;
        damage += basedamage;
    }
}
