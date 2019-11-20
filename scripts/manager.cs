using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class manager : MonoBehaviour {

    public Texture2D curs;
    public Sprite filledstar;
    public Sprite unfilledstar;

    public static manager instance;
    public bool muted;
    public bool endless;
    public bool playing;
    public bool paused;
    public int curlevel;

    public GameObject pausemenu;
    public GameObject winscreen;
    public GameObject lossscreen;
    public GameObject lossscreen2;
    public Toggle mutetoggle;
    public Image star2;
    public Image star3;
    public Text wavereport;

    public AudioSource win;
    public AudioSource title;
    public AudioSource lose;
    public AudioSource buildmus;
    public AudioSource battlemus;
    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            if (!PlayerPrefs.HasKey("level1"))
            {
                PlayerPrefs.SetInt("level1", 0);
                PlayerPrefs.SetInt("level2", 0);
                PlayerPrefs.SetInt("level3", 0);
                PlayerPrefs.SetInt("level4", 0);
                PlayerPrefs.SetInt("level5", 0);
                PlayerPrefs.SetInt("level6", 0);
                PlayerPrefs.SetInt("level7", 0);
                PlayerPrefs.SetInt("level8", 0);
                PlayerPrefs.SetInt("level9", 0);
                PlayerPrefs.SetInt("level1e", 0);
                PlayerPrefs.SetInt("level2e", 0);
                PlayerPrefs.SetInt("level3e", 0);
                PlayerPrefs.SetInt("level4e", 0);
                PlayerPrefs.SetInt("level5e", 0);
                PlayerPrefs.SetInt("level6e", 0);
                PlayerPrefs.SetInt("level7e", 0);
                PlayerPrefs.SetInt("level8e", 0);
                PlayerPrefs.SetInt("level9e", 0);
            }
        }
        else
        {
            Destroy(this);
        }
    }

    void Start () {
        Cursor.SetCursor(curs, Vector2.zero, CursorMode.Auto);
        /* Application.ExternalEval(
       @"if(typeof(kongregateUnitySupport) != 'undefined'){
         kongregateUnitySupport.initAPI('manager', 'OnKongregateAPILoaded');
       };"
     );*/
      //  Resetgame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (playing)
            {
                if (Input.GetButtonDown("menu"))
                {
                    pausegame();
                }
                if (Input.GetButtonDown("Jump"))
                {
                    if (Time.timeScale == 0.5f)
                    {
                        Time.timeScale = 1;
                    }
                    else if (Time.timeScale == 1)
                    {
                        Time.timeScale = 2;
                    }
                    else if (Time.timeScale == 2)
                    {
                        Time.timeScale = 4;
                    }
                    else if (Time.timeScale == 4)
                    {
                        Time.timeScale = 0.5f;
                    }
                }
            }
        }
    }

    public void mute()
    {
        if(muted)
        {
            if(mutetoggle.isOn)
            {
                mutetoggle.isOn = false;
            }
            muted = false;
            if(playing)
            {
                if(GameObject.Find("Grid").GetComponent<grid>().wave_state == -1)
                {
                    buildmus.Play();
                }
                else
                {
                    battlemus.Play();
                }
            }
            else
            {
                title.Play();
            }
        }
        else
        {
            if (!mutetoggle.isOn)
            {
                mutetoggle.isOn = true;
            }
            muted = true;
            if (playing)
            {
                battlemus.Stop();
                buildmus.Stop();
            }
            else
            {
                title.Stop();
            }
        }
    }

    public void tomain()
    {
        buildmus.Stop();
        battlemus.Stop();
        pausemenu.SetActive(false);
        paused = false;
        playing = false;
        endless = false;
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        winscreen.SetActive(false);
        lossscreen.SetActive(false);
        lossscreen2.SetActive(false);
        if (!muted)
        {
            if (!title.isPlaying)
            {
                title.Play();
            }
        }
    }

    public void backtogame()
    {
        pausemenu.SetActive(false);
        paused = false;
        Time.timeScale = 1;
    }

    public void OnKongregateAPILoaded(string userInfoString)
    {
        OnKongregateUserInfo(userInfoString);
    }

    public void OnKongregateUserInfo(string userInfoString)
    {
        var info = userInfoString.Split('|');
        var userId = System.Convert.ToInt32(info[0]);
        var username = info[1];
        var gameAuthToken = info[2];
    }

    public void levelcomplete(int lnum, int stars)
    {
        Time.timeScale = 1;
        title.Stop();
        buildmus.Stop();
        battlemus.Stop();

        if (!muted)
        {
            win.Play();
        }
        paused = true;
        playing = false;
        if(stars > PlayerPrefs.GetInt("level" + lnum.ToString()))
        {
            PlayerPrefs.SetInt("level" + lnum.ToString(), stars);
        //    Application.ExternalCall("kongregate.stats.submit", "level"+lnum.ToString(), stars);
        }
        if(stars == 2)
        {
            star2.sprite = filledstar;
        }
        else if(stars == 3)
        {
            star2.sprite = filledstar;
            star3.sprite = filledstar;
        }
        winscreen.SetActive(true);
    }

    public void nextlevel()
    {
        curlevel++;
        if (curlevel > 9)
        {
            tomain();
        }
        else
        {
            star2.sprite = unfilledstar;
            star3.sprite = unfilledstar;
            winscreen.SetActive(false);
            lossscreen.SetActive(false);
            lossscreen2.SetActive(false);
            paused = false;
            SceneManager.LoadScene(2);
            curlevel++;
        }
    }

    public void lost(int lnum, int total)
    {
        if (total != 0)
        {
            if (total > PlayerPrefs.GetInt("level" + lnum.ToString() + "e"))
            {
                PlayerPrefs.SetInt("level" + lnum.ToString() + "e", total);
              //  Application.ExternalCall("kongregate.stats.submit", "level" + lnum.ToString() + "e", total);
            }
            wavereport.text = total.ToString();
            Time.timeScale = 1;
            paused = true;
            if (!muted)
            {
                lose.Play();
            }
            lossscreen2.SetActive(true);
            curlevel--;
        }
        else
        {
            Time.timeScale = 1;
            paused = true;
            if (!muted)
            {
                lose.Play();
            }
            lossscreen.SetActive(true);
            curlevel--;
        }
    }

    public void Resetgame()
    {
        PlayerPrefs.SetInt("level1", 0);
        PlayerPrefs.SetInt("level2", 0);
        PlayerPrefs.SetInt("level3", 0);
        PlayerPrefs.SetInt("level4", 0);
        PlayerPrefs.SetInt("level5", 0);
        PlayerPrefs.SetInt("level6", 0);
        PlayerPrefs.SetInt("level7", 0);
        PlayerPrefs.SetInt("level8", 0);
        PlayerPrefs.SetInt("level9", 0);
        PlayerPrefs.SetInt("level1e", 0);
        PlayerPrefs.SetInt("level2e", 0);
        PlayerPrefs.SetInt("level3e", 0);
        PlayerPrefs.SetInt("level4e", 0);
        PlayerPrefs.SetInt("level5e", 0);
        PlayerPrefs.SetInt("level6e", 0);
        PlayerPrefs.SetInt("level7e", 0);
        PlayerPrefs.SetInt("level8e", 0);
        PlayerPrefs.SetInt("level9e", 0);
    }

    public void pausegame()
    {
        paused = true;
        pausemenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void go_to_level(int lvl)
    {
        if (title.isPlaying)
        {
            title.Stop();
        }
        SceneManager.LoadScene(2);
        playing = true;
        curlevel = lvl;
        endless = false;
    }

    public void go_to_endless(int lvl)
    {
        if (title.isPlaying)
        {
            title.Stop();
        }
        SceneManager.LoadScene(2);
        playing = true;
        curlevel = lvl;
        endless = true;
    }

    public void switch_music(bool b)
    {
        if(!muted)
        {
            if(b)
            {
                battlemus.Stop();
                buildmus.Play();
            }
            else
            {
                buildmus.Stop();
                battlemus.Play();
            }
        }
    }

}
