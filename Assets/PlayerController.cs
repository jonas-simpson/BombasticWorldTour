using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;

    public NavMeshAgent agent;

    public int hp = 0;
    public int defense = 0;
    public int attack = 0;
    public int movement1 = 5;
    public int movement2 = 10;
    public int range = 0;

    public int defenseNorth = 0;
    public int defenseEast = 0;
    public int defenseSouth = 0;
    public int defenseWest = 0;

    public int actionCount = 2;

    public bool isActive = false;

    public Text hpText;
    public Text teamActions;

    private Transform pathTarget;
    private NavMeshPath path;

    public int inRange = 0;

    public GameObject sessionMaster;
    GameTracker gameManager;


    // Use this for initialization
    void Start ()
    {
        path = new NavMeshPath();
        sessionMaster = GameObject.Find("SessionMaster");
        gameManager = sessionMaster.GetComponent<GameTracker>();

        updateUI();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (isActive && actionCount > 0 && gameManager.playerTurn == true)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);    //take current mouse position and make a ray in that direction
            RaycastHit hit;

            /*
            //if mouse is over the playing field
            if (Physics.Raycast(ray, out hit))
            {
                drawLine(hit.point);

                
                //Check for left mouse button
                if (Input.GetMouseButtonDown(0) && gameManager.teamActionCount > 0)
                {
                    if (inRange <= actionCount && inRange <= gameManager.teamActionCount && inRange != 0)
                    {
                        moveUnit(hit.point);
                    }
                }
                
            }
            */
        }
        
	}

    private void OnTriggerEnter(Collider other)
    {
        //Calculate defense
        if (other.gameObject.tag == "Tile")
        {
            Tile currentTile = other.gameObject.GetComponent<Tile>();

            Debug.Log(other.gameObject.name);

            defenseNorth = currentTile.northCover;
            defenseSouth = currentTile.southCover;
            defenseEast = currentTile.eastCover;
            defenseWest = currentTile.westCover;
        }
    }

    //OLD DEFENSE
    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision?");
        if(other.gameObject.tag == "lowCover")
        {
            Debug.Log("wall collision");
            Vector3 targetDir = (other.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(targetDir, Vector3.forward);
            Vector3 cross = Vector3.Cross(targetDir, Vector3.forward);
            Debug.Log(cross.y);
            if (cross.y < 0 || cross.y > 0.99)
                angle = -angle;
            //float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg

            Debug.DrawRay(transform.position, targetDir, Color.white, 2, false);

            Debug.Log("Angle: " + angle);

            if (angle <= 45 && angle >= -45)
            {
                Debug.Log("north");
                defenseNorth++;
            }
            else if (angle >= 45 && angle <= 135)
            {
                Debug.Log("east");
                defenseEast++;
            }
            else if (angle >= 135 && angle <= 180 || angle <= -135 && angle >= -180)
            {
                Debug.Log("south");
                defenseSouth++;
            }
            else if (angle >= -135 && angle <= -45)
            {
                Debug.Log("west");
                defenseWest++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("collision?");
        if (other.gameObject.tag == "lowCover")
        {
            Debug.Log("wall collision");
            Vector3 targetDir = (other.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(targetDir, Vector3.forward);
            Vector3 cross = Vector3.Cross(targetDir, Vector3.forward);
            Debug.Log(cross.y);
            if (cross.y < 0 || cross.y > 0.99)
                angle = -angle;
            //float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg

            Debug.DrawRay(transform.position, targetDir, Color.black, 2, false);

            Debug.Log("Angle: " + angle);

            if (angle <= 45 && angle >= -45)
            {
                Debug.Log("north");
                defenseNorth--;
            }
            else if (angle >= 45 && angle <= 135)
            {
                Debug.Log("east");
                defenseEast--;
            }
            else if (angle >= 135 && angle <= 180 || angle <= -135 && angle >= -180)
            {
                Debug.Log("south");
                defenseSouth--;
            }
            else if (angle >= -135 && angle <= -45)
            {
                Debug.Log("west");
                defenseWest--;
            }
        }
    }
    */

    public void resetHero()
    {
        actionCount = 2;
    }

    public void drawLine(Vector3 hit)
    {
        //DRAW LINE
        NavMesh.CalculatePath(transform.position, hit, NavMesh.AllAreas, path);
        if (path.corners.Length >= 2)
        {
            Vector3 previousCorner = path.corners[0];
            float lengthSoFar = 0.0F;
            int i = 1;
            while (i < path.corners.Length)
            {
                Vector3 currentCorner = path.corners[i];
                lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
                previousCorner = currentCorner;
                i++;
            }

            for (int j = 0; j < path.corners.Length - 1; j++)
            {
                if (lengthSoFar <= movement1 && actionCount == 2)
                {
                    inRange = 1;
                    Debug.DrawLine(path.corners[j], path.corners[j + 1], Color.blue);
                }
                else if (lengthSoFar >= movement1 && lengthSoFar <= movement2 && actionCount == 2)
                {
                    inRange = 2;
                    Debug.DrawLine(path.corners[j], path.corners[j + 1], Color.yellow);
                }
                else if (actionCount == 1 && lengthSoFar <= movement1)
                {
                    inRange = 1;
                    Debug.DrawLine(path.corners[j], path.corners[j + 1], Color.yellow);
                }
                else if (lengthSoFar >= movement2 && actionCount == 2 || lengthSoFar >= movement1 && actionCount == 1)
                {
                    inRange = 0;
                    Debug.DrawLine(path.corners[j], path.corners[j + 1], Color.red);
                }
            }
        }
    }

    public void moveUnit(Vector3 hit)
    {
        //MOVE OUR AGENT
        //defenseNorth = defenseEast = defenseSouth = defenseWest = 0;
        agent.SetDestination(hit);
        actionCount -= inRange;

        gameManager.teamActionCount -= inRange;
        gameManager.SetActionText();
    }

    public void activateUnit()
    {
        isActive = true;
    }

    public void deactivateUnit()
    {
        isActive = false;
    }

    public void fireAtEnemy()
    {
        if (isActive)
        {
            actionCount--;
        }
    }

    public void updateUI()
    {
        hpText.text = "HP: " + hp.ToString();
    }

}
