using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileMaterialScript : MonoBehaviour
{
    // Start is called before the first frame update
   [MenuItem("Tools/Assign Tile Material")]
   public static void AssignTileMaterial()
   {
       GameObject[] tile = GameObject.FindGameObjectsWithTag("Tile");
       Material material = Resources.Load<Material>("Tile");
       foreach(GameObject obj in tile)
       {
           obj.GetComponent<Renderer>().material = material;
       }
   }

   [MenuItem("Tools/Assign Tile Script")]
    public static void AssignTileInfo()
    {
        GameObject[] tile = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject obj in tile)
        {
            obj.AddComponent<TileInfoScipt>();
        }
    }
    
}
