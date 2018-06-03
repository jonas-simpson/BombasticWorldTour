using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelControl;

namespace GridMaster
{
    public class GridBase : MonoBehaviour
    {
        //Setting up the grid
        public int maxX;
        public int maxY;
        public int maxZ;

        //Offset relates to the world positions only
        public float offsetX;
        public float offsetY;
        public float offsetZ;

        public Node[,,] grid;   //this is our grid!!!

        public GameObject gridFloorPrefab;

        public Vector3 startNodePosition;
        public Vector3 endNodePosition;

        public int enabledY;
        List<GameObject> YCollisions = new List<GameObject>();

        public int agents;

        LevelManager lvlManager;

        public void InitGrid(LevelControl.LevelInitializer.GridStats gridStats)
        {
            maxX = gridStats.maxX;
            maxY = gridStats.maxY;
            maxZ = gridStats.maxZ;

            offsetX = gridStats.offsetX;
            offsetY = gridStats.offsetY;
            offsetZ = gridStats.offsetZ;

            lvlManager = LevelManager.GetInstance();

            CreateGrid();
            CreateMouseCollision();
            CloseAllMouseCollisions();

            YCollisions[enabledY].SetActive(true);
        }

        /*
        void Start()
        {
            //The typical way to create a grid
            grid = new Node[maxX, maxY, maxZ];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        //Apply the offsets and create the world object for each node
                        float posX = x * offsetX;
                        float posY = y * offsetY;
                        float posZ = z * offsetZ;
                        GameObject go = Instantiate(gridFloorPrefab, new Vector3(posX, posY, posZ),
                            Quaternion.identity) as GameObject;
                        //Rename the new gameobject...
                        go.transform.name = x.ToString() + " " + y.ToString() + " " + z.ToString();
                        //...and parent it under this transform to be more organized
                        go.transform.parent = transform;

                        //Create a new node and update its values
                        Node node = new Node();
                        node.x = x;
                        node.y = y;
                        node.z = z;
                        node.worldObject = go;

                        RaycastHit[] hits = Physics.BoxCastAll(new Vector3(posX, posY, posZ),
                                            new Vector3(1, 0, 1), Vector3.forward);
                        for (int i = 0; i < hits.Length; i++)
                        {
                                node.isWalkable = false;
                        }

                        //Then place it into our new grid!
                        grid[x, y, z] = node;
                    }
                }
            }
        }
        */
        //Just a quick and dirty way to visualiza the path
        //public bool start;
        void Update()
        {
            /*if (start)
            {
                start = false;
                //create the new pathfinder class
                //Pathfinding.Pathfinder path = new Pathfinding.Pathfinder();

                //to test the avoidance, just make a node unwalkable
                //grid[1, 0, 1].isWalkable = false;

                //pass the target nodes
                Node startNode = GetNodeFromVector3(startNodePosition);
                Node end = GetNodeFromVector3(endNodePosition);

                //path.startPosition = startNode;
                //path.endPosition = end;

                //find the path
                //List<Node> p = path.FindPath();

                //and simply disable the world object for each node we are going to pass from
                startNode.worldObject.SetActive(false);
                
                foreach(Node n in p)
                {
                    n.worldObject.SetActive(false);
                }
                
                for (int i = 0; i < agents; i++)
                {
                    Pathfinding.PathfindMaster.GetInstance().RequestPathfind(startNode, end, ShowPath);
                }
            }*/
        }

        //obsolete
        public void ShowPath(List<Node> path)
        {
            foreach (Node n in path)
            {
                n.worldObject.SetActive(false);
            }
            //Debug.log("agent complete");
        }

        public Node GetNode(int x, int y, int z)
        {
            /*
            Used to get a node from a grid.
            If it is greater than all the max values we have,
            then it will return null.
            */

            Node retVal = null;

            if(x >= 0 && x < maxX &&
               y >= 0 && y < maxY &&
               z >= 0 && z < maxZ)
            {
                retVal = grid[x, y, z];
            }

            return retVal;
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            float worldX = worldPosition.x;
            float worldY = worldPosition.y;
            float worldZ = worldPosition.z;

            worldX /= offsetX;
            worldY /= offsetY;
            worldZ /= offsetZ;

            int x = Mathf.RoundToInt(worldX);
            int y = Mathf.RoundToInt(worldY);
            int z = Mathf.RoundToInt(worldZ);

            if (x > maxX - 1)
                x = maxX - 1;
            if (y > maxY - 1)
                y = maxY - 1;
            if (z > maxZ - 1)
                z = maxZ - 1;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            if (z < 0)
                z = 0;

            return grid[x, y, z];
        }

        //obsolete
        public Node GetNodeFromVector3(Vector3 pos)
        {
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);
            int z = Mathf.RoundToInt(pos.z);

            Node retVal = GetNode(x, y, z);
            return retVal;
        }

        void CreateGrid()
        {
            //The typical way to create a grid
            grid = new Node[maxX, maxY, maxZ];

            for (int i = 0; i < maxY; i++)
            {
                lvlManager.lvlObjects.Add(new ObjsPerFloor());
                lvlManager.lvlObjects[i].floorIndex = i;
            }

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        //Apply the offsets and create the world object for each node
                        float posX = x * offsetX;
                        float posY = y * offsetY;
                        float posZ = z * offsetZ;
                        GameObject go = Instantiate(gridFloorPrefab, new Vector3(posX, posY, posZ),
                            Quaternion.identity) as GameObject;
                        //Rename the new gameobject...
                        go.transform.name = x.ToString() + " " + y.ToString() + " " + z.ToString();
                        //...and parent it under this transform to be more organized
                        go.transform.parent = transform;

                        //Create a new node and update its values
                        Node node = new Node();
                        node.x = x;
                        node.y = y;
                        node.z = z;
                        node.worldObject = go;
                        node.nodeRef = go.GetComponentInChildren<NodeReferences>();
                        node.isWalkable = false;
                        node.nodeRef.tileRender.enabled = false;

                        RaycastHit[] hits = Physics.BoxCastAll(new Vector3(posX, posY, posZ),
                                            new Vector3(0.5f, 0.5f, 0.5f), Vector3.up);
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if(hits[i].transform.GetComponent<LevelObject>())
                            {
                                LevelObject lvlObj = hits[i].transform.GetComponent<LevelObject>();

                                if(!lvlManager.lvlObjects[y].objs.Contains(lvlObj.gameObject))
                                {
                                    lvlManager.lvlObjects[y].objs.Add(lvlObj.gameObject);
                                }

                                node.nodeRef.tileRender.enabled = true;

                                if(lvlObj.objType == LevelObject.LvlObjectType.obstacle)
                                {
                                    node.isWalkable = false;
                                    node.nodeRef.ChangeTileMaterial(1);
                                    break;
                                }

                                if(lvlObj.objType == LevelObject.LvlObjectType.floor)
                                {
                                    node.isWalkable = true;
                                    node.nodeRef.ChangeTileMaterial(0);
                                }                         
                            }
                        }

                        //Then place it into our new grid!
                        grid[x, y, z] = node;
                    }
                }
            }
        }

        void CreateMouseCollision()
        {
            for (int y = 0; y < maxY; y++)
            {
                GameObject go = new GameObject();
                go.transform.name = "Collision for Y " + y.ToString();
                go.AddComponent<BoxCollider>();
                go.GetComponent<BoxCollider>().size = new Vector3(maxX * offsetX, 0.1f, maxZ * offsetZ);

                go.transform.position = new Vector3((maxX * offsetX) / 2 - offsetX / 2,
                                                        y * offsetY, (maxZ * offsetZ) / 2 - offsetZ / 2);
                YCollisions.Add(go);
            }
        }

        void CloseAllMouseCollisions()
        {
            for (int i = 0; i < YCollisions.Count; i++)
            {
                YCollisions[i].SetActive(false);
            }
        }

        //Singleton
        public static GridBase instance;
        public static GridBase GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }
    }
}
