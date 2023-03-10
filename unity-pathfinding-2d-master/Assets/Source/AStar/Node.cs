using UnityEngine;

namespace Assets.Source.AStar
{
    public class Node
    {
        public Node(Vector2 center, int xindex, int yindex, bool isSolid = false) {
            Center = center;
            XIndex = xindex;
            YIndex = yindex;
            IsSolid = isSolid;
        }


        public Vector2 Center { get; }

        public bool IsSolid { get; set; }

        #region AStar Values
        /// <summary>
        /// The cost of moving to the next square.  This is where you'd want to add arbitrary costs 
        /// for various terrain types if you wanted the pathfinder to not prefer certain tiles
        /// </summary>
        public float GCost { get; set; }

        /// <summary>
        /// The distance to the end node
        /// </summary>
        public float HCost { get; set; }

        /// <summary>
        /// Returns GCost + HCost
        /// </summary>
        public float FCost => GCost + HCost;

        /// <summary>
        /// The parent of this node is the node that came before this node in the final path.  
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// Stores this node's x index in the array.  Makes it 
        /// easier to do reverse lookups 
        /// </summary>
        public int XIndex { get; set; }

        /// <summary>
        /// Stores this node's y index in an array.  makes it 
        /// easier to do reverse lookups
        /// </summary>
        public int YIndex { get; set; }
        #endregion

        public override string ToString()
        {
            return $"({XIndex}, {YIndex})";
        }
    }
}
