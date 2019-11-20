using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Splash : MonoBehaviour {

    public Image splash;
    public Image black;
    private float timer;

	// Use this for initialization
	void Start () {
        timer = 3.5f;
	}
	
	// Update is called once per frame
	void Update () {

        if (timer > 2.5f)
        {
            timer -= Time.deltaTime;
            black.color = new Color(0, 0, 0, timer - 2.5f);
        }
        if(timer < 2.5f && timer > 2)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 2f && timer > 1f)
        {
            timer -= Time.deltaTime;
            black.color = new Color(0, 0, 0, 2f - timer);
            if (timer <= 1f)
            {
                splash.color = Color.clear;
            }
        }
        if (timer <= 1 && timer > 0)
        {
            timer -= Time.deltaTime;
            black.color = new Color(0, 0, 0,timer);
            if (timer <= 0)
            {
                black.color = Color.clear;
                manager.instance.title.Play();
            }        
        }
        if(timer < 0)
        {
            if(Input.GetMouseButtonDown(0))
            {
                manager.instance.tomain();
            }
        }

    }

}
