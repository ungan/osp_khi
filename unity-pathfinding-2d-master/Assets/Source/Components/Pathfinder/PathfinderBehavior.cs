using Assets.Source.AStar;
using Assets.Source.Components.NavMesh;
using System.Collections;
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
        private bool stop = true;
        private bool ispathfinder = false;

        private PathfinderBehavior pfib;

        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.tag == "pfi")
            {
                pfib = collision.gameObject.GetComponent<PathfinderBehavior>();
                if(pfib.lastMappedPath.Count < lastMappedPath.Count)
                {
                    stop = true;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "pfi")
            {
                if (pfib.lastMappedPath.Count < lastMappedPath.Count)
                {
                    stop = false;
                }
            }
        }
        private void Awake()
        {

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
            }
            
            StartCoroutine("pathfinder");

            
            //Debug.Log("lastMappedPath.Count" + lastMappedPath.Count);
            if(stop) move();
            //movecell_count = 0;                                                                         // 최적화 안된경우 최적화 안된경우에 사용할것
            //destination = c_destination;
            //lastMappedPath = pathMapper.FindPath(transform.position, destination, canMoveDiagonally);   // 출발지 목적지 ???          // 작은거 쓸때는 안에 넣을것 큰거는 밖에?
            // Try moving solids around and checking out how the path updates                           // 솔리드를 이동하고 경로가 어떻게 업데이트되는지 확인하십시오.

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

                    //Debug.Log("lastMappedPath " + lastMappedPath[movecell_count]);
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
        private IEnumerator pathfinder()       
        {
            
            if (ispathfinder == true) yield break;

            ispathfinder = true;
            movecell_count = 0;                                                                         // 최적화? 속도를 올리고 싶은경우에 사용할것
            destination = c_destination;
            lastMappedPath = pathMapper.FindPath(transform.position, destination, canMoveDiagonally);   // 출발지 목적지 ???          // 작은거 쓸때는 안에 넣을것 큰거는 밖에?
            yield return new WaitForSeconds(0.2f);

            ispathfinder = false;
            
            yield return null;
        }

    }


}
