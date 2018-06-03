using System.Collections;
using UnityEngine;

namespace GridMaster
{
    public class Node
    {
        //Node's position in the grid
        public int x;
        public int y;
        public int z;

        //Node's cost for pathfinding purposes
        public float hcost;
        public float gcost;

        public float fcost
        {
            get //the fcost is the gcost + hcost, so we can get it directly this way
            {
                return gcost + hcost;
            }
        }

        public Node parentNode;
        public bool isWalkable = true;

        //Reference to the world object so we can have the world position of the node
        public GameObject worldObject;
        public NodeReferences nodeRef;

        //Types of node we can have, we will use this later on in a case by case example
        public NodeType nodeType;
        public enum NodeType
        {
            ground,
            air
        }

    }

}