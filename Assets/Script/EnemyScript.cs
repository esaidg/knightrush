using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MoveParent
{
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);
        if(!turn)
        {
            return;
        }
        if(!moving)
        {
            FindOpenTiles();
        }
        else
        {
            Move();
        }
    }

    //For attacking
    void CalculatePath()
    {
        TileInfoScipt targetTile = getTarget(target);
        FindPath(targetTile);
    }
    void FindAttackTiles()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject attackTile = null;
        float dist = Mathf.Infinity;
        foreach(GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if(d < dist)
            {
                dist = d;
                attackTile = obj;
            }
        }

        target = attackTile;
    }
}
