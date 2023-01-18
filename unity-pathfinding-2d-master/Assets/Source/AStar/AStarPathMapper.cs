using Assets.Source.Components.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.AStar
{
    public class AStarPathMapper
    {

        private NavigationMeshComponent navigationMesh;

        private Node[][] nodes;
        private int gridWidth;
        private int gridHeight;

        public AStarPathMapper(NavigationMeshComponent navMesh) 
        {
            this.navigationMesh = navMesh;
        }

        public List<Node> FindPath(Vector2 startPosition, Vector2 targetPosition, bool allowDiagonalMovement)           // 뭐 길을 찾는 시스템?
        {
            // Perform all pathing operations on a clone of currrent A* grid
            nodes = navigationMesh.CloneGrid();
            gridWidth = navigationMesh.GridWidth;
            gridHeight = navigationMesh.GridHeight;

            // Find the nodes closest to our destination positions
            // if these are off the grid, will return the edge of the grid as close as it can get
            var startNodePosition =  navigationMesh.FindNearestNodeIndex(startPosition);                                // findnearestnodeindex는 입력된 좌표의 가까운 값을 입력함
            var targetNodePosition = navigationMesh.FindNearestNodeIndex(targetPosition);
            var startNode = nodes[startNodePosition.ix][startNodePosition.iy];
            // For the target node, if the destination is a solid node, find the nearest non-solid neighbor node instead    //대상 노드의 경우 대상이 솔리드 노드인 경우 대신 가장 가까운 솔리드가 아닌 이웃 노드를 찾습니다 ???
            var targetNode = FindNearestOpenNode(nodes[targetNodePosition.ix][targetNodePosition.iy], startNode);

            //navigationMesh.UpdateTiles(new Vector3(startNodePosition.ix, startNodePosition.iy, 0));

            //startNode.IsSolid = false;
            //Debug.Log("startNode.IsSolid : " + startNode.IsSolid);
            // The "open list" is the list of nodes that we need to visit
            var openList = new List<Node>() { startNode };                                                              // 방문 해야 하는 노드

            // The "closed set" is the list of nodes that we have visited already                                       // 이미 방문한 노드
            var closedList = new List<Node>();

            // Loop until we have no more nodes to visit        // 더이상 반복할 노드가 없을때 까지 반복
            while (openList.Count > 0) 
            {
                // Grab whatever the next node on the list is       // 목록의 다음 노드가 무엇이든 가져옵니다.
                var currentNode = openList.First();

                // Iterate through the open list starting from the second element to figure out what node we want to visit next     //두 번째 요소부터 시작하여 열린 목록을 반복하여 다음에 방문할 노드를 파악합니다
                // If there is no other element, this whole thing is skipped                                                        // 다른 요소가 없으면 이 전체를 건너뜁니다.
                foreach (var node in openList.Skip(1)) 
                {
                    // if this node appears to get us to our destination quicker, visit that boy first                              // 이 노드가 우리를 목적지로 더 빨리 데려다 줄 것 같으면 먼저 그 소년을 방문하십시오.
                    if (node.FCost <= currentNode.FCost && node.HCost < currentNode.HCost)
                    {
                        currentNode = node;
                    }
                }

                // mark this node as visited, remove it from the open list and add it to the closed list                            // 이 노드를 방문한 것으로 표시하고 열린 목록에서 제거하고 닫힌 목록에 추가합니다.    
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // if we find our target node               // 대상 노드를 찾으면 즉 target 노드를 찾으면
                if (currentNode == targetNode)
                {
                    // Crawl backwards to retrieve the total path       // 뒤로 크롤링 전체 경로 검색
                    return TraceFinalPath(startNode, targetNode);
                }

                foreach (Node neighbor in FindNeighborNodes(currentNode, false)) 
                {
                    // Ignore any solid or previously visited nodes         // 솔리드 또는 이전에 방문한 노드 무시
                    if ((neighbor != targetNode && neighbor.IsSolid) || closedList.Contains(neighbor)) 
                    {
                        continue;
                    }

                    // For this implementation of A*, the GCost is the linear distance between node indices //이 A* 구현의 경우 GCost는 노드 인덱스 간의 선형 거리입니다.
                    // Add the distance between currentNode and this particular neighbor                    // currentNode와 이 특정 이웃 사이의 거리를 추가합니다.
                    //var moveCost = currentNode.GCost + GetDistance(currentNode, neighbor); //<- This might not be right(?)    // 이것은 틀릴수 있다.
                    var moveCost = currentNode.GCost + GetDistance(currentNode, targetNode);

                    // Check if this neighbor node should be where we visit next        // 이 이웃 노드가 다음에 방문해야 하는 곳인지 확인
                    if (moveCost < neighbor.GCost || !openList.Contains(neighbor)) 
                    {
                        neighbor.GCost = moveCost;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            // If we managed to get here, it means we have visited every node and could not find a path.    // 여기에 올 수 있었다면 모든 노드를 방문했지만 경로를 찾을 수 없다는 의미입니다.
            // Return an empty list, and leave it up to the implementor to decide what to do                // 빈 목록을 반환하고 수행할 작업을 결정하도록 구현자에게 맡깁니다.
            return new List<Node>();
        }

        // Find the linear distance between two indices in the node array           //노드 배열에서 두 인덱스 사이의 선형 거리 찾기
        private float GetDistance(Node node1, Node node2)
        { 
            var x = Mathf.Abs(node1.XIndex - node2.XIndex);
            var y = Mathf.Abs(node2.YIndex - node2.YIndex);
            return x + y;
        }

        // Follow the parent nodes backwards from destination => start      // 목적지에서 뒤로 부모 노드를 따르십시오 => 시작
        private List<Node> TraceFinalPath(Node startNode, Node targetNode)
        {
            var path = new List<Node>();

            var currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            // reverse the list so we are going from start to finish        // 우리가 처음부터 끝까지 갈 수 있도록 목록을 뒤집으십시오.
            path.Reverse();

            return path;
        }

        private IEnumerable<Node> FindNeighborNodes(Node node, bool includeDiagonals = true)
        {
            for (var ix = -1; ix < 2; ix++)
            {
                for (var iy = -1; iy < 2; iy++) 
                {
                    var nx = node.XIndex + ix;
                    var ny = node.YIndex + iy;

                    if (nx >= 0 && nx <= nodes.Length-1) 
                    {
                        if (ny >= 0 && ny <= nodes[nx].Length-1)
                        {
                            if (includeDiagonals)
                            {
                                // return all neighbor nodes who are not me         // 내가 아닌 모든 이웃 노드 반환
                                if (ix != 0 || iy != 0)
                                { 
                                    yield return nodes[nx][ny];
                                }
                            }
                            else 
                            {
                                // Only return this node if it is not a diagonal                            // 대각선이 아닌 경우에만 이 노드를 반환합니다.
                                // Diagonals are defined as coordinates relative to this current node       // 대각선은 이 현재 노드에 상대적인 좌표로 정의됩니다.
                                // where the absolute value of both x and y index is non-zero               // 여기서 x 및 y 인덱스의 절대값은 0이 아닙니다.
                                if (Mathf.Abs(ix) == 0 || Mathf.Abs(iy) == 0)
                                {
                                    // return all nodes who are not me
                                    if (ix != 0 || iy != 0)
                                    {
                                        yield return nodes[nx][ny];
                                    }
                                }                                
                            }                            
                        }                        
                    }                    
                    
                }
            }         
        }

        // Gets the actual total distance in worlds space between two points        // 두 지점 사이의 세계 공간에서 실제 총 거리를 가져옵니다
        private float GetPhysicalDistance(Node node1, Node node2) =>Vector2.Distance(node1.Center, node2.Center);

        // recursively seek out a valid non-solid node          // 유효한 비고체 노드를 재귀적으로 찾습니다.
        private Node FindNearestOpenNode(Node node, Node startNode, Node originalNode = null, List<Node> visited = null)
        {
            // If this node is non-solid, return it right away // 이 노드가 견고하지 않으면 바로 반환
            if (!node.IsSolid)
            {
                return node;
            }

            // If sourceNode is null, that means this is the first layer of recursion       // sourceNode가 null이면 이것이 재귀의 첫 번째 계층임을 의미합니다.
            var sourceNode = originalNode ?? node;

            // Begin keeping track of visited neighbors                                     // 방문한 이웃 추적 시작
            if (visited == null)
            {
                visited = new List<Node>();
            }
            visited.Add(node);

            // Grab each surrounding nodes around this node, but order by its physical distance         // 이 노드 주변의 각 주변 노드를 잡고 물리적 거리에 따라 정렬합니다.
            // ordering it like that assures us that the first node we check will be the closest to us  // 그렇게 주문하면 우리가 확인하는 첫 번째 노드가 우리에게 가장 가깝다는 것을 확신할 수 있습니다.
            var neighbors = FindNeighborNodes(node, true)
                            .Where(n => visited != null && !visited.Contains(n))
                            .OrderBy(n=> GetPhysicalDistance(n, startNode))
                            .OrderBy(n=> GetPhysicalDistance(n, sourceNode));

            // Check each neighbor for its nearest non-solid node                                       // 가장 가까운 비고체 노드에 대해 각 이웃을 확인합니다.
            // Remember, this will return the node itself if the node isn't solid                       // 노드가 견고하지 않은 경우 노드 자체를 반환합니다.
            foreach (Node neighbor in neighbors) 
            {
                var firstNonSolidNode = FindNearestOpenNode(neighbor, startNode, sourceNode, visited);
                if (firstNonSolidNode != null) 
                {
                    return firstNonSolidNode;
                }                    
            }
            // searched all neighbors, found no solids                                                  // 모든 이웃을 검색했지만 고체를 찾지 못했습니다.
            return null;
        }
    }
}
