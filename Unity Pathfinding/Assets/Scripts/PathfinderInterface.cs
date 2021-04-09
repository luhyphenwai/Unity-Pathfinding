using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderInterface : MonoBehaviour
{
    public enum EditingMode{Walls, PathfinderObject, TargetPosition}
    public EditingMode mode;
    public PathfindingGrid grid;
    public AStarPathfinder aStar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EditGrid();
    }
    void EditGrid(){
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0)){
            switch(mode){
                case EditingMode.Walls:
                    grid.WorldPointToNode(mousePosition).walkable = false;
                    grid.WorldPointToNode(mousePosition).updateNodeObject();
                    break;
            }
        }
    }

    public void GenerateMaze(){
        StopAllCoroutines();
        grid.StartCoroutine(grid.GenerateMaze());
    }
}
