using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;

    public int hp = 0;
    public int defense = 0;
    public int attack = 3;
    public int movement1 = 5;
    public int movement2 = 10;
    public int range = 0;

    public int defenseNorth = 0;
    public int defenseEast = 0;
    public int defenseSouth = 0;
    public int defenseWest = 0;

    public Rigidbody rb;

    private int actionCount = 2;

    public bool isActive = false;

    private Transform pathTarget;
    private NavMeshPath path;

    public int inRange = 0;

    public bool isMoving = false;

    public GameObject sessionMaster;
    GameTracker gameManager;

    Material myMat;

    public Canvas myCanvas;
    public Text healthText;

    // Use this for initialization
    void Start()
    {
        path = new NavMeshPath();
        sessionMaster = GameObject.Find("SessionMaster");
        gameManager = sessionMaster.GetComponent<GameTracker>();

        rb = GetComponent<Rigidbody>();

        myMat = GetComponent<Renderer>().material;

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 0)
        {
            isMoving = true;
        }

        if (gameManager.playerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        PlayerController currentHero = gameManager.CheckActiveHero();
                        currentHero.actionCount--;
                        gameManager.teamActionCount--;
                        gameManager.SetActionText();
                        hp -= currentHero.attack;
                        UpdateUI();

                        if (hp <= 0)
                        {
                            Destroy(healthText);
                            Destroy(myCanvas);
                            Destroy(gameObject);
                            gameManager.enemyCount--;
                        }
                    }
                }
            }
        }
    }
    //OLD DEFENSE
    /*
    private void OnTriggerEnter(Collider other)
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

    public void move()
    {
        Vector3 targetTile;
        Vector3 randomDirection = Random.insideUnitSphere * movement1;
        Debug.Log(randomDirection);
        randomDirection += transform.position;
        Debug.Log(randomDirection);
        targetTile = gameManager.PickTile(randomDirection);
        NavMeshHit hit;
        NavMesh.SamplePosition(targetTile, out hit, movement1, 1);
        Vector3 finalPosition = hit.position;
        GetComponent<NavMeshAgent>().destination = finalPosition;

        //yield return new WaitForSeconds(5);
        //actionCount--;
    }

    void OnMouseOver()
    {
        // Change the Color of the GameObject when the mouse hovers over it
        if (gameManager.playerTurn)
        {
            myMat.color = Color.yellow;
        }
    }

    void OnMouseExit()
    {
        //Change the Color back to white when the mouse exits the GameObject
            myMat.color = Color.white;
    }

    void UpdateUI()
    {
        healthText.text = "HP: " + hp;
    }

    public void AttackPlayer()
    {
        //Debug.Log("inside attackPlayer");
        GameObject target = gameManager.FindClosestHero(gameObject);
        PlayerController targetHero = target.GetComponent<PlayerController>();
        Debug.Log(targetHero);

        int playerDamage = attack - gameManager.calculateDefense(target, gameObject);

        Debug.Log("defense: " + gameManager.calculateDefense(target, gameObject));

        if (playerDamage < 0)
            playerDamage = 0;

        Debug.Log("playerdamage: " + playerDamage);

        targetHero.hp -= playerDamage;
        targetHero.updateUI();
    }
}
