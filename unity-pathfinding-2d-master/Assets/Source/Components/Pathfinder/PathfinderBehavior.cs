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

        private Vector3 destination;

        private void Awake()
        {
            destination = new Vector3(0f, 0f, 0f);
            navigationMesh = navigationMeshObject?.GetComponent<NavigationMeshComponent>()
                ?? throw new UnityException("Navigation mesh is missing required Navigation Mesh Component");

            pathMapper = new AStarPathMapper(navigationMesh);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))                                                            // 마우스 클릭 위치 받아오고 그 클릭한 위치를 destination으로 설정
            {
                Vector3 mouse = Input.mousePosition;
                Ray castPoint = Camera.main.ScreenPointToRay(mouse);

                destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                
            }

            // Try moving solids around and checking out how the path updates
            lastMappedPath = pathMapper.FindPath(transform.position, destination, canMoveDiagonally);   // 출발지 목적지 ???
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


    }
}
