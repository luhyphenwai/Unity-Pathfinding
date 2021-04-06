using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    [Header("References")]
    public PathfindingGrid grid;
    public Node[] nodes;
    public Vector2 startingPosition;
    public Vector2 targetPosition;


    private void Start() {
        grid = GameObject.Find("Pathfinding Grid").GetComponent<PathfindingGrid>();
        List<Node> nodeList = new List<Node>();
        for (int i = 0; i < grid.nodes.Length; i++){
            Node node = new Node(grid.nodes[i].walkable, grid.nodes[i].position);
            node.SetGridCoords(grid.nodes[i].gridCoords);
            nodeList.Add(node);
        }
        nodes = nodeList.ToArray();
    }
    private void Update() {
        startingPosition = transform.position;
    }
    public Node[] FindPath(Node startingNode, Node targetNode){
        // Initialize lists
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        openNodes.Add(startingNode);

        Node current = openNodes[0];
        current.gCost = 0;

        // Loop while not at target node
        while (openNodes.Count > 0){
            current = openNodes[0];
            for (int i = 0; i < openNodes.Count; i++){
                if (openNodes[i].fCost <= current.fCost){
                    if (openNodes[i].hCost < current.hCost){
                        current = openNodes[i];
                    }
                }
            }

            // Move current node to closed nodes
            openNodes.Remove(current);
            closedNodes.Add(current);

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
                        neighbourNodes.Add(node);
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
                    }
                }
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
        path.Reverse();;
        return path.ToArray();
    }

    // Convert world point into node
    public Node WorldPointToNode(Vector3 position){ 
        Node closestNode = nodes[0];
        float closestNodeDistance = Vector2.Distance(position, nodes[0].position);
        for (int i = 0; i < nodes.Length; i ++){
            if (Vector2.Distance(position, nodes[i].position) < closestNodeDistance){
                closestNode = nodes[i];
                closestNodeDistance = Vector2.Distance(position, nodes[i].position);
            }
        }
        return closestNode;
    }

    // Drawing point on mouse
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (Application.isPlaying){
            Gizmos.DrawSphere(WorldPointToNode(Camera.main.ScreenToWorldPoint(Input.mousePosition)).position, 0.3f);
        }
    }
}
