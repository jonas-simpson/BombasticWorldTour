using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public NavMeshSurface surface;

    public int width = 10;
    public int height = 10;

    public GameObject wall;
    public GameObject player;

    private bool playerSpawned = false;

	// Use this for initialization
	void Start ()
    {
        GenerateLevel();

        surface.BuildNavMesh();
	}
	
    //Create a grid-based level
    void GenerateLevel()
    {
        //loop over the grid
        for (int x = 0; x <= width; x +=2)
        {
            for (int y = 0; y <= height; y+=2)
            {
                //Should we place a wall?
                if (Random.value > 0.7f)
                {
                    //Spawn a wall
                    Vector3 pos = new Vector3(x - width / 2f, 1f, y - height / 2f);
                    Instantiate(wall, pos, Quaternion.identity, transform);
                }
                //Should we spawn the player?
                else if (!playerSpawned)
                {
                    //Spawn the player
                    Vector3 pos = new Vector3(x - width / 2f, 1f, y - height / 2f);
                    Instantiate(player, pos, Quaternion.identity);
                    playerSpawned = true;
;               }
            }
        }
    }
}
