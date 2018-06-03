using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridMaster;

namespace UnitControl
{
    public class UnitController : MonoBehaviour
    {
        UnitStates states;
        GridBase grid;
        public Vector3 startingPosition;   //hardcoded

        public Node currentNode;

        public bool movePath;

        int indexPath = 0;

        public List<Node> currentPath = new List<Node>();
        public List<Node> shortPath = new List<Node>();

        public float speed = 5;

        float startTime;
        float fractLength;
        Vector3 startPos;
        Vector3 targetPos;
        bool updatedPos;

        private void Start()
        {
            grid = GridBase.GetInstance();
            states = GetComponent<UnitStates>();
            PlaceOnNodeImmediate(startingPosition);

            currentNode = grid.GetNodeFromVector3(startingPosition);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (!movePath)
                    movePath = true;
            }

            states.move = movePath;

            if(movePath)
            {
                if(!updatedPos)
                {
                    if (indexPath < shortPath.Count - 1)
                    {
                        indexPath++;
                    }
                    else
                    {
                        movePath = false;
                    }

                    currentNode = grid.GetNodeFromWorldPosition(transform.position);
                    startPos = currentNode.worldObject.transform.position;
                    targetPos = shortPath[indexPath].worldObject.transform.position;

                    fractLength = Vector3.Distance(startPos, targetPos);
                    startTime = Time.time;
                    updatedPos = true;
                }

                float distCover = (Time.time - startTime) * states.movingSpeed;

                if(fractLength == 0)
                {
                    fractLength = 0.1f;
                }

                float fracJourney = distCover / fractLength;

                if(fracJourney > 1)
                {
                    updatedPos = false;
                }

                Vector3 targetPosition = Vector3.Lerp(startPos, targetPos, fracJourney);

                Vector3 dir = targetPos - startPos;
                dir.y = 0;

                if (!Vector3.Equals(dir, Vector3.zero))
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                            Time.deltaTime * states.maxSpeed);
                }

                transform.position = targetPosition;
            }
        }

        public void EvaluatePath()
        {
            Vector3 curDirection = Vector3.zero;

            for (int i = 1; i < currentPath.Count; i++)
            {
                Vector3 nextDirection = new Vector3(
                    currentPath[i - 1].x - currentPath[i].x,
                    currentPath[i - 1].y - currentPath[i].y,
                    currentPath[i - 1].z - currentPath[i].z);

                if(Vector3.Equals(nextDirection, curDirection))
                {
                    shortPath.Add(currentPath[i - 1]);
                    shortPath.Add(currentPath[i]);
                }

                curDirection = nextDirection;
            }

            shortPath.Add(currentPath[currentPath.Count - 1]);
        }

        public void ResetMovingVariables()
        {
            updatedPos = false;
            indexPath = 0;
            fractLength = 0;
        }

        public void PlaceOnNodeImmediate(Vector3 nodePos)
        {
            int x = Mathf.CeilToInt(nodePos.x);
            int y = Mathf.CeilToInt(nodePos.y);
            int z = Mathf.CeilToInt(nodePos.z);

            Node node = grid.GetNode(x, y, z);

            transform.position = node.worldObject.transform.position;
        }
    }
}
