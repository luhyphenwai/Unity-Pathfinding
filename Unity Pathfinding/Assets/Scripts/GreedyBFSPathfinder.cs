using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyBFSPathfinder : MonoBehaviour
{
    [Header("References")]
    public PathfindingGrid grid;
    public Node[] nodes;
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
        // Load list of nodes
        nodes = grid.nodes;
        for(int i = 0; i < nodes.Length; i++){
            nodes[i].updateNodeObject();
        }

        // Initialize lists
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        openNodes.Add(startingNode);

        Node current = openNodes[0];
        current.gCost = 0;

        // Loop while not at target node
        while (openNodes.Count > 0 ){
            current = openNodes[0];
            for (int i = 0; i < openNodes.Count; i++){
                if (openNodes[i].hCost < current.hCost){
                    current = openNodes[i];
                }
            }

            // Move current node to closed nodes
            openNodes.Remove(current);
            closedNodes.Add(current);
            current.nodeObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            current.nodeObject.GetComponent<Animator>().SetTrigger("Selected");

            // Check for target node
            if (current == targetNode){
                break;
            }

            // Find neighbour nodes
            List<Node> neighbourNodes = new List<Node>();
            for (int i = 0; i < nodes.Length; i++){
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
                if (!neighbour.walkable || closedNodes.Contains(neighbour)){
                    continue;
                }
                // H cost
                float h = Vector2.Distance(neighbour.position, targetNode.position);
                // G cost
                float g = current.gCost + Vector2.Distance(neighbour.position, current.position);
                // Change neighbour node settings 
                if (g < neighbour.gCost || !openNodes.Contains(neighbour)){
                    neighbour.hCost = h;
                    neighbour.gCost = g;
                    neighbour.parent = current;
                    if (!openNodes.Contains(neighbour)){
                        openNodes.Add(neighbour);
                        neighbour.nodeObject.GetComponent<SpriteRenderer>().color = Color.blue;
                        neighbour.nodeObject.GetComponent<Animator>().SetTrigger("Selected");
                    }
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
            current = current.parent;
        }

        // Reset nodes
        for (int i = 0; i < nodes.Length; i++){
            nodes[i].hCost = 0;
            nodes[i].gCost = 0;
            nodes[i].parent = null;
        }

        // Return completed path
        path.Reverse();
        finalPath = path.ToArray();

        for(int i = 0; i < path.Count; i++){
            path[i].nodeObject.GetComponent<SpriteRenderer>().color = Color.red;
            path[i].nodeObject.GetComponent<Animator>().SetTrigger("Selected");
            yield return new WaitForSeconds(0.01f);
        }
        
    }

}
