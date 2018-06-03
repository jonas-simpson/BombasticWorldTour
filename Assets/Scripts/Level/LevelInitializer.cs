using System.Collections;
using UnityEngine;

namespace LevelControl
{
    public class LevelInitializer : MonoBehaviour
    {
        public GameObject gridBasePrefab;

        public int unitCount = 1;
        public GameObject unitPrefabs;

        private WaitForEndOfFrame waitEF;

        public GridStats gridStats;

        [System.Serializable]
        public class GridStats
        {
            public int maxX = 10;
            public int maxY = 3;
            public int maxZ = 10;

            public float offsetX = 1;
            public float offsetY = 1;
            public float offsetZ = 1;
        }

        void Start()
        {
            waitEF = new WaitForEndOfFrame();

            StartCoroutine("InitLevel");
        }
        
        IEnumerator InitLevel()
        {
            yield return StartCoroutine(CreateGrid());
            yield return StartCoroutine(CreateUnits());
            yield return StartCoroutine(EnablePlayerInteractions());
        }

        IEnumerator CreateGrid()
        {
            GameObject go = Instantiate(gridBasePrefab, Vector3.zero, Quaternion.identity)
                            as GameObject;
            go.GetComponent < GridMaster.GridBase >().InitGrid(gridStats);

            yield return waitEF;
        }

        IEnumerator CreateUnits()
        {
            for (int i = 0; i < unitCount; i++)
            {
                Instantiate(unitPrefabs, Vector3.zero, Quaternion.identity);
            }

            yield return waitEF;
        }

        IEnumerator EnablePlayerInteractions()
        {
            GetComponent<PI.PlayerInteractions>().enabled = true;

            yield return waitEF;
        }
    }
}
