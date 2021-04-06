using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathfindingGrid : MonoBehaviour
{
    [Header("References")]
    public LayerMask unWalkableLayer;
    public Node[] nodes;

    // Toggle to display gizmos (MAY LAG)
    public bool displayGizmos;

    [Header("Grid Settings")]
    public int gridSize;
    [Min(0.1f)]
    public float nodeSize;
    public float wallDistance;
    private void Awake() {
        GenerateGrid(gridSize, nodeSize);
    }

    // Only runs in editor and runs every time script is edited
    private void Update() {
        if (!Application.isPlaying){
            GenerateGrid(gridSize, nodeSize);
        }
    }

    // Generate grid nodes
    void GenerateGrid(float size, float distance){
        List<Node> nodeList = new List<Node>();
        // Set grid size
        float gridWidth = gridSize * nodeSize;

        // For every x column
        for (float x = -gridSize; x <= gridSize; x ++){
            // For every y row
            for (float y = -gridSize; y <= gridSize; y ++){
                // Set node position to current position
                Vector2 position = new Vector2(x*nodeSize,y*nodeSize);

                // Check if node position is walkable
                bool walkable = !Physics2D.CircleCast(position, wallDistance, Vector3.forward, unWalkableLayer);

                // Create new node
                Node node = new Node(walkable, position);
                node.SetGridCoords(new Vector2(x,y));
                nodeList.Add(node);
            }
        }
        nodes = nodeList.ToArray();
        
    }
    
    // Draw grid nodes
    private void OnDrawGizmos() {
        if (displayGizmos){
            if (nodes.Length != 0){
                for (int i = 0; i < nodes.Length; i ++){
                    if (nodes[i].walkable){
                        Gizmos.color = Color.green;
                    }   else {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawSphere(nodes[i].position, nodeSize/3);
                }
            }
        }
        

    }

    
}
