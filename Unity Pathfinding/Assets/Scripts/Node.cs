using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GameObject nodeObject;
    public bool walkable;
    public Vector2 position;
    public Vector2 gridCoords;
    public Node parent;

    public float movementCost = 0;
    public float gCost = 0;
    public float hCost = 0;
    public Node(bool _walkable, Vector3 _position){
        walkable = _walkable;
        position = _position;
    }
    public void SetGridCoords(Vector2 coords){
        gridCoords = coords;
    }

    public float fCost{
        get {
            return gCost + hCost + movementCost;
        }
    }

    public void updateNodeObject(){
        if (walkable){
            nodeObject.GetComponent<SpriteRenderer>().color = Color.white;
        }   else {
            nodeObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    
}
