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

    public Rigidbody rb;

    private int actionCount = 2;

    public bool isActive = false;

    private Transform pathTarget;
    private NavMeshPath path;

    private int inRange = 0;

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
        if (gameManager.playerTurn == false)
        {
            Debug.Log("going into move");
            move();
            Debug.Log("going into attackPlayer");
            AttackPlayer();
        }

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

    public void move()
    {
        Vector3 randomDirection = Random.insideUnitSphere * movement1;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, movement1, 1);
        Vector3 finalPosition = hit.position;
        GetComponent<NavMeshAgent>().destination = finalPosition;

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
        Debug.Log("inside attackPlayer");
        GameObject target = gameManager.FindClosestHero(gameObject);
        PlayerController targetHero = target.GetComponent<PlayerController>();
        Debug.Log(targetHero);

        targetHero.hp -= attack;
        targetHero.updateUI();
    }
}
