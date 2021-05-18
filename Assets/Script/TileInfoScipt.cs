using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfoScipt : MonoBehaviour
{
    //If troop (player or enemy) is on the square
    public bool current = false;
    //The square (player or enemy) is moving to
    public bool target = false;
    //Available tiles to move to(selectable)
    public bool openTile = false;
    //Available tiles players can move over
    public bool freeTile = true;
    //Tiles that are attackable
    //List of tiles next to the current tile
    public List<TileInfoScipt> adj = new List<TileInfoScipt>();
    public bool isChecked = false;
    public TileInfoScipt parent = null;
    public int distance = 0;

    public float f = 0;
    public float g = 0;
    public float h = 0;


    public void FindTiles(float jump, TileInfoScipt target)
    {
        Reset();
        Check(Vector3.forward, jump, target);
        Check(-Vector3.forward, jump, target);
        Check(Vector3.right, jump, target);
        Check(-Vector3.right, jump, target);
    }

    /*
    Set variables to default values
    */
    public void Reset()
    {
        current = false;
        target = false;
        openTile = false;
        freeTile = true;
        isChecked = false;
        parent = null;
        distance = 0;
        f = 0;
        g = 0;
        h = 0;
        adj.Clear();
    }

    public void Check(Vector3 dir, float jump, TileInfoScipt target)
    {
        Vector3 halfs = new Vector3(0.25f, (1 + jump)/2, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + dir, halfs);
        foreach(Collider x in colliders)
        {
            TileInfoScipt tile = x.GetComponent<TileInfoScipt>();
            if(tile != null && tile.freeTile)
            {
                RaycastHit hit;
                if(!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                {
                    adj.Add(tile);
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(current)
       {
           GetComponent<Renderer>().material.color = Color.cyan;
       } 
       else if(target)
       {
           GetComponent<Renderer>().material.color = Color.green;
       }
       else if(openTile)
       {
           
           GetComponent<Renderer>().material.color = Color.yellow;
       }
       else
       {
           
           GetComponent<Renderer>().material.color = new Color32(3,31,0,255);
       }
    }
}
