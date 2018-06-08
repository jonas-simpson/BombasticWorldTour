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

    public int actionCount = 2;

    public bool isActive = false;

    public Text hpText;
    public Text teamActions;

    private Transform pathTarget;
    private NavMeshPath path;

    private int inRange = 0;

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
        }
	}

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
