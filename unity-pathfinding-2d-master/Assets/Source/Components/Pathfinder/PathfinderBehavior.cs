using Assets.Source.AStar;
using Assets.Source.Components.NavMesh;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Components.Pathfinder
{
    public class PathfinderBehavior : MonoBehaviour
    {

        [SerializeField]
        private GameObject navigationMeshObject;

        [SerializeField]
        private LayerMask clickableLayers;

        [SerializeField]
        private bool canMoveDiagonally = true;
        
        private NavigationMeshComponent navigationMesh;
        private AStarPathMapper pathMapper;

        private List<Node> lastMappedPath;

        private GameObject target_obj;
        private Vector3 destination;
        private Vector3 c_destination;

        private int movecell_count = 0;
        private float speed = 1f;

        private bool moving = false;

        private void Awake()
        {
            //partyManager = GameObject.Find("Party").GetComponent<PartyManager>();  //파티(플레이어)찾기 SJM

            target_obj = GameObject.Find("target_obj");
            destination = new Vector3(target_obj.transform.position.x, target_obj.transform.position.y, target_obj.transform.position.z);
            navigationMesh = navigationMeshObject?.GetComponent<NavigationMeshComponent>()
                ?? throw new UnityException("Navigation mesh is missing required Navigation Mesh Component");

            pathMapper = new AStarPathMapper(navigationMesh);

            //lastMappedPath = pathMapper.FindPath(transform.position, destination, canMoveDiagonally);   // 출발지 목적지 ???
        }

        private void Update()
        {
            c_destination = new Vector3(target_obj.transform.position.x, target_obj.transform.position.y, target_obj.transform.position.z);
            /*
            if (Input.GetMouseButtonDown(0))                                                            // 마우스 클릭 위치 받아오고 그 클릭한 위치를 destination으로 설정
            {
                Vector3 mouse = Input.mousePosition;
                Ray castPoint = Camera.main.ScreenPointToRay(mouse);

                c_destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                
            }*/

            if(Mathf.Round(c_destination.x) != Mathf.Round(destination.x) || Mathf.Round(c_destination.y) != Mathf.Round(destination.y) || Mathf.Round(c_destination.z) != Mathf.Round(destination.z) || moving == false)
            {
                movecell_count = 0;
                destination = c_destination;
            }
            lastMappedPath = pathMapper.FindPath(transform.position, destination, canMoveDiagonally);   // 출발지 목적지 ???
            move();
            // Try moving solids around and checking out how the path updates

        }

        private void FixedUpdate()
        {
            
        }

        private void OnDrawGizmos()                                                                     // 가야하는길에 노란색 원 그려줌
        {
            Gizmos.color = Color.yellow;

            if (lastMappedPath != null && lastMappedPath.Any()) 
            {
                foreach (Node node in lastMappedPath)                                                   // 아마 처음부터 끝까지의 노드를 불러오는 반복문
                {
                    Gizmos.DrawWireSphere(node.Center, 0.2f);            
                }          
            }
        }

        private void move()
        {
            if (Mathf.Round(transform.position.x) != Mathf.Round(destination.x) || Mathf.Round(transform.position.y) != Mathf.Round(destination.y) || Mathf.Round(transform.position.z) != Mathf.Round(destination.z))
            {
                moving = true;
                if (lastMappedPath != null && lastMappedPath.Any())
                {
                    Node node2;

                    if (movecell_count == 0) node2 = lastMappedPath[0];

                    //if (movecell_count == lastMappedPath.Count) break;  // 다 움직였을경우 멈추게?


                    node2 = lastMappedPath[movecell_count];
                        
                    transform.position = Vector3.MoveTowards(transform.position, node2.Center, speed * Time.deltaTime);

                    if (Mathf.Round(transform.position.x) == Mathf.Round(node2.Center.x) && Mathf.Round(transform.position.y) == Mathf.Round(node2.Center.y))
                    {
                        if (movecell_count < lastMappedPath.Count-1) movecell_count++;
                    }

                }
            }
            else
            {
                moving = false;
            }
        }

    }
}
