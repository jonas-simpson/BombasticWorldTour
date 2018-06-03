using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridMaster;
using Pathfinding;
using UnitControl;

namespace PI
{
    public class PlayerInteractions : MonoBehaviour
    {
        public UnitController activeUnit;

        public bool hasPath;
        public bool holdPath;

        public PathfindMaster pathfinder;
        public GridBase grid;
        Node prevNode;

        public bool visualizePath;
        public GameObject lineGO;
        LineRenderer line;

        void Start()
        {
            //grid = GridBase.GetInstance();
            //pathfinder = PathfindMaster.GetInstance();
        }

        void Update()
        {
            if (activeUnit)
            {
                if(!hasPath)
                {
                    //Debug.Log(activeUnit.transform.position);
                    //Debug.Log(grid);
                    Node startNode = grid.GetNodeFromWorldPosition(activeUnit.transform.position);
                    Node targetNode = FindNodeFromMousePosition();
                    //Debug.Log(targetNode);

                    if(targetNode != null && startNode != null)
                    {
                        if (prevNode != targetNode && !holdPath)
                        {
                            pathfinder.RequestPathfind(startNode, targetNode, PopulatePathOfActiveUnit);
                            prevNode = targetNode;
                            hasPath = true;
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    holdPath = !holdPath;
                }

                if(activeUnit.shortPath.Count < 1)
                {
                    holdPath = false;
                }
                //Debug.Log(activeUnit.shortPath.Count);
                if(visualizePath)
                {
                    if(line == null)
                    {
                        GameObject go = Instantiate(lineGO, transform.position, Quaternion.identity) as GameObject;
                        line = go.GetComponent<LineRenderer>();
                    }
                    else
                    {
                        //line.SetVertexCount(activeUnit.shortPath.Count); obsolete
                        line.positionCount = activeUnit.shortPath.Count;

                        for (int i = 0; i < activeUnit.shortPath.Count; i++)
                        {
                            line.SetPosition(i, activeUnit.shortPath[i].worldObject.transform.position);
                        }
                    }
                }
            }
        }

        Node FindNodeFromMousePosition()
        {
            Node retVal = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 200))
            {
                retVal = grid.GetNodeFromWorldPosition(hit.point);
            }

            /*
            Debug.Log(retVal.x);
            Debug.Log(retVal.y);
            Debug.Log(retVal.z);
            */

            return retVal;
        }

        void PopulatePathOfActiveUnit(List<Node> nodes)
        {
            activeUnit.currentPath.Clear();
            activeUnit.shortPath.Clear();

            activeUnit.currentPath.Add(
                grid.GetNodeFromWorldPosition(activeUnit.transform.position));

            for (int i = 0; i < nodes.Count; i++)
            {
                activeUnit.currentPath.Add(nodes[i]);
            }

            activeUnit.EvaluatePath();
            activeUnit.ResetMovingVariables();
            hasPath = false;
        }
    }
}
