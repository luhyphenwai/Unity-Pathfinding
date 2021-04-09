using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathfindingGrid : MonoBehaviour
{
    [Header("References")]
    public LayerMask unWalkableLayer;
    public Node[] nodes;
    public List<GameObject> nodeObjects;

    // Toggle to display gizmos (MAY LAG)
    public bool displayGizmos;

    [Header("Grid Settings")]
    public int gridSize;
    [Min(0.1f)]
    public float nodeSize;
    public float wallDistance;
    public GameObject nodeObject;
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
        
        // Clear node objects
        if (Application.isPlaying){
            for (int i = 0; i < nodeObjects.Count; i++){
                Destroy(nodeObjects[i]);
            }
        }
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
                if (Application.isPlaying){
                    node.nodeObject = Instantiate(nodeObject, position, Quaternion.identity);
                    node.nodeObject.transform.localScale = new Vector2(nodeSize, nodeSize);
                    node.nodeObject.transform.parent = transform;
                    nodeObjects.Add(node.nodeObject);
                    node.updateNodeObject();
                }
                nodeList.Add(node);
            }
        }
        nodes = nodeList.ToArray();
        
    }

    public IEnumerator GenerateMaze(){
        for (int i = 0; i < nodes.Length; i++){
            
        }

        yield return new WaitForEndOfFrame();
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

            Gizmos.color = Color.blue;
            if (Application.isPlaying){
                Gizmos.DrawSphere(WorldPointToNode(Camera.main.ScreenToWorldPoint(Input.mousePosition)).position, 0.3f);
            }
        }
        

    }

    
}
