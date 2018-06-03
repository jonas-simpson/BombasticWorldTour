using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridMaster;

namespace Pathfinding
{
    public class Pathfinder
    {
        GridBase gridBase;
        public Node startPosition;
        public Node endPosition;

        public volatile bool jobDone = false;
        PathfindMaster.PathfindingJobComplete completeCallback;
        List<Node> foundPath;

        public Pathfinder(Node start, Node target, PathfindMaster.PathfindingJobComplete callback)
        {
            startPosition = start;
            endPosition = target;
            completeCallback = callback;
            gridBase = GridBase.GetInstance();
        }

        public void FindPath()
        {
            foundPath = FindPathActual(startPosition, endPosition);

            jobDone = true;
        }

        //This is basically the function we will call
        /*
        public List<Node> FindPath()
        {
            /*
             * We need to make sure we have the reference to gridBase
             * and since this isn't a Monobehavior class, we can't use
             * any of Unity's main functions.
             *

            gridBase = GridBase.GetInstance();

            return FindPathActual(startPosition, endPosition);
        }
        */

        public void NotifyComplete()
        {
            if(completeCallback != null)
            {
                completeCallback(foundPath);
            }
        }

        private List<Node> FindPathActual(Node start, Node target)
        {
            //Typical A* algorythm from here on

            List<Node> foundPath = new List<Node>();

            //We need two lists, one for the nodes we need to check
            //and one for the nodes we've already checked
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            //We start adding the open set
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    //We check the costs for the current node
                    //You can have more options here but that's not important now
                    if (openSet[i].fcost < currentNode.fcost ||
                        (openSet[i].fcost == currentNode.fcost &&
                        openSet[i].hcost < currentNode.hcost))
                    {
                        //and then we assign a new current node
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                //we remove the cuurrent node from the open set and add to the cloase set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                //if the current node is the target node
                if(currentNode.Equals(target))
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, currentNode);
                    break;
                }

                //if we haven't reached our target, then we need to start checking the neighbors
                foreach (Node neighbor in GetNeighbors(currentNode, true))
                {
                    if (!closedSet.Contains(neighbor))
                    {
                        //we create a new movement cost for our neighbors
                        float newMovementCostToNeighbor = currentNode.gcost + GetDistance(currentNode, neighbor);

                        //and if it's lower than the neighbor's cost
                        if (newMovementCostToNeighbor < neighbor.gcost || !openSet.Contains(neighbor))
                        {
                            //we calculate the new costs
                            neighbor.gcost = newMovementCostToNeighbor;
                            neighbor.hcost = GetDistance(neighbor, target);
                            //assign the parent node
                            neighbor.parentNode = currentNode;
                            //and dd the neighbor node to the open set
                            if(!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                        }
                    }
                }
            }

            //return the path at the end
            return foundPath;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            //retrace the path, is basically going from the endNode to the startNode
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                //by taking the parentNodes we assigned
                currentNode = currentNode.parentNode;
            }

            //then we reverse the list
            path.Reverse();

            return path;
        }

        private List<Node> GetNeighbors(Node node, bool getVerticalneighbors = false)
        {
            //this is where we start taking our neighbors
            List<Node> retList = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int yIndex = -1; yIndex <= 1; yIndex++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        int y = yIndex;

                        //if we don't want a 3D A*, then we don't search the y
                        if(!getVerticalneighbors)
                        {
                            y = 0;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                            //000 is the current node
                        }
                        else
                        {
                            Node searchPos = new Node();

                            //the nodes we want are what's forward/backward,
                            //left /right, up/down from us
                            searchPos.x = node.x + x;
                            searchPos.y = node.y + y;
                            searchPos.z = node.z + z;

                            Node newNode = GetNeighborNode(searchPos, true, node);

                            if(newNode != null)
                            {
                                retList.Add(newNode);
                            }
                        }
                    }
                }
            }
            
            return retList;

        }

        private Node GetNeighborNode(Node adjPos, bool searchTopDown, Node currentNodePos)
        {
            /*
             * This is where the meat of the code is
             * We can add all the checks we need here to tweak the algorythm to our heart's content
             * but fist let's start from the usual stuff you'll see in A*
             */

            Node retVal = null;

            //lets take the node from the adacent positions we passed
            Node node = GetNode(adjPos.x, adjPos.y, adjPos.z);

            //if the node is not null and we can walk on it
            if (node != null && node.isWalkable)
            {
                //we can use the node!
                retVal = node;
            }
            //if not...
            else if (searchTopDown) //...and we have a 3D A*...
            {
                //...then look what the adjacent node has under it
                adjPos.y -= 1;
                Node bottomBlock = GetNode(adjPos.x, adjPos.y, adjPos.z);

                //if there is a bottom block and we can walk on it,
                if(bottomBlock != null && bottomBlock.isWalkable)
                {
                    retVal = bottomBlock;   //we can return that!
                }
                //if not, we look above
                else
                {
                    adjPos.y += 2;
                    Node topBlock = GetNode(adjPos.x, adjPos.y, adjPos.z);
                    if (topBlock != null && topBlock.isWalkable)
                    {
                        retVal = topBlock;
                    }
                }
            }

            //if the node is diagonal to the current node then check the neighbors
            //so to move diagonally, we need 4 walkable nodes
            int originalX = adjPos.x - currentNodePos.x;
            int originalZ = adjPos.z - currentNodePos.z;

            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
            {
                //the first node is (originalX, 0) and the second block is (0, originalZ)
                //they need to be pathfinding walkable
                Node neighbor1 = gridBase.GetNode(currentNodePos.x + originalX,
                    currentNodePos.y, currentNodePos.z + originalZ);
                if (neighbor1 == null || !neighbor1.isWalkable)
                {
                    retVal = null;
                }

                Node neighbor2 = gridBase.GetNode(currentNodePos.x,
                    currentNodePos.y, currentNodePos.z + originalZ);
                if (neighbor2 == null || !neighbor2.isWalkable)
                {
                    retVal = null;
                }
            }

            //and here's where we can add additional checks
            if (retVal != null)
            {
                //example, do not approach a node from the left
                /* if (node.x > currentNodePos.x)
                 * {
                 *      node = null;
                 * }
                 */
            }

            return retVal;
        }

        private Node GetNode(int x, int y, int z)
        {
            Node n = null;

            lock (gridBase)
            {
                n = gridBase.GetNode(x, y, z);
            }
            return n;
        }

        private int GetDistance(Node posA, Node posB)
        {
            //We find the distance between each node
            int distX = Mathf.Abs(posA.x - posB.x);
            int distY = Mathf.Abs(posA.y - posB.y);
            int distZ = Mathf.Abs(posA.z - posB.z);

            if (distX > distZ)
            {
                return 14*distZ + 10*(distX - distZ) + 10*distY;
            }

            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }
}
