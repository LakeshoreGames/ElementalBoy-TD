using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu_en : MonoBehaviour {

    int step;
    float timer;
    int type;

	// Use this for initialization
	void Start () {
        type = Random.Range(1, 4);
        change_anim();
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (step % 2 == 0)
        {
            if (timer >= 1)
            {
                timer = 0;
                step++;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(new Vector3(3.5f, (step - 2) * -1.5f + 4.4f, 0), new Vector3(3.5f, step * -1.5f + 4.4f, 0),timer);
            if(timer >= 1)
            {
                timer = 0;
                step++;
            }
        }
        if(step == 8)
        {
            step = 0;
            change_anim();
        }
    }

    void change_anim()
    {
        type++;
        if (type == 4)
        {
            type = 1;
        }
        GetComponent<Animator>().SetInteger("entype", type);
    }

}
