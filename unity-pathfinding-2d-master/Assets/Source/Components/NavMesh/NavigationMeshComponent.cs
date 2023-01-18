using Assets.Source.AStar;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Components.NavMesh
{
    public class NavigationMeshComponent : MonoBehaviour
    {
        [Tooltip("How many tiles going horizontally")]          // 가로로 가는 타일 수
        [SerializeField]
        private int gridWidth = 20;
        public int GridWidth => gridWidth;

        [Tooltip("How many tiles going vertically")]            // 수직으로 가는 타일 수
        [SerializeField]
        private int gridHeight = 15;
        public int GridHeight => gridHeight;


        [Tooltip("The size of each tile in units.  Should be large enough for a pathfinder to pass through without clipping the edge of corners.")]             // 각 타일의 크기(단위)입니다. 길잡이가 모서리를 자르지 않고 통과할 수 있을 만큼 충분히 커야 합니다.
        [SerializeField]
        private float tileSize = 0.5f;

        [Tooltip("Recalculates the entire navigation mesh every frame.  Leave this on if things in your level move around, " +                                  // 매 프레임마다 전체 내비게이션 메시를 다시 계산합니다. 레벨의 사물이 움직이면 이 설정을 켜두세요.
            "or set to false if the level is static for a performance boost.")]                                                                                 // 또는 성능 향상을 위해 레벨이 정적이면 false로 설정하십시오.
        [SerializeField]
        private bool syncEveryFrame = true;

        private Node[][] nodes;

        private void Start()
        {
            nodes = new Node[gridWidth][];
            GenerateNavigationMesh();
        }

        // Generate the initial nav mesh        // 초기 탐색 메시 생성
        private void GenerateNavigationMesh()
        {
            for (var ix = 0; ix < gridWidth; ix++)          //
            {
                nodes[ix] = new Node[gridHeight];
                for (var iy = 0; iy < gridHeight; iy++)         //
                {
                    var center = new Vector2(x: (ix  * tileSize) + transform.position.x, y: (iy * tileSize) + transform.position.y);
                    var isSolid = TileContainsSolid(center);

                    nodes[ix][iy] = new Node(center, ix, iy, isSolid);
                }
            }            
        }

        // Returns true if any solid exists in a radius of 'tileSize' around the specified center position      // 지정된 중심 위치 주변의 'tileSize' 반경에 솔리드가 있으면 true를 반환합니다.
        private bool TileContainsSolid(Vector2 center) => Physics2D.OverlapArea(new Vector2(center.x - (tileSize / 2), center.y - (tileSize / 2)),
                                                                                new Vector2(center.x + (tileSize / 2), center.y + (tileSize / 2))) !=null;   
        

        private void Update()
        {
            if (syncEveryFrame)
            {
                //UpdateTiles();
            }
        }

        /// <summary>
        /// Used to clone the 2d array of nodes so we can modify them safely        // 안전하게 수정할 수 있도록 노드의 2d 배열을 복제하는 데 사용됩니다.
        /// </summary>
        /// <returns></returns>
        public Node[][] CloneGrid() => nodes.Select(n => n.ToArray()).ToArray();


        // For each tile, update its "isSolid" status.                              // 각 타일에 대해 "isSolid" 상태를 업데이트합니다.
        public void UpdateTiles(Vector3 obj) 
        {
            if (nodes != null)
            {   
                foreach (var nodeRow in nodes)
                {
                    foreach (var node in nodeRow)
                    {
                        if (node.XIndex != obj.x && node.YIndex != obj.y)
                        {
                            node.IsSolid = TileContainsSolid(node.Center);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a tuple which contains the position of the nearest node.  If the specified position is beyond the boundaries        // 가장 가까운 노드의 위치를 ​​포함하는 튜플을 반환합니다. 지정된 위치가 경계를 넘어선 경우
        /// of the navigation mesh, it will return the closest node it can get within the confines of the grid.                         // 내비게이션 메시의 그리드 범위 내에서 얻을 수 있는 가장 가까운 노드를 반환합니다.         
        /// </summary>
        /// <param name="worldPosition">Position in world space </param>
        /// <returns>x / y position of the node</returns>
        public (int ix, int iy) FindNearestNodeIndex(Vector2 worldPosition)
        {
            // Basically reverse the function to plot the grid square to figure out its index
            var row = (int)Mathf.Clamp(Mathf.Round((worldPosition.x - transform.position.x) / tileSize), 0, gridWidth-1);
            var col = (int)Mathf.Clamp(Mathf.Round((worldPosition.y - transform.position.y) / tileSize), 0, gridHeight-1);
            return (row, col);
        }


        // todo: change to this line because otherwise that grid will get annoying      // 그렇지 않으면 해당 그리드가 짜증날 수 있으므로 이 줄로 변경하십시오
        //private void OnDrawGizmosSelected()
        private void OnDrawGizmos()
        {
            if (nodes != null)
            {
                for (var ix = 0; ix < nodes.Length; ix++)
                {
                    for (var iy = 0; iy < nodes[0].Length; iy++) 
                    {
                        var node = nodes[ix][iy];

                        if (node.IsSolid)
                        {
                            Gizmos.color = Color.red;
                        }
                        else
                        {
                            Gizmos.color = Color.grey;
                        }

                        GizmosUtils.GizmosUtils.DrawText(GUI.skin, node.ToString(), node.Center, fontSize: 8);
                        Gizmos.DrawWireCube(node.Center, new Vector2(tileSize*0.95f,tileSize*0.95f));
                    }                
                }
            }
        }

    }
}
