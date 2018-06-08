using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTracker : MonoBehaviour
{
    public int teamActionCount = 5;
    public bool playerTurn;

    public Text teamActionText;

    //public GameObject enemy01;
    //EnemyController enemy01stats;

    private GameObject[] heroes;
    public int heroCount;

    private GameObject[] enemies;
    public int enemyCount;

    public PlayerController activeHero;

    // Use this for initialization
    void Start ()
    {
        teamActionCount = 5;
        playerTurn = true;

        actionReset();

        //enemy01stats = enemy01.GetComponent<EnemyController>();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        heroes = GameObject.FindGameObjectsWithTag("Hero");
        heroCount = heroes.Length;

        Debug.Log("enemyCount: " + enemyCount);
        Debug.Log("heroCount: " + heroCount);

    }

    // Update is called once per frame
    void Update ()
    {
		if (teamActionCount <= 0)
        {
            playerTurn = false;
            teamActionText.text = "EnemyTurn";
        }

        if(playerTurn == false)
        {
            moveEnemies();
            attackPlayers();
            actionReset();
        }
    }

    public void actionReset()
    {
        teamActionCount = 5;
        teamActionText.text = "Team Actions: " + teamActionCount.ToString();

        for (int i = 0; i < heroCount; i++)
        {
            PlayerController heroResetter = heroes[i].GetComponent<PlayerController>();
            heroResetter.resetHero();
        }

        playerTurn = true;
    }

    public void SetActionText()
    {
        teamActionText.text = "Team Actions: " + teamActionCount.ToString();
    }

    public void moveEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemyMover = enemies[i].GetComponent<EnemyController>();
            enemyMover.move();
        }
    }

    public void attackPlayers()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemyAttacker = enemies[i].GetComponent<EnemyController>();
            enemyAttacker.AttackPlayer();
        }
    }

    public PlayerController CheckActiveHero()
    {
        for (int i = 0; i < heroCount; i++)
        {
            PlayerController heroChecker = heroes[i].GetComponent<PlayerController>();
            if (heroChecker.isActive)
            {
                return heroChecker;
            }
        }
        return activeHero;
    }

    public GameObject FindClosestHero(GameObject currentEnemy)
    {
        float[] heroDistance = new float[heroCount];
        float lowestDistance = 100;
        int lowestIndex = 0;

        for (int i = 1; i < heroCount; i++)
        {
            PlayerController heroChecker = heroes[i].GetComponent<PlayerController>();
            heroDistance[i] = Vector3.Distance(heroes[i].transform.position, currentEnemy.transform.position);

            if (heroDistance[i] < lowestDistance)
            {
                lowestDistance = heroDistance[i];
                lowestIndex = i;
            }
        }

        return heroes[lowestIndex];

        /*
        for (int i = 0; i < heroCount - 1; i++)
        {
            for (int j = i + 1; j < heroCount; j++)
            {
                PlayerController heroChecker = heroes[i].GetComponent<PlayerController>();
                heroDistance[] = Vector3.Distance(heroes[i].transform.position, currentEnemy.transform.position);

                if (heroDistance[i] < heroDistance[j])
                {
                    GameObject aux = heroes[i];
                    heroes[i] = heroes[j];
                    heroes[j] = aux;
                }
            }
        }
        
        return heroes[0];
        */

        /*
        for (int i = 0; i < heroCount; i++)
        {
            PlayerController heroChecker = heroes[i].GetComponent<PlayerController>();
            heroDistance[i] = Vector3.Distance(heroes[i].transform.position, currentEnemy.transform.position);

            if (heroDistance[i] < heroDistance[i-1])
            {

            }
        }
        */
    }
}
