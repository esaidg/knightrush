using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour
{
    public static bool isAttack = false;
    public static bool isMove = false;

   public void setMove()
   {
       isAttack = false;

       isMove = true;
   }

   public void setAttack()
   {
       isMove = false;

       isAttack = true;
   }
   
   public void backToStartMenu()
   {
       isAttack = false;
       isMove = false;
       SceneManager.LoadScene("StartMenu");
   }

    public void buttonEndTurn()
   {
       GameTurn.finishTurn();
       GameTurn.checkVictory();
   }
   
   public static void resetGUIBool()
   {
       isAttack = false;
       isMove = false;
   }
}
