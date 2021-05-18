using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurn : MonoBehaviour
{
    static Dictionary<string, List<MoveParent>> troops = new Dictionary<string, List<MoveParent>>();
    static Queue<string> turnq = new Queue<string>();
    static Queue<MoveParent> teamq = new Queue<MoveParent>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(teamq.Count == 0)
        {
            teamQueue();
        }
    }

    //Begins the turn for the team
    public static void beginTurn()
    {
        Debug.Log("Starting turn...");
        if(teamq.Count > 0)
        {
            Debug.Log(teamq.Peek() + "'s turn");
            if(teamq.Peek() == null)
            {
                teamq.Dequeue();
            }
            teamq.Peek().turnStart();
        }
    }
    
    //End the teams turn
    public static void finishTurn()
    {
        Debug.Log("Ending turn...");
        MoveParent troop = teamq.Dequeue();
        troop.turnEnd();
        if(teamq.Count > 0)
        {
            beginTurn();
        }
        else
        {
            string team = turnq.Dequeue();
            turnq.Enqueue(team);
            teamQueue();
        }
    }
    static void teamQueue()
    {
        List<MoveParent> listTeam = troops[turnq.Peek()];
        foreach(MoveParent troop in listTeam)
        {
            //Debug.Log("adding troop " + troop + " to queue");
            teamq.Enqueue(troop);
        }

        beginTurn();
    }

    public static void addTroops(MoveParent troop)
    {
        List<MoveParent> list;
        if(!troops.ContainsKey(troop.tag))
        {
            list = new List<MoveParent>();
            troops[troop.tag] = list;
            if(!turnq.Contains(troop.tag))
            {
                turnq.Enqueue(troop.tag);
            }
        }
        else
        {
            list = troops[troop.tag];
        }
        list.Add(troop);
    }

    public static void checkVictory()
    {
        Debug.Log("Checking victory...");
        if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
        {
            Debug.Log("Player wins!");
        }
        else if(GameObject.FindGameObjectsWithTag("Troop").Length <=0)
        {
            Debug.Log("Enemy wins !");
        }
        //Debug.Log("Enemy count: " + GameObject.FindGameObjectsWithTag("Enemy").Length);
        //Debug.Log("Player count: " + GameObject.FindGameObjectsWithTag("Troop").Length);
    }
}
