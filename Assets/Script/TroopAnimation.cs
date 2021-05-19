using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAnimation : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkanimation();
    }

    void checkanimation()
    {
        if(GetComponentInParent<MoveParent>().moving)
        {
            anim.SetBool("isRunning", true);
        }
        else if(!GetComponentInParent<MoveParent>().moving)
        {
            anim.SetBool("isRunning", false);
        }
    }

    public void attackAnimation()
    {
        anim.Play("infantry_04_attack_A");
    }
}
