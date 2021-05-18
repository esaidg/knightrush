using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MoveParent
{
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
       init(); 
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
            Debug.Log("Enemy turn...");
            FindTarget();
            CalculatePath();
            FindOpenTiles();
            actTargetTile.target = true;
        }
        else
        {
            Move();
        }
    }

    void CalculatePath()
    {
        TileInfoScipt targetTile = getTarget(target);
        FindPath(targetTile);
    }

    void FindTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Troop");
        GameObject nearest = null;
        float dist = Mathf.Infinity;
        foreach(GameObject obj in targets)
        {   
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if(d < dist)
            {
                dist = d;
                nearest = obj;
            }
        }
        target = nearest;
    }

    public void FindAttackTilesEnemy()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Troop");
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
        if(dist <= range)
        {
            target = attackTile;
            performAttackOn(target);
        }
        else 
        {
            Debug.Log("No targets found withing range of " + this + " Range: " + range);
            GameTurn.finishTurn();
            GameTurn.checkVictory();
        }
    }
}
