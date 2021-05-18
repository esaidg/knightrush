using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Parent class to control move of all players' troops and enemy troops
*/
public class MoveParent : MonoBehaviour
{
    //# of tiles a troop can move
    public int move = 3;
    //# of tiles a troop can jump
    public float jump = 1;
    //How fast a troop moves to anothe tile
    public float moveSpeed = 2;
    //Show openTiles if troop is not moving
    public bool moving = false;
    public float jumpVelocity = 4.5f;
    public bool turn = false;

    public int health = 100;
    public int attack = 75;
    public bool isDead = false;
    public int range = 5;

    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;
    Vector3 jumpTarget;
    //how fast troop moves from tile to tile
    Vector3 velocity = new Vector3();
    //direction troop is going towards
    Vector3 heading = new Vector3();

    //Sounds
    public AudioSource sounds;
    public AudioClip arrow;
    public AudioClip dying;
    public AudioClip swordstrike;
    public AudioClip walking;

    //List of tiles to move to
    List<TileInfoScipt> moveTiles = new List<TileInfoScipt>();
    GameObject[] tiles;
    //To create path during troop move
    Stack<TileInfoScipt> path = new Stack<TileInfoScipt>();
    //Tile troop is currently standing on
    TileInfoScipt currentTile;
    //to store the height / 2 of the tile, calculate where a troop will be standing on top of a tile
    float halfheight = 0;

    public TileInfoScipt actTargetTile;

    //Troop cannot move anymore if hasMove = true. Turn is over is hasAttacked is true;
    public bool hasAttacked = false;
    public bool hasMoved = false;
    public bool attacking = false;

    protected void init()
    {
        swordstrike = Resources.Load<AudioClip>("Sword Swish 1");
        sounds = GetComponent<AudioSource>();
        GetComponentInChildren<HealthScript>().maxHealth(health);
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfheight = GetComponent<Collider>().bounds.extents.y;
        GameTurn.addTroops(this);
    }

    public void getCurrent()
    {
        currentTile = getTarget(gameObject);
        //Debug.Log("tile is " + currentTile);
        currentTile.current = true;
    }

    public TileInfoScipt getTarget(GameObject target)
    {   
        //Debug.Log("get target " + target);
        RaycastHit hit;
        TileInfoScipt tile = null;
        Debug.DrawRay(target.transform.position, -Vector3.up, Color.black, 1);
        if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            //Debug.Log("tile" + tile);
            tile = hit.collider.GetComponent<TileInfoScipt>();
        }
        return tile;
    }

    public void ComputeAdjLists(float jump, TileInfoScipt target)
    {
        foreach(GameObject tile in tiles)
        {
            TileInfoScipt t = tile.GetComponent<TileInfoScipt>();
            t.FindTiles(jump, target);
        }
    }

    public void FindOpenTiles()
    {
        ComputeAdjLists(jump, null);
        getCurrent();

        Queue<TileInfoScipt> tileq = new Queue<TileInfoScipt>();
        tileq.Enqueue(currentTile);
        currentTile.isChecked = true;

        while(tileq.Count > 0)
        {
            TileInfoScipt t = tileq.Dequeue();
            moveTiles.Add(t);
            t.openTile = true;
            if(t.distance < move)
            {
                foreach(TileInfoScipt tile in t.adj)
                {
                    if(!tile.isChecked)
                    {
                        tile.parent = t;
                        tile.isChecked = true;
                        tile.distance = 1 + t.distance;
                        tileq.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void MoveToTile(TileInfoScipt tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        TileInfoScipt next = tile;
        while(next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void turnStart()
    {
        turn = true;
    }
    public void turnEnd()
    {
        turn = false;
        GameGUI.isAttack = false;
        GameGUI.isMove = false;
    }

    public void Move()
    {
        if(path.Count > 0)
        {
            TileInfoScipt t = path.Peek();
            Vector3 target = t.transform.position;
            //calculate troop's position on top of target tile
            target.y += halfheight + t.GetComponent<Collider>().bounds.extents.y;
            if(Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool needToJump = transform.position.y != target.y;
                if(needToJump)
                {
                   performJump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                //Tile center reached
                transform.position = target;
                path.Pop();
                fallingDown = false;
                jumpingUp = false;
                movingEdge = false;
                jumpTarget = Vector3.zero;
            }
        }
        else
        {
            RemoveOpenTiles();
            moving = false;
            hasMoved = true;
            Debug.Log("Move has been completed");
            GameGUI.resetGUIBool();
            checkTurnManager();
            //GameTurn.finishTurn();

        }
    }

    protected void RemoveOpenTiles()
    {
        if(currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach(TileInfoScipt tile in moveTiles)
        {
            tile.Reset();
        }
        moveTiles.Clear();
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }
    
    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    void performJump(Vector3 target)
    {
        if(fallingDown)
        {
            fallDown(target);
        }
        else if(jumpingUp)
        {
            jumpUp(target);
        }
        else if(movingEdge)
        {
           moveToEdge();
        }
        else
        {
            prepareJump(target);
        }
    }

    void fallDown(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;
        if(transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;
            Vector3 temp = transform.position;
            temp.y = target.y;
            transform.position = temp;
            velocity = new Vector3();
        }
    }
    void jumpUp(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;
        if(transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }
    void moveToEdge()
    {
        if(Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;
            velocity /= 3.0f;
            velocity.y = 1.5f;
        }
    }
    void prepareJump(Vector3 target)
    {
        float targety = target.y;
        target.y = transform.position.y;
        CalculateHeading(target);
        if(transform.position.y > targety)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;
            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        }
        else 
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;
            velocity = heading * moveSpeed / 3.0f;
            float difference = targety - transform.position.y;
            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    protected TileInfoScipt FindEndTile(TileInfoScipt t)
    {
        Stack<TileInfoScipt> tempPath = new Stack<TileInfoScipt>();
        TileInfoScipt next = t.parent;
        while(next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }
        if(tempPath.Count <= move)
        {
            return t.parent;
        }

        TileInfoScipt tileEnd = null;
        for(int i = 0; i<= move; i++)
        {
            tileEnd = tempPath.Pop();
        }
        return tileEnd;
    }
    protected TileInfoScipt lowestF(List<TileInfoScipt> list)
    {
        TileInfoScipt low = list[0];
        foreach(TileInfoScipt t in list)
        {
            if(t.f < low.f)
            {
                low = t;
            }
        }

        list.Remove(low);
        return low;
    }
    public void FindPath(TileInfoScipt target)
    {
        ComputeAdjLists(jump, target);
        getCurrent();
        List<TileInfoScipt> openList = new List<TileInfoScipt>();
        List<TileInfoScipt> closeList = new List<TileInfoScipt>();
        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

        while(openList.Count > 0)
        {
            TileInfoScipt t = lowestF(openList);

            closeList.Add(t);
            if(t == target)
            {
                actTargetTile = FindEndTile(t);
                MoveToTile(actTargetTile);
                return;
            }
            foreach(TileInfoScipt tile in t.adj)
            {
                if(closeList.Contains(tile))
                {
                    //nothing, processed
                }
                else if(openList.Contains(tile))
                {
                    float temp = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    if(temp < tile.g)
                    {
                        tile.parent = t;
                        tile.g = temp;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    tile.parent = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;
                    openList.Add(tile);
                }
            }
        }
    }

    public void FindPathAttack(TileInfoScipt target)
    {
        ComputeAdjLists(jump, target);
        getCurrent();
        List<TileInfoScipt> openList = new List<TileInfoScipt>();
        List<TileInfoScipt> closeList = new List<TileInfoScipt>();
        openList.Add(currentTile);
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f =  currentTile.h;
        while(openList.Count > 0)
        {
            TileInfoScipt t = lowestF(openList);

            closeList.Add(t);
            if(t == target)
            {
                return;
            }
            foreach(TileInfoScipt tile in t.adj)
            {
                if(closeList.Contains(tile))
                {
                    //nothing
                }
                else if(openList.Contains(tile))
                {
                    float temp = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    if(temp < tile.g)
                    {
                        tile.parent = t;
                        tile.g = temp;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    tile.parent = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;
                    openList.Add(tile);
                }
            }
        }
    }

     public void performAttackOn(GameObject target)
    { 
        playSwordStrikeSound();
        Vector3 temp = target.transform.position;
        CalculateHeading(temp);
        transform.forward = heading;
        attacking = true;
        GetComponentInChildren<TroopAnimation>().checkAttack();
        Debug.Log("attacking: " + attacking);

        target.GetComponent<MoveParent>().health -= attack;
        Debug.Log(this + " has dealt " + attack + " DMG to " + target);
        target.GetComponentInChildren<HealthScript>().setHealth(target.GetComponent<MoveParent>().health);
        if(target.GetComponent<MoveParent>().health <= 0)
        {
            target.GetComponent<MoveParent>().isDead = true;
            Destroy(target.gameObject);
            Debug.Log(target + " has been defeated");
        }
        attacking = false;
        Debug.Log("attacking: " + attacking);
        GameTurn.finishTurn();
        GameTurn.checkVictory();
    }

    //Check if the turn should be finished
    public void checkTurnManager()
    {
        //Debug.Log("Troop Status: " + this);
        //Debug.Log("Checking turn manager...");
        //Debug.Log("has attacked: " + this.hasAttacked);
        //Debug.Log("has moved: " + this.hasMoved);
        if(this.tag == "Enemy")
        {
            hasMoved = false;
            hasAttacked = false;
            GetComponent<MoveEnemy>().FindAttackTilesEnemy();
            //GameTurn.finishTurn();
        }
        if(this.tag == "Troop" && hasMoved)
        {
            //hasMoved = false;
            hasAttacked = false;
            //GameTurn.finishTurn();
        }
    }

    //Sound functions
    public void playSwordStrikeSound()
    {
        Debug.Log("Playing sword strike");
        sounds.clip = swordstrike;
        sounds.PlayOneShot(sounds.clip);
    }

    public void playWalkingSound()
    {
       
    }

    public void playDyingSound()
    {
      
    }

    public void playArrowSound()
    {
        
    }
}
