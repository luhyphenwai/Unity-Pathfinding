using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathfinderInterface : MonoBehaviour
{
    public enum EditingMode{Walls, PathfinderObject, TargetPosition}
    public enum Pathfinder{AStar, Dikjstra}
    [Header("References")]
    public PathfindingGrid grid;
    public AStarPathfinder aStar;
    public DijkstraPathfinder dpath;
    public GameObject targetNode;
    public GameObject pathfinderObject;

    [Header("Editing Settings")]
    public Pathfinder currentPathfinder;
    public EditingMode currentMode;
    public TMP_Dropdown dd;
    public Slider s;
    public Slider gridSlider;
    public float gridSize;
    // Start is called before the first frame update
    void Start()
    {
        dd = GameObject.Find("Grid Editing Menu").GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        currentMode = (EditingMode)dd.value;
        EditGrid();

        if (gridSlider.value != gridSize){
            grid.gridSize = 10 * (int)gridSlider.value;
            grid.nodeSize = 0.5f / gridSlider.value;
            pathfinderObject.transform.localScale = new Vector2(grid.nodeSize, grid.nodeSize);
            targetNode.transform.localScale = new Vector2(grid.nodeSize, grid.nodeSize);
            grid.GenerateGrid();
        }
        gridSize = gridSlider.value;

        // Reset colors if trying to edit grid
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            for(int i = 0; i < grid.nodes.Length; i++){
                grid.nodes[i].updateNodeObject();
            }
        }
    }
    void EditGrid(){
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) && mousePosition.x > -5.15f){
            switch(currentMode){
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

        if (Input.GetMouseButton(1) && mousePosition.x > -5.15f){
            if (currentMode == EditingMode.Walls){
                grid.WorldPointToNode(mousePosition).walkable = true;
                grid.WorldPointToNode(mousePosition).updateNodeObject();
            }
        }
    }
    
    

    public void ResetGrid(){
        StopAllCoroutines();
        for (int i = 0; i < grid.nodes.Length; i++){
            grid.nodes[i].walkable = true;
            grid.nodes[i].updateNodeObject();
        }
    }

    [ContextMenu("A Star Pathfinding")]
    public void StartAStarPathfinder(){
        StopAllCoroutines(); 
        
        
        StartCoroutine
            (aStar.FindPath(grid.WorldPointToNode(pathfinderObject.transform.position), 
                            grid.WorldPointToNode(targetNode.transform.position), s.value));
    }

    [ContextMenu("Dijkstra Pathfinding")]
    public void StartDijkstraPathfinder(){
        StopAllCoroutines(); 
        
        StartCoroutine
            (dpath.FindPath(grid.WorldPointToNode(pathfinderObject.transform.position), 
                            grid.WorldPointToNode(targetNode.transform.position), s.value));
    }
    public void GenerateMaze(){
        StopAllCoroutines();
        grid.StartCoroutine(grid.GenerateMaze());
    }
}
