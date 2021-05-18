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
        checkAttack();
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

    public void checkAttack()
    {
        if(GetComponentInParent<MoveParent>().attacking)
        {
            anim.SetBool("isAttacking", true);
        }
        else if(!GetComponentInParent<MoveParent>().attacking)
        {
            anim.SetBool("isAttacking", false);
        }
    }
}
