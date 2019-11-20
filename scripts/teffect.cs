using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void attack()
    {
        GetComponent<Animator>().SetTrigger("attack");
    }
    public void set(int type)
    {
        if(type>4)
        {
            GetComponent<Animator>().SetTrigger("change");
        }
        GetComponent<Animator>().SetInteger("type", type);
        
    }
}


