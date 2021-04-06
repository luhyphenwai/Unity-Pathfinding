using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderFollow : MonoBehaviour
{
    public float speed;
    public AStarPathfinder pathfinder;
    public Node[] path;
    public int currentNode;
    public bool showGizmos;
    private void Awake() {
        pathfinder = gameObject.GetComponent<AStarPathfinder>();
    }

    private void Update() {
        // Generate new path from mouse position
        if (Input.GetMouseButton(0)){
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Node mouseNode = pathfinder.WorldPointToNode(mousePosition);
            Node startingNode = pathfinder.WorldPointToNode(transform.position);
            if (mouseNode.walkable){
                path = pathfinder.FindPath(startingNode, mouseNode);
                currentNode = 0;
            }
        }

        // Move through path
        if (path != null && Vector2.Distance(transform.position, path[path.Length-1].position) > 0.1){
            transform.position = Vector2.MoveTowards(transform.position, path[currentNode].position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, path[currentNode].position) < 0.1){
                currentNode++;
            }
        }   else {
            currentNode = 0;
        }
    }

    // Draw path line
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (Application.isPlaying && showGizmos){
            if (path != null){
                for (int i = currentNode; i < path.Length; i++){
                    if (i < path.Length-1 && currentNode != 0){
                        Gizmos.DrawLine(path[i].position, path[i+1].position);
                    }
                }
            }
        }
    }
 
}
