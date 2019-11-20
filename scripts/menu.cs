using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour {

    public GameObject[] states;
    public GameObject[] infotabs;

    int cur_state;
    int cur_info;


    public GameObject anims;
    public GameObject[] stars;
    public Text[] endless;
    public Toggle mute;
    public Sprite star;
    // Use this for initialization
    void Start () {
        setstars();
        setbests();
        if(manager.instance.muted)
        {
            setmute();
            mute.isOn = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void change_state(int i)
    {
        states[cur_state].SetActive(false);
        cur_state = i;
        states[cur_state].SetActive(true);

        if(cur_state !=0)
        {
            anims.SetActive(false);
        }
        else
        {
            anims.SetActive(true);
        }
    }

    public void nextinfo()
    {
        infotabs[cur_info].SetActive(false);
        cur_info++;
        if (cur_info == infotabs.Length)
        {
            cur_info = 0;
        }
        infotabs[cur_info].SetActive(true);

    }
    public void lastinfo()
    {
        infotabs[cur_info].SetActive(false);
        cur_info--;
        if (cur_info == -1)
        {
            cur_info = infotabs.Length - 1;
        }
        infotabs[cur_info].SetActive(true);
    }

    public void gottogame(int lvlnum)
    {
        manager.instance.go_to_level(lvlnum);
    }

    public void gottogame2(int lvlnum)
    {
        manager.instance.go_to_endless(lvlnum);
    }

    public void setstars()
    {
        for(int i = 1; i <= 9; i++)
        {
            int temp = PlayerPrefs.GetInt("level" + (i).ToString());
            Image[] s = stars[i-1].GetComponentsInChildren<Image>();
            if(temp == 3)
            {
                s[2].sprite = star;
                s[1].sprite = star;
                s[0].sprite = star;
            }
            else if(temp == 2)
            {
                s[1].sprite = star;
                s[0].sprite = star;

            }
            else if (temp == 1)
            {
                s[0].sprite = star;
            }
        }
    }

    public void setbests()
    {
        for (int i = 1; i <= 9; i++)
        {
            int temp = PlayerPrefs.GetInt("level" + (i).ToString()+"e");
            endless[i - 1].text = temp.ToString();
        }
    }

    public void more()
    {
        Application.OpenURL("http://www.lakeshoregames.co");
    }

    public void setmute()
    {
        manager.instance.mute();
    }
}
