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

    public Camera cam;
    //public GameObject cameraTarget;
    public float smoothing = 5f;        // The speed with which the camera will be following.
    public Vector3 offset;

    public GameObject activeCharacter;

    private bool movingEnemies = false;

    private bool funny = false;

    private GameObject[] tiles;
    public int tileCount;

    // Use this for initialization
    void Start ()
    {
        teamActionCount = 5;
        playerTurn = true;

        offset = cam.transform.position;

        StartCoroutine(actionReset());

        //enemy01stats = enemy01.GetComponent<EnemyController>();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        heroes = GameObject.FindGameObjectsWithTag("Hero");
        heroCount = heroes.Length;

        tiles = GameObject.FindGameObjectsWithTag("Tile");
        tileCount = tiles.Length;

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
            funny = false;
            teamActionCount++;
        }

        if (playerTurn == false && funny == false)
        {
            funny = true;
            Debug.Log("haha so funny!");
            StartCoroutine(activateEnemies());
            StartCoroutine(actionReset());
        }
    }

    public void ForceEndTurn()
    {
        teamActionCount = 0;
    }

    void FixedUpdate()
    {
        activeCharacter = FindActiveCharacter();

        cam.transform.position = activeCharacter.transform.position + offset;
    }

    public IEnumerator actionReset()
    {
            while (movingEnemies)
                yield return new WaitForSeconds(0.1f);

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

    /*
    public IEnumerator moveEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemyMover = enemies[i].GetComponent<EnemyController>();
            enemyMover.move();
            Debug.Log("moved enemy");
            yield return new WaitForSeconds(2f);
        }
    }
    */

    public IEnumerator activateEnemies()
    {
        movingEnemies = true;

        Debug.Log("Activating enemies");
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemyMover = enemies[i].GetComponent<EnemyController>();
            enemyMover.isActive = true;
            enemyMover.move();
            Debug.Log("moved enemy");
            yield return new WaitForSeconds(1f);
            enemyMover.AttackPlayer();
            yield return new WaitForSeconds(1f);
            enemyMover.isActive = false;
        }

        movingEnemies = false;
    }

    /*
    public void attackPlayers()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemyAttacker = enemies[i].GetComponent<EnemyController>();
            enemyAttacker.AttackPlayer();
        }
    }
    */

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

        for (int i = 0; i < heroCount; i++)
        {
            //PlayerController heroChecker = heroes[i].GetComponent<PlayerController>();
            heroDistance[i] = Vector3.Distance(heroes[i].transform.position, currentEnemy.transform.position);

            if (heroDistance[i] < lowestDistance)
            {
                lowestDistance = heroDistance[i];
                lowestIndex = i;
            }
        }
        //Debug.Log(heroes[lowestIndex]);
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

    public GameObject FindActiveCharacter()
    {
        if (playerTurn == true)
        {
            for (int i = 0; i < heroCount; i++)
            {
                PlayerController heroChecker = heroes[i].GetComponent<PlayerController>();
                if (heroChecker.isActive)
                {
                    return heroes[i];
                }
            }
            return heroes[0];
        }

        else
        {
            for (int i = 0; i < enemyCount; i++)
            {
                EnemyController enemyChecker = enemies[i].GetComponent<EnemyController>();
                if (enemyChecker.isActive)
                {
                    return enemies[i];
                }
            }
            return enemies[0];
        }
    }

    public int calculateDefense(GameObject heroObject, GameObject enemyObject)
    {
        int finalDefense = 0;
        PlayerController hero = heroObject.GetComponent<PlayerController>();
        EnemyController enemy = enemyObject.GetComponent<EnemyController>();

        //enemy attacking hero
        if (playerTurn == false)
        {
            Vector3 targetDir = (heroObject.transform.position - enemyObject.transform.position).normalized;
            float angle = Vector3.Angle(targetDir, Vector3.forward);
            Vector3 cross = Vector3.Cross(targetDir, Vector3.forward);
            Debug.Log(cross.y);
            if (cross.y < 0 || cross.y > 0.99)
                angle = -angle;
            //float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg

            Debug.DrawRay(enemy.transform.position, targetDir, Color.red, 2, false);

            Debug.Log("Angle: " + angle);

            if (angle <= 90 && angle >= -90)
            {
                finalDefense += hero.defenseNorth;
            }
            if (angle >= 0 && angle <= 180)
            {
                finalDefense += hero.defenseEast;
            }
            if (angle >= 90 && angle <= 180 || angle <= -90 && angle >= -180)
            {
                finalDefense += hero.defenseSouth;
            }
            if (angle >= -180 && angle <= 0)
            {
                finalDefense += hero.defenseWest;
            }
        }

        //hero attacking enemy
        else
        {
            Vector3 targetDir = (enemyObject.transform.position - heroObject.transform.position).normalized;
            float angle = Vector3.Angle(targetDir, Vector3.forward);
            Vector3 cross = Vector3.Cross(targetDir, Vector3.forward);
            Debug.Log(cross.y);
            if (cross.y < 0 || cross.y > 0.99)
                angle = -angle;
            //float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg

            Debug.DrawRay(hero.transform.position, targetDir, Color.red, 2, false);

            Debug.Log("Angle: " + angle);

            if (angle <= 45 && angle >= -45)
            {
                finalDefense += enemy.defenseNorth;
            }
            else if (angle >= 45 && angle <= 135)
            {
                finalDefense += enemy.defenseEast;
            }
            else if (angle >= 135 && angle <= 180 || angle <= -135 && angle >= -180)
            {
                finalDefense += enemy.defenseSouth;
            }
            else if (angle >= -135 && angle <= -45)
            {
                finalDefense += enemy.defenseWest;
            }
        }

        return finalDefense;
    }

    public void MovePlayer(Vector3 target)
    {
        PlayerController activeHero = CheckActiveHero();
        if (activeHero.inRange <= activeHero.actionCount && activeHero.inRange <= teamActionCount && activeHero.inRange != 0)
        {

            activeHero.moveUnit(target);
        }
    }

    public void DrawLine(Vector3 target)
    {
        PlayerController activeHero = CheckActiveHero();
        activeHero.drawLine(target);
    }

    public Vector3 PickTile(Vector3 target)
    {
        Debug.Log("inside PickTile");
        float lowestDistance = 1000;
        float[] tileDistance = new float[tileCount];
        int lowestIndex = 0;
        for (int i = 0; i < tileCount; i++)
        {
            tileDistance[i] = Vector3.Distance(tiles[i].transform.position, target);

            if (tileDistance[i] < lowestDistance)
            {
                lowestDistance = tileDistance[i];
                lowestIndex = i;
            }
        }
        Debug.Log(tiles[lowestIndex]);
        return tiles[lowestIndex].transform.position;
    }

    public int CheckTileContents(Vector3 currentTile)
    {
        for (int i = 0; i < heroCount; i++)
        {
            if (Vector3.Distance(heroes[i].transform.position, currentTile) < 0.5)
            {
                return 1;
            }
        }
        for (int i = 0; i < enemyCount; i++)
        {
            if (Vector3.Distance(enemies[i].transform.position, currentTile) < 0.5)
            {
                return 2;
            }
        }
        return 0;
    }
    
}

