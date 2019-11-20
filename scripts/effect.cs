using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effect : MonoBehaviour
{
    public Animator anim;
    public float cd;
    // Use this for initialization

    public void Update()
    {
        if (cd > 0)
        {
            cd -= Time.deltaTime;
            if(cd<0)
            {
                cd = 0;
            }
                
        }
    }

    public void trigger_effect(int type)
    {
        if (cd == 0f)
        {
            cd = 0.1f;
            switch (type)
            {
                case 1:
                    anim.SetTrigger("fire");
                    break;
                case 2:
                    anim.SetTrigger("earth");
                    break;
                case 3:

                    anim.SetTrigger("air");
                    break;
                case 4:

                    anim.SetTrigger("water");
                    break;
                case 5:

                    anim.SetBool("inferno", true);
                    break;
                case 6:

                    anim.SetBool("lava", true);
                    break;
                case 7:

                    anim.SetBool("lightning", true);
                    break;

                case 8:
                    anim.SetBool("acid", true);
                    break;

                case 9:
                    anim.SetBool("metal", true);
                    break;

                case 10:
                    anim.SetBool("sand", true);
                    break;

                case 11:
                    anim.SetBool("swamp", true);

                    break;

                case 12:
                    anim.SetBool("tornado", true);
                    break;

                case 13:
                    anim.SetBool("wave", true);
                    break;

                case 14:
                    anim.SetBool("ocean", true);
                    break;
                case -1:
                    anim.SetBool("select", true);
                    break;
            }
        }
    }
}
