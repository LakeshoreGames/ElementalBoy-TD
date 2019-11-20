using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class grid : MonoBehaviour
{
    public tile[,] tiles;
    public int[,] level;
    public int[] waverewards;

    public Toggle firebutton; 
    public Toggle earthbutton; 
    public Toggle airbutton; 
    public Toggle waterbutton; 
    public Toggle startbutton; 
    public Toggle infobutton;
    public Toggle firstlastbutton;
    public Image firstbutton;
    public Button Lbut;
    public Button Rbut;
    public Button ubut;

    public Text[] towerinfo;
    public Text goldcount;
    public Text remaininglife;

    public Vector2 mousepos;
    public int buildmode;
    public bool OVER;
    public bool endless;
    public int loops;
    public int totalwaves;

    public GameObject main;
    public GameObject buildmenu;
    public GameObject info;

    public Transform towerparent;
    public Transform enemyparent;
    public Transform effectparent;
    public Transform textparent;

    public List<Vector2Int> path;
    public int numwaves;
    public int wavelengths;
    public Queue<Vector3Int> enemystats;

    public int hp = 10;
    public float turntimer;
    public bool spawnen = true;
    public bool startwave = false;
    public int levelnum;
    public int enspawned = 0;
    public int curwave = 0;
    public float resources = 300;

    public Sprite ground_sprite;
    public Sprite path_sprite;

    public Vector2Int selectedtower;

    public GameObject tile_prefab;
    public GameObject enemy_visual_prefab;
    public GameObject tower_visual_prefab;
    public GameObject effect_visual_prefab;
    public GameObject text_prefab;

    public tower[,] towers;
    public GameObject[,] tower_visuals;

    public List<enemy> enemies;
    public List<enemy_visual> enemy_visuals;
    public List<text_move> enemy_numbers;

    public GameObject[,] effect_visuals;

    public int wave_state;
    public GameObject cursor;
    public float turn_timer;

    void Start()
    {
        levelnum = manager.instance.curlevel;
        level = levels.get_level(levelnum);
        towers = new tower[12, 12];
        tower_visuals = new GameObject[12, 12];
        effect_visuals = new GameObject[12, 12];
        tiles = new tile[12, 12];
        endless = manager.instance.endless;

        List<Vector3Int> temp_path = new List<Vector3Int>();
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                tiles[i, j] = new tile();
                tower_visuals[i,j] = Instantiate(tower_visual_prefab, transform.position + new Vector3(i * 0.16f, j * 0.16f), transform.rotation,towerparent);
                effect_visuals[i,j] = Instantiate(effect_visual_prefab, transform.position + new Vector3(i * 0.16f, j * 0.16f), transform.rotation, effectparent);
                GameObject temp = Instantiate(tile_prefab, transform.position + new Vector3(i * 0.16f, j * 0.16f), transform.rotation,transform);
                if (level[i, j] == 0)
                {
                    temp.GetComponent<SpriteRenderer>().sprite = ground_sprite;
                }
                else
                {
                    temp.GetComponent<SpriteRenderer>().sprite = path_sprite;
                    temp_path.Add(new Vector3Int(i, j, level[i, j]));
                }
            }
        }
        path = new List<Vector2Int>();
        for (int i = 1; i <= temp_path.Count; i++)
        {
            for (int j = 0; j < temp_path.Count; j++)
            {
                if(temp_path[j].z == i)
                {
                    path.Add(new Vector2Int(temp_path[j].x,temp_path[j].y));
                    j = temp_path.Count;
                }
            }
        }


        enemies = new List<enemy>();
        enemy_visuals = new List<enemy_visual>();
        wave_state = 0;
        selectedtower = new Vector2Int(-1,-1);
        spawnen = true;
        //INITIALIZE TILES
        loops = 2;
        enemies = new List<enemy>();
        enemystats = new Queue<Vector3Int>();
        mousepos = new Vector2Int(0,0);
        manager.instance.switch_music(true);

        int[,] tempwavedata = levels.get_enemies(levelnum);

        numwaves = tempwavedata.GetLength(0);
        waverewards = new int[numwaves];
        wavelengths = 10;
        for (int i = 0; i<numwaves;i++)
        {
           waverewards[i] = tempwavedata[i, 2];
            for (int j = 0; j < wavelengths; j++)
            {
                    enemystats.Enqueue(new Vector3Int(levels.WaveConfig[tempwavedata[i, 0], j, 0], tempwavedata[i, 1], levels.WaveConfig[tempwavedata[i, 0], j, 1] ));
            }
        }
    }

    void Update()
    {
        goldcount.text = "Gold: " + resources.ToString();
        remaininglife.text = "Health: " + hp.ToString();   

        switch(wave_state)
        {
            case 0:
                break;
            case 1:
                do_enemy_turn();
                break;
            case 2:
                enemy_turn_visual();
                break;
            case 3:
                player_turn();
                break;
            case 4:
                player_turn_visual();
                break;
        }

        mousepos = new Vector2(Mathf.FloorToInt(Input.mousePosition.x / 48), Mathf.FloorToInt(Input.mousePosition.y / 48));
        if (mousepos.x < 12 && mousepos.y < 12)
        {
            cursor.transform.position = mousepos*.16f;
            if(!cursor.activeInHierarchy)
            {
                cursor.SetActive(true);
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (buildmode == 0)
                {
                    select();
                }
                else
                {
                    build();
                }
            }
        }
        else
        {
            cursor.SetActive(false);
        }

        if (!OVER)
        {
            if (hp <= 0)
            {
                finishlevel();
                OVER = true;
            }
        }  
    }

    void do_enemy_turn()
    {
       
        if (spawnen)
        {
            Vector3Int tstats = enemystats.Dequeue();
            if (tstats.x != 0)
            {
                for (int i = tstats.z; i > 0; i--)
                {
                    enemy tenemy = new enemy();
                    tenemy.set_en(tstats.x, tstats.y);
                    enemies.Add(tenemy);
                }
            }
            enspawned++;
            if (enspawned == wavelengths)
            {
                spawnen = false;
                curwave++;
                totalwaves++;
            }
        }
        move_enemies();
        turntimer = 1f;
        wave_state++;
    }

    void enemy_turn_visual()
    {
        turntimer -= Time.deltaTime;
        if(turntimer <= 0)
        {
            wave_state++;
        }
    }

    void player_turn()
    {
        doturretattack();
        wave_state++;
        turntimer = 1f;
    }

    void player_turn_visual()
    {
        if (enemies.Count == 0 && !spawnen)
        {
            manager.instance.switch_music(true);
            startbutton.interactable = true;
            startbutton.isOn = true;
            resources += waverewards[curwave - 1];
            wave_state = 0;
            turntimer = 1;
            enspawned = 0;
            spawnen = true;
            if (curwave == numwaves)
            {
                if (endless)
                {
                    loops += loops * 2;
                    repop();
                    curwave = 0;
                }
                else
                {
                    finishlevel();
                    OVER = true;
                }
            }
        }
        else
        {
            turntimer -= Time.deltaTime;
            if (turntimer <= 0)
            {
                wave_state = 1;
            }
        }
       
    }

    void resolve()
    {
       
    }

    void move_enemies()
    {
        List<Vector2Int> groups = new List<Vector2Int>();
        List<int> types = new List<int>();
        if (enemies.Count > 0)
        {
            List<enemy> toremove = new List<enemy>();
            foreach (var enemy in enemies)
            {
                Vector2Int group = new Vector2Int(enemy.position,0);
                if (!enemy.slowed)
                {
                    enemy.position += enemy.movespeed;
                }
                else
                {
                    if(enemy.movespeed == 1)
                    {
                        if(!enemy.halfmoved)
                        {
                            enemy.halfmoved = true;
                        }
                        else
                        {
                            enemy.position++;
                            enemy.halfmoved = false;
                            enemy.slowed = false;
                        }
                    }
                    else
                    {
                        enemy.position += Mathf.FloorToInt(enemy.movespeed * 0.5f);
                        enemy.slowed = false;
                    }
                }

                if (enemy.position >= path.Count)
                {
                    toremove.Add(enemy);
                }
                else if (enemy.health <= 0)
                {
                    toremove.Add(enemy);
                }
                else
                {
                    group.y = enemy.position;
                    groups.Add(group);
                    types.Add(enemy.entype);
                }
            }
            foreach (var enemy in toremove)
            {
                if (enemy.position >= path.Count)
                {
                    hp--;
                }
                enemies.Remove(enemy);
            }
            updatevis(types,groups);
        }
    }

    void updatevis(List<int> ts, List<Vector2Int> gs)
    {

        List<Vector2Int> temp = gs;
        temp = temp.Distinct().ToList();
        for(int i = 0; i < enemy_visuals.Count; i++ )
        {
            if(i<temp.Count)
            {
                enemy_visuals[i].gameObject.SetActive(true);
                enemy_numbers[i].gameObject.SetActive(true);
            }
            else
            {
                enemy_visuals[i].gameObject.SetActive(false);
                enemy_numbers[i].gameObject.SetActive(false);
            }
        }
        while(enemy_visuals.Count < temp.Count)
        {
            enemy_visuals.Add(Instantiate(enemy_visual_prefab, enemyparent).GetComponent<enemy_visual>());
            enemy_numbers.Add(Instantiate(text_prefab, textparent).GetComponent<text_move>());
        }

        for (int i = 0; i < temp.Count; i++)
        {
            enemy_visuals[i].lp = temp[i].x;
            enemy_visuals[i].np = temp[i].y;
            enemy_visuals[i].base_types = Vector3Int.zero;
        }
        for (int i = 0; i < ts.Count; i++)
        {
            for (int j = 0; j < temp.Count; j++)
            {
               if(gs[i].x == enemy_visuals[j].lp && gs[i].y == enemy_visuals[j].np)
                {
                    switch(ts[i])
                    {
                        case 1:
                            enemy_visuals[j].base_types.x++;
                            break;
                        case 2:
                            enemy_visuals[j].base_types.y++;
                            break;
                        case 3:
                            enemy_visuals[j].base_types.z++;
                            break;
                    }
                    j = enemy_visuals.Count;
                }
            }
        }
        for (int i = 0; i < temp.Count; i++)
        {
            enemy_visuals[i].find_type();
            enemy_visuals[i].advance(new Vector3(path[enemy_visuals[i].lp].x * 0.16f, path[enemy_visuals[i].lp].y * 0.16f), new Vector3(path[enemy_visuals[i].np].x * 0.16f, path[enemy_visuals[i].np].y * 0.16f));
            enemy_numbers[i].GetComponent<Text>().text = (enemy_visuals[i].base_types.x + enemy_visuals[i].base_types.y + enemy_visuals[i].base_types.z).ToString();
            enemy_numbers[i].advance(new Vector3(path[enemy_visuals[i].lp].x, path[enemy_visuals[i].lp].y), new Vector3(path[enemy_visuals[i].np].x, path[enemy_visuals[i].np].y));

        }

    }

    public void start_wave()
    {
        if (!startbutton.isOn)
        {
            wave_state = 1;
            startbutton.interactable = false;
            manager.instance.switch_music(false);
        }
    }

    public void setbuildmode(int type)
    {
        if(type == -1)
        {
            firebutton.isOn = true;
            waterbutton.isOn = true;
            earthbutton.isOn = true;
            airbutton.isOn = true;
            buildmode = 0;
            return;
        }

        if (selectedtower.x != -1)
        {
            desel();
        }//?????????????????

        if (buildmode == 0)
        {

            if (type == 1)
            {
                firebutton.isOn = false;
                waterbutton.isOn = true;
                earthbutton.isOn = true;
                airbutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Fire";
                towerinfo[2].text = "Reload: " + 2;
                towerinfo[3].text = "Damage: " + 1.5;
                towerinfo[4].text = "Aim: " + "Area";
                towerinfo[5].text = "Slows: " + "No";
                towerinfo[6].text = "Range: " + "3/3";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (type == 2)
            {
                earthbutton.isOn = false;
                waterbutton.isOn = true;
                airbutton.isOn = true;
                firebutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Earth";
                towerinfo[2].text = "Reload: " + 0;
                towerinfo[3].text = "Damage: " + 2;
                towerinfo[4].text = "Aim: " + "Single";
                towerinfo[5].text = "Slows: " + "No";
                towerinfo[6].text = "Range: " + "1";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (type == 3)
            {
                airbutton.isOn = false;
                earthbutton.isOn = true;
                waterbutton.isOn = true;
                firebutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Air";
                towerinfo[2].text = "Reload: " + 1;
                towerinfo[3].text = "Damage: " + 1;
                towerinfo[4].text = "Aim: " + "Line";
                towerinfo[5].text = "Slows: " + "No";
                towerinfo[6].text = "Range: " + "1/3";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (type == 4)
            {
                waterbutton.isOn = false;
                earthbutton.isOn = true;
                airbutton.isOn = true;
                firebutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Water";
                towerinfo[2].text = "Reload: " + 1;
                towerinfo[3].text = "Damage: " + 1.5;
                towerinfo[4].text = "Aim: " + "Single";
                towerinfo[5].text = "Slows: " + "Yes";
                towerinfo[6].text = "Range: " + "1";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }

        }
        else
        {
            if (buildmode != 1 && type == 1)
            {
                firebutton.isOn = false;
                waterbutton.isOn = true;
                earthbutton.isOn = true;
                airbutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Fire";
                towerinfo[2].text = "Reload: " + 2;
                towerinfo[3].text = "Damage: " + 1.5;
                towerinfo[4].text = "Aim: " + "Area";
                towerinfo[5].text = "Slows: " + "No";
                towerinfo[6].text = "Range: " + "3/3";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (buildmode != 2 && type == 2)
            {
                earthbutton.isOn = false;
                waterbutton.isOn = true;
                airbutton.isOn = true;
                firebutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Earth";
                towerinfo[2].text = "Reload: " + 0;
                towerinfo[3].text = "Damage: " + 2;
                towerinfo[4].text = "Aim: " + "Single";
                towerinfo[5].text = "Slows: " + "No";
                towerinfo[6].text = "Range: " + "1";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (buildmode != 3 && type == 3)
            {
                airbutton.isOn = false;
                earthbutton.isOn = true;
                waterbutton.isOn = true;
                firebutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Air";
                towerinfo[2].text = "Reload: " + 1;
                towerinfo[3].text = "Damage: " + 1;
                towerinfo[4].text = "Aim: " + "Line";
                towerinfo[5].text = "Slows: " + "No";
                towerinfo[6].text = "Range: " + "1/3";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (buildmode != 4 && type == 4)
            {
                waterbutton.isOn = false;
                earthbutton.isOn = true;
                airbutton.isOn = true;
                firebutton.isOn = true;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: " + "Water";
                towerinfo[2].text = "Reload: " + 1;
                towerinfo[3].text = "Damage: " + 1.5;
                towerinfo[4].text = "Aim: " + "Single";
                towerinfo[5].text = "Slows: " + "Yes";
                towerinfo[6].text = "Range: " + "1";
                towerinfo[7].text = "Weaken: " + "no";
                towerinfo[8].text = "Upgrade: ";
            }
            else if (buildmode == 1 && type == 1)
            {
                firebutton.isOn = true;
                waterbutton.isOn = true;
                earthbutton.isOn = true;
                airbutton.isOn = true;
                buildmode = 0;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: ";
                towerinfo[2].text = "Reload: ";
                towerinfo[3].text = "Damage: ";
                towerinfo[4].text = "Aim: ";
                towerinfo[5].text = "Slows: ";
                towerinfo[6].text = "Range: ";
                towerinfo[7].text = "Weaken: ";
                towerinfo[8].text = "Upgrade: ";
                return;                
            }
            else if (buildmode == 2 && type == 2)
            {
                earthbutton.isOn = true;
                waterbutton.isOn = true;
                airbutton.isOn = true;
                firebutton.isOn = true;
                buildmode = 0;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: ";
                towerinfo[2].text = "Reload: ";
                towerinfo[3].text = "Damage: ";
                towerinfo[4].text = "Aim: ";
                towerinfo[5].text = "Slows: ";
                towerinfo[6].text = "Range: ";
                towerinfo[7].text = "Weaken: ";
                towerinfo[8].text = "Upgrade: ";
                return;
            }
            else if (buildmode == 3 && type == 3)
            {
                airbutton.isOn = true;
                earthbutton.isOn = true;
                waterbutton.isOn = true;
                firebutton.isOn = true;
                buildmode = 0;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: ";
                towerinfo[2].text = "Reload: ";
                towerinfo[3].text = "Damage: ";
                towerinfo[4].text = "Aim: ";
                towerinfo[5].text = "Slows: ";
                towerinfo[6].text = "Range: ";
                towerinfo[7].text = "Weaken: ";
                towerinfo[8].text = "Upgrade: ";
                return;
            }
            else if (buildmode == 4 && type == 4)
            {
                waterbutton.isOn = true;
                earthbutton.isOn = true;
                airbutton.isOn = true;
                firebutton.isOn = true;
                buildmode = 0;
                towerinfo[0].text = "Level: ";
                towerinfo[1].text = "Type: ";
                towerinfo[2].text = "Reload: ";
                towerinfo[3].text = "Damage: ";
                towerinfo[4].text = "Aim: ";
                towerinfo[5].text = "Slows: ";
                towerinfo[6].text = "Range: ";
                towerinfo[7].text = "Weaken: ";
                towerinfo[8].text = "Upgrade: ";
                return;
            }
        }
        buildmode = type;
    }

    public void select()
    {
        if (towers[(int)mousepos.x, (int)mousepos.y] != null)
        {
            if(selectedtower.x != -1)
            {
                desel();
            }

            selectedtower.x = (int)mousepos.x;
            selectedtower.y = (int)mousepos.y;

            draweffect(-1, selectedtower.x, selectedtower.y);
            ubut.interactable = true;
            ubut.image.color = new Color(1, 1, 1, 1);
            if (towers[selectedtower.x, selectedtower.y].directional)
            {
                Lbut.interactable = true;
                Rbut.interactable = true;
            }
            else
            {
                Lbut.interactable = false;
                Rbut.interactable = false;
            }
            if (towers[selectedtower.x, selectedtower.y].singletarget)
            {
                firstlastbutton.interactable = true;
                firstbutton.color = new Color(1, 1, 1, 1);
            }
            else
            {
                firstlastbutton.interactable = false;
                firstbutton.color = new Color(0.78f, 0.78f, 0.78f, 0.5f);
            }

            if (towers[selectedtower.x, selectedtower.y].last)
            {
                if (firstlastbutton.isOn)
                {
                    towers[selectedtower.x, selectedtower.y].last = false;
                    firstlastbutton.isOn = false;
                }
            }
            else
            {
                if (!firstlastbutton.isOn)
                {
                    towers[selectedtower.x, selectedtower.y].last = true;
                    firstlastbutton.isOn = true;
                }
            }
            towerinfo[0].text = "Level: " + towers[(int)mousepos.x, (int)mousepos.y].level.ToString();
            towerinfo[1].text = "Type: " +  towers[(int)mousepos.x, (int)mousepos.y].name;
            towerinfo[2].text = "Reload: " +  towers[(int)mousepos.x, (int)mousepos.y].attackrate.ToString();
            towerinfo[3].text = "Damage: " +  towers[(int)mousepos.x, (int)mousepos.y].damage.ToString();
            towerinfo[4].text = "Aim: " +  towers[(int)mousepos.x, (int)mousepos.y].aim;
            towerinfo[5].text = "Slows: " +  towers[(int)mousepos.x, (int)mousepos.y].slow.ToString();
            towerinfo[6].text = "Range: " +  towers[(int)mousepos.x, (int)mousepos.y].range;
            towerinfo[7].text = "Weaken: " +  towers[(int)mousepos.x, (int)mousepos.y].weaken.ToString();
            towerinfo[8].text = "Upgrade: " + (100 +  towers[(int)mousepos.x, (int)mousepos.y].level * 100).ToString();
        }
        else
        {
            if (selectedtower.x != -1)
            {
                desel();
            }
        }
    }

    public void build()
    {
        if (towers[(int)mousepos.x, (int)mousepos.y] == null)
        {
            if (resources >= 100)
            {
                towers[(int)mousepos.x, (int)mousepos.y] = new tower();
                towers[(int)mousepos.x, (int)mousepos.y].setup(buildmode);
                tower_visuals[(int)mousepos.x, (int)mousepos.y].SetActive(true);
                tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(buildmode);
                tiles[(int)mousepos.x, (int)mousepos.y].hastower = true;
                resources -= 100;
            }
        }
        else if (resources >= 200)
        {
            if (towers[(int)mousepos.x, (int)mousepos.y].type == 1)
            {
                tiles[(int)mousepos.x, (int)mousepos.y].hastower = true;
                if (buildmode == 1)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(5, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(5);
                }
                else if (buildmode == 2)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(6, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(6);
                }
                else if (buildmode == 3)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(7, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(7);
                }
                else if (buildmode == 4)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(8, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(8);
                }
                resources -= 200;
            }
            else if (towers[(int)mousepos.x, (int)mousepos.y].type == 2)
            {
                tiles[(int)mousepos.x, (int)mousepos.y].hastower = true;
                if (buildmode == 1)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(6, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(6);
                }
                else if (buildmode == 2)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(9, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(9);
                }
                else if (buildmode == 3)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(10, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(10);
                }
                else if (buildmode == 4)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(11, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(11);
                }
                resources -= 200;
            }
            else if (towers[(int)mousepos.x, (int)mousepos.y].type == 3)
            {
                tiles[(int)mousepos.x, (int)mousepos.y].hastower = true;
                if (buildmode == 1)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(7, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(7);
                }
                else if (buildmode == 2)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(10, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(10);
                }
                else if (buildmode == 3)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(12, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(12);
                }
                else if (buildmode == 4)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(13, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(13);
                }
                resources -= 200;
            }
            else if (towers[(int)mousepos.x, (int)mousepos.y].type == 4)
            {
                tiles[(int)mousepos.x, (int)mousepos.y].hastower = true;
                if (buildmode == 1)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(8, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(8);
                }
                else if (buildmode == 2)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(11, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(11);
                }
                else if (buildmode == 3)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(13, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(13);
                }
                else if (buildmode == 4)
                {
                    towers[(int)mousepos.x, (int)mousepos.y].setup(14, towers[(int)mousepos.x, (int)mousepos.y].facing);
                    tower_visuals[(int)mousepos.x, (int)mousepos.y].GetComponent<teffect>().set(14);
                }
                resources -= 200;
            }
        }
    }

    public void desel()
    {
        towerinfo[0].text = "Level: " ;
        towerinfo[1].text = "Type: ";
        towerinfo[2].text = "Rate: ";
        towerinfo[3].text = "Damage: " ;
        towerinfo[4].text = "Aim: ";
        towerinfo[5].text = "Slows: " ;
        towerinfo[6].text = "Range: "  ;
        towerinfo[7].text = "Weaken: " ;
        towerinfo[8].text = "Upgrade: ";
        Lbut.interactable = false;
        Rbut.interactable = false;
        firstlastbutton.interactable = false;
        firstlastbutton.isOn = true;
        firstbutton.color = new Color(0.78f, 0.78f, 0.78f, 0.5f);
        ubut.interactable = false;
        ubut.image.color = new Color(0.78f, 0.78f, 0.78f, 0.5f);
        if (selectedtower.x != -1)
        {
            effect_visuals[selectedtower.x, selectedtower.y].SetActive(false);
        }
        selectedtower.x = -1;
    }

    public void doturretattack()
    {
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if(tiles[i,j].hastower)
                {
                    if(towers[i,j].turnstilattack == 0)
                    {
                        towers[i,j].turnstilattack = towers[i,j].attackrate;
                        if (towers[i,j].singletarget)
                        {
                            singletarget(i,j);
                        }
                        else if (towers[i,j].radial)
                        {
                            radial(i, j);
                        }
                        else if (towers[i,j].directional)
                        {
                            directional(i, j);
                        }
                    }
                    else
                    {
                        towers[i,j].turnstilattack--;
                    }
                }
            }
        }
    }

    public void draweffect(int type, int x, int y)
    {
        effect_visuals[x, y].SetActive(true);
        effect_visuals[x, y].GetComponent<effect>().trigger_effect(type);
    } 

    public void showtowereffect(int x, int y)
    {
        tower_visuals[x, y].GetComponent<teffect>().attack();
    }

    public void gotobuildmode()
    {
        main.SetActive(false);
        buildmenu.SetActive(true);
        if (selectedtower.x != -1)
        {
            desel();
        }
    }

    public void gotomain()
    {
        main.SetActive(true);
        buildmenu.SetActive(false);
        if(!infobutton.isOn)
        {
            infobutton.isOn = true;
            buildinfo();
        }
    }

    public void singletarget(int i, int j)
    {   
        int[] pathsinrange = new int[path.Count];
        for (int w = 0; w < pathsinrange.Length; w++)
        {
            pathsinrange[w] = -1;
        }
        for (int k = i - towers[i,j].rangex; k <= i + towers[i,j].rangex; k++)
            {
                if (k < 0)
                {

                }
                else if (k > 12)
                {
                    k = i + towers[i,j].rangex;
                }
                else
                {
                    for (int l = j - towers[i,j].rangey; l <= j + towers[i,j].rangey; l++)
                    {
                        if (l < 0)
                        {

                        }
                        else if (l > 12)
                        {
                            l = j + towers[i,j].rangey;
                        }
                        else
                        {
                            foreach (Vector2Int t in path)
                            {
                                if (t.x == k && t.y == l)
                                {
                                    pathsinrange[path.IndexOf(t)] = path.IndexOf(t);
                                }
                            }
                        }

                    }
                }
            }        
        if (!towers[i,j].last)
            {
                for (int k = pathsinrange.Length - 1; k >= 0; k--)
                {
                    if (pathsinrange[k] >= 0)
                    {
                        foreach (enemy e in enemies)
                        {
                            if (e.position == k)
                            {
                                if(towers[i,j].slow)
                                {
                                  e.slowed = true;
                                }
                                e.takedamage(towers[i,j].damage,1, towers[i,j].weaken);
                                k = -1;
                                draweffect(towers[i,j].type, path[e.position].x, path[e.position].y);
                                showtowereffect(i,j);
                            }
                        }

                    }
                }
            }
            else
            {
            for (int k = 0; k < pathsinrange.Length; k++)
            {
                if (pathsinrange[k] >= 0)
                {
                    foreach (enemy e in enemies)
                    {
                        if (e.position == k)
                        {
                            if (towers[i,j].slow)
                            {
                                e.slowed = true;
                            }
                            e.takedamage(towers[i,j].damage, 1, towers[i,j].weaken);
                            k = pathsinrange.Length;
                            draweffect(towers[i,j].type, path[e.position].x, path[e.position].y);
                            showtowereffect(i, j);
                        }
                    }
                }
            }
            }
        }   

    public void radial(int i, int j)
    {
        showtowereffect(i, j);
        int[] pathsinrange = new int[path.Count];
        for (int w = 0; w < pathsinrange.Length; w++)
        {
            pathsinrange[w] = -1;
        }
        for (int k = i - towers[i,j].rangex; k <= i + towers[i,j].rangex; k++)
        {
            if (k < 0)
            {

            }
            else if (k > 11)
            {
                k = i + towers[i,j].rangex;
            }
            else
            {
                for (int l = j - towers[i,j].rangey; l <= j + towers[i,j].rangey; l++)
                {
                    if (l < 0)
                    {

                    }
                    else if (l > 11)
                    {
                        l = j + towers[i,j].rangey;
                    }
                    else
                    {
                        if (k != i || l != j)
                        {
                            draweffect(towers[i,j].type, k, l);
                        }
                        foreach (Vector2Int t in path)
                        {
                            if (t.x == k && t.y == l)
                            {
                                pathsinrange[path.IndexOf(t)] = path.IndexOf(t);
                            }
                        }
                    }

                }
            }
        }
        for (int k = pathsinrange.Length - 1; k >= 0; k--)
        {
            if (pathsinrange[k] >= 0)
            {
                foreach (enemy e in enemies)
                {
                    if (e.position == k)
                    {
                        if (towers[i,j].slow)
                        {
                            e.slowed = true;
                        }
                        e.takedamage(towers[i,j].damage,2, towers[i,j].weaken);
                    }
                }

            }
        }
    }

    public void directional(int i, int j)
    {
        showtowereffect(i, j);
        int[] pathsinrange = new int[path.Count];
        for (int w = 0; w < pathsinrange.Length; w++)
        {
            pathsinrange[w] = -1;
        }
        if (towers[i,j].facing == 0 || towers[i,j].facing == 2)
        {
            for (int k = i - towers[i,j].rangey; k <= i + towers[i,j].rangey; k++)
            {
                if (k < 0)
                {

                }
                else if (k > 11)
                {
                    k = i + towers[i,j].rangey;
                }
                else
                {
                    if (towers[i,j].facing == 0)
                    {
                        for (int l = j + 1; l <= j + towers[i,j].rangex; l++)
                        {
                            if (l < 0)
                            {

                            }
                            else if (l > 11)
                            {
                                l = j + towers[i,j].rangex;
                            }
                            else
                            {
                                draweffect(towers[i,j].type, k, l);
                                foreach (Vector2Int t in path)
                                {
                                    if (t.x == k && t.y == l)
                                    {
                                        pathsinrange[path.IndexOf(t)] = path.IndexOf(t);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int l = j - 1; l >= j - towers[i,j].rangex; l--)
                        {
                            if (l < 0)
                            {

                            }
                            else if (l > 11)
                            {
                                l = j - towers[i,j].rangex;
                            }
                            else
                            {
                                draweffect(towers[i,j].type, k, l);
                                foreach (Vector2Int t in path)
                                {
                                    if (t.x == k && t.y == l)
                                    {
                                        pathsinrange[path.IndexOf(t)] = path.IndexOf(t);
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        else if (towers[i,j].facing == 1 )
        {
            for (int k = i + 1; k <= i + towers[i,j].rangex; k++)
            {
                if (k < 0)
                {

                }
                else if (k > 11)
                {
                    k = i + towers[i,j].rangex;
                }
                else
                {                  
                        for (int l = j - towers[i,j].rangey; l <= j + towers[i,j].rangey; l++)
                        {
                            if (l < 0)
                            {

                            }
                            else if (l > 11)
                            {
                                l = j + towers[i,j].rangey;
                            }
                            else
                            {
                                draweffect(towers[i,j].type, k, l);
                                foreach (Vector2Int t in path)
                                {
                                    if (t.x == k && t.y == l)
                                    {
                                    pathsinrange[path.IndexOf(t)] = path.IndexOf(t);
                                    }
                                }
                            }

                        }                 
                }
            }

        }
        else if (towers[i,j].facing == 3)
        {
            for (int k = i - 1; k >= i - towers[i,j].rangex; k--)
            {
                if (k < 0)
                {

                }
                else if (k > 11)
                {
                    k = i + towers[i,j].rangex;
                }
                else
                {
                    for (int l = j - towers[i,j].rangey; l <= j + towers[i,j].rangey; l++)
                    {
                        if (l < 0)
                        {

                        }
                        else if (l > 11)
                        {
                            l = j + towers[i,j].rangey;
                        }
                        else
                        {
                            draweffect(towers[i,j].type, k, l);
                            foreach (Vector2Int t in path)
                            {
                                if (t.x == k && t.y == l)
                                {
                                    pathsinrange[path.IndexOf(t)] = path.IndexOf(t);
                                }
                            }
                        }

                    }
                }
            }

        }
        for (int k = pathsinrange.Length - 1; k >= 0; k--)
        {
            if (pathsinrange[k] >= 0)
            {
                foreach (enemy e in enemies)
                {
                    if (e.position == k)
                    {
                        if (towers[i,j].slow)
                        {
                            e.slowed = true;
                        }
                        e.takedamage(towers[i,j].damage,3, towers[i,j].weaken);
                    }
                }

            }
        }
    }

    public void rotatetower(int c)
    {
        towers[selectedtower.x, selectedtower.y].facing += c;
        if(  towers[selectedtower.x, selectedtower.y].facing < 0)
        {
              towers[selectedtower.x, selectedtower.y].facing = 3;
        }
        else if (  towers[selectedtower.x, selectedtower.y].facing > 3)
        {
              towers[selectedtower.x, selectedtower.y].facing = 0;
        }
        if(  towers[selectedtower.x, selectedtower.y].facing == 0)
        {
            tower_visuals[selectedtower.x, selectedtower.y].transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (  towers[selectedtower.x, selectedtower.y].facing == 1)
        {
            tower_visuals[selectedtower.x, selectedtower.y].transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        if (  towers[selectedtower.x, selectedtower.y].facing == 2)
        {
            tower_visuals[selectedtower.x, selectedtower.y].transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (  towers[selectedtower.x, selectedtower.y].facing == 3)
        {
            tower_visuals[selectedtower.x, selectedtower.y].transform.rotation = Quaternion.Euler(0, 0, 90);
        }

    }

    public void levelup()
    {
        if (selectedtower.x != -1)
        {
            if (resources >= (100 + towers[selectedtower.x, selectedtower.y].level * 100))
            {
                resources -= (100 + towers[selectedtower.x, selectedtower.y].level * 100);
                towers[selectedtower.x, selectedtower.y].levelup();

                towerinfo[0].text = "Level: " +  towers[selectedtower.x, selectedtower.y].level.ToString();
                towerinfo[1].text = "Type: " +  towers[selectedtower.x, selectedtower.y].name;
                towerinfo[2].text = "Reload: " +  towers[selectedtower.x, selectedtower.y].attackrate.ToString();
                towerinfo[3].text = "Damage: " +  towers[selectedtower.x, selectedtower.y].damage.ToString();
                towerinfo[4].text = "Aim: " +  towers[selectedtower.x, selectedtower.y].aim;
                towerinfo[5].text = "Slows: " +  towers[selectedtower.x, selectedtower.y].slow.ToString();
                towerinfo[6].text = "Range: " +  towers[selectedtower.x, selectedtower.y].range;
                towerinfo[7].text = "Weaken: " +  towers[selectedtower.x, selectedtower.y].weaken.ToString();
                towerinfo[8].text = "Upgrade: " + (100 +  towers[selectedtower.x, selectedtower.y].level * 100).ToString();
            }
        }
    }

    public void finishlevel()
    {
        if (hp <= 0)
        {
            if (endless)
            {
                manager.instance.lost(levelnum, totalwaves);
            }
            else
            {
                manager.instance.lost(levelnum,0);
            }
        }
        else
        {
            int stars = 1;
            if (hp == 10)
            {
                stars = 3;
            }
            else if (hp > 5)
            {
                stars = 2;
            }
            manager.instance.levelcomplete(levelnum, stars);
        }
    }

    public void buildinfo()
    {
        if (infobutton.isOn)
        {
            info.SetActive(false);
        }
        if (!infobutton.isOn)
        {
            info.SetActive(true);
        }
    }

    public void repop()
    {
        int[,] tempwavedata = levels.get_enemies(levelnum);
        numwaves = tempwavedata.GetLength(0);
        waverewards = new int[numwaves];
        wavelengths = 10;
        for (int i = 0; i < numwaves; i++)
        {
            waverewards[i] = tempwavedata[i, 2];
            for (int j = 0; j < wavelengths; j++)
            {
                enemystats.Enqueue(new Vector3Int(levels.WaveConfig[tempwavedata[i, 0], j, 0], tempwavedata[i, 1] +loops, levels.WaveConfig[tempwavedata[i, 0], j, 1]));
            }
        }
    }

    public void menu()
    {
        manager.instance.pausegame();
    }
}
