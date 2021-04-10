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
    public PathfinderInterface pi;

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
        pi = GameObject.Find("Interface").GetComponent<PathfinderInterface>();
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
        // Reset nodes
        for (int i = 0; i < nodes.Length; i++){
            nodes[i].walkable = false;
            nodes[i].updateNodeObject();
        }

        // References
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        Node current = nodes[0];
        openNodes.Add(current);
        pi.pathfinderObject.transform.position = current.position;

        // Find the neighbour nodes
        for (int i = 0; i < nodes.Length; i++){
            Node node = nodes[i];
            if (Mathf.Abs(current.gridCoords.x - node.gridCoords.x) == 2 || Mathf.Abs(current.gridCoords.y - node.gridCoords.y) == 2){
                // Do not include diagonal nodes
                if (node.gridCoords.x == current.gridCoords.x || node.gridCoords.y == current.gridCoords.y){
                    // Do not add already added nodes
                    if (!openNodes.Contains(node)){
                        openNodes.Add(node);
                    }
                }   
                
            }
        }
        
        while (openNodes.Count > 0) {
            
            openNodes.Remove(current);
            closedNodes.Add(current);
            current.walkable = true;
            current.updateNodeObject();

            // Find the neighbour nodes
            List<Node> neighbourNodes = new List<Node>();
            for (int i = 0; i < nodes.Length; i++){
                Node node = nodes[i];
                if (Mathf.Abs(current.gridCoords.x - node.gridCoords.x) <= 2 && Mathf.Abs(current.gridCoords.y - node.gridCoords.y) <= 2){
                    // Keep 2 node distance 
                    if (Vector2.Distance(current.gridCoords, node.gridCoords) > 1){

                        // Do not include diagonal nodes
                        if (node.gridCoords.x == current.gridCoords.x || node.gridCoords.y == current.gridCoords.y){
                            // Do not add already added nodes
                            if (!closedNodes.Contains(node)){
                                neighbourNodes.Add(node);
                            }
                            if (!openNodes.Contains(node)){
                                openNodes.Add(node);
                            }
                        }   
                    }
                    
                }
            }

            // If there are avaliable neighbour nodes
            if (neighbourNodes.Count > 0 ){
                // Pick random node
                Node node = neighbourNodes[Random.Range(0,neighbourNodes.Count)];
                
                // Find nodes in between selected node and current node
                for (int i = 0; i < nodes.Length; i++){
                    float x = (node.gridCoords.x + current.gridCoords.x)/2;
                    float y = (node.gridCoords.y + current.gridCoords.y)/2;
                    if (nodes[i].gridCoords.x == x){
                        if (nodes[i].gridCoords.y == y){
                            // Do not include diagonal nodes
                            if (nodes[i].gridCoords.x == current.gridCoords.x || nodes[i].gridCoords.y == current.gridCoords.y){
                                // Remove node in between
                                nodes[i].walkable = true;
                                nodes[i].updateNodeObject();
                                print("yes");
                            }   
                        }
                    }
                }

                openNodes.Remove(node);
                current = node;
            }
            // Backtrack and find old nodes
            else {
                // Find neighbour nodes of old closed nodes
                for (int i = closedNodes.Count -1; i >= 0; i--){
                    for (int j = 0; j < nodes.Length; j++){
                        Node node = nodes[j];
                        if (Mathf.Abs(closedNodes[i].gridCoords.x - node.gridCoords.x) <= 2 && 
                            Mathf.Abs(closedNodes[i].gridCoords.y - node.gridCoords.y) <= 2){
                            // Keep 2 node distance 
                            if (Vector2.Distance(closedNodes[i].gridCoords, node.gridCoords) > 1){

                                // Do not include diagonal nodes
                                if (node.gridCoords.x == closedNodes[i].gridCoords.x || node.gridCoords.y == closedNodes[i].gridCoords.y){
                                    // Do not add already added nodes
                                        if (!closedNodes.Contains(node)){
                                            current = closedNodes[i];
                                        }
                                }
                            }   
                            
                        }
                    }

                }
            }
            
            yield return new WaitForEndOfFrame();
        }


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
