using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfinder : MonoBehaviour
{
    [Header("References")]
    public PathfindingGrid grid;
    public List<Node> nodes;
    public Vector2 startingPosition;
    public Vector2 targetPosition;
    public Node[] finalPath;

    private void Start() {
        grid = GameObject.Find("Grid").GetComponent<PathfindingGrid>();
        transform.localScale = new Vector2(grid.nodeSize, grid.nodeSize);
        transform.position = grid.WorldPointToNode(transform.position).position;
    }
    private void Update() {
        startingPosition = transform.position;
    }
    public IEnumerator FindPath(Node startingNode, Node targetNode, float speed){
        
        // Initialize lists
        nodes = new List<Node>();
        for(int i = 0; i< grid.nodes.Length; i++){
            nodes.Add(grid.nodes[i]);
        }

        for(int i = 0; i < nodes.Count; i++){
            nodes[i].updateNodeObject();
        }

        // Reset nodes
        for (int i = 0; i < nodes.Count; i++){
            nodes[i].hCost = 0;
            nodes[i].gCost = 999999;
            nodes[i].parent = null;
        }


        Node current = startingNode;
        current.gCost = 0;

        // Loop while not at target node
        while (nodes.Count > 0 ){
            current = nodes[0];
            for (int i = 0; i < nodes.Count; i++){
                if (nodes[i].gCost < current.gCost){
                    current = nodes[i];
                }
            }

            // Move current node to closed nodes
            nodes.Remove(current);
            current.nodeObject.GetComponent<SpriteRenderer>().color = Color.yellow;

            // Check for target node
            if (current == targetNode){
                break;
            }

            // Find neighbour nodes
            List<Node> neighbourNodes = new List<Node>();
            for (int i = 0; i < nodes.Count; i++){
                Node node = nodes[i];
                if (Mathf.Abs(node.gridCoords.x - current.gridCoords.x) <= 1){
                    if (Mathf.Abs(node.gridCoords.y - current.gridCoords.y) <= 1){
                        // Do not include diagonal nodes
                        if (node.gridCoords.x == current.gridCoords.x || node.gridCoords.y == current.gridCoords.y){
                            neighbourNodes.Add(node);
                        }   
                    }
                }
            }

            // Check neighbours
            foreach (Node neighbour in neighbourNodes){
                // Check if neighbour is viable
                if (!neighbour.walkable || !nodes.Contains(neighbour)){
                    continue;
                }
                // G cost
                float g = current.gCost + Vector2.Distance(neighbour.position, current.position);

                // Change neighbour node settings 
                if (g < neighbour.gCost || neighbour.gCost == 0){
                    neighbour.gCost = g;
                    neighbour.parent = current;
                    neighbour.nodeObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }
            } 
            if (speed > 0){
                yield return new WaitForSeconds(speed);
            }
        }

        // Retrace path back
        List<Node> path = new List<Node>();
        while (current != startingNode){
            path.Add(current);
            current.nodeObject.GetComponent<SpriteRenderer>().color = Color.red;
            current = current.parent;
        }

        // Reset nodes
        for (int i = 0; i < nodes.Count; i++){
            nodes[i].hCost = 0;
            nodes[i].gCost = 0;
            nodes[i].parent = null;
        }

        // Return completed path
        path.Reverse();
        finalPath = path.ToArray();
    }
}
