using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderInterface : MonoBehaviour
{
    public enum EditingMode{Walls, PathfinderObject, TargetPosition}
    public enum Pathfinder{AStar, Dikjstra}
    public Pathfinder currentPathfinder;
    public GameObject pathfinderObject;
    public EditingMode mode;
    public PathfindingGrid grid;
    public AStarPathfinder aStar;
    public GameObject targetNode;
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
                case EditingMode.TargetPosition:
                    targetNode.transform.position = grid.WorldPointToNode(mousePosition).position;
                    break;
                case EditingMode.PathfinderObject:
                    pathfinderObject.transform.position = grid.WorldPointToNode(mousePosition).position;
                    break;
                    
            }
        }
    }

    [ContextMenu("A Star Pathfinding")]
    public void StartAStarPathfinder(){
        StartCoroutine
            (aStar.FindPath(grid.WorldPointToNode(pathfinderObject.transform.position), 
                            grid.WorldPointToNode(targetNode.transform.position)));
    }
    public void GenerateMaze(){
        StopAllCoroutines();
        grid.StartCoroutine(grid.GenerateMaze());
    }
}
