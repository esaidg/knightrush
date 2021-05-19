using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTroop : MoveParent
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
            RemoveOpenTiles();
            hasMoved = false;
            return;
        }
        if(!moving)
        {
            if(turn){getCurrent();}
            if(GameGUI.isAttack)
            {
                FindAttackTiles();
            }
            else if(GameGUI.isMove && !hasMoved)
            {
                FindOpenTiles();
                CheckMouse();
            }
        }
        else
        {
            Move();
        }
    }

    void CheckMouse()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "Tile")
                {
                    TileInfoScipt t = hit.collider.GetComponent<TileInfoScipt>();
                    if(t.openTile)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }

    //For attacking
    void CalculatePath()
    {
        if(target == null)
        {
            Debug.Log("No enemys found");
            hasMoved = false;
            GameTurn.finishTurn();
        }
        TileInfoScipt targetTile = getTarget(target);
        FindPathAttack(targetTile);
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
