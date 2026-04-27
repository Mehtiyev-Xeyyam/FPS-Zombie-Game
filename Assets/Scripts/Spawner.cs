using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnGroup
    {
        [Header("Zombies")]
        public GameObject prefab;

        [Header("Spawn Settings")]
        public float spawnInterval = 2f;
        public int maxSpawnCount = 10;

        [Header("Spawn Area")]
        public Vector2 fieldSize = new Vector2(10f, 10f);

        [HideInInspector] public int currentCount = 0;
        [HideInInspector] public float nextSpawnTime = 0f;
    }

    [SerializeField] private SpawnGroup[] spawnGroups = new SpawnGroup[4];

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    private void Start()
    {
        for (int i = 0; i < spawnGroups.Length; i++)
        {
            if (spawnGroups[i] == null)
                spawnGroups[i] = new SpawnGroup();

            spawnGroups[i].nextSpawnTime = Time.time + spawnGroups[i].spawnInterval;
        }
    }

    private void Update()
    {
        for (int i = 0; i < spawnGroups.Length; i++)
        {
            if (Time.time >= spawnGroups[i].nextSpawnTime)
            {
                SpawnObject(i);
                spawnGroups[i].nextSpawnTime = Time.time + spawnGroups[i].spawnInterval;
            }
        }
    }

    private void SpawnObject(int groupIndex)
    {
        SpawnGroup group = spawnGroups[groupIndex];

        if (group.currentCount >= group.maxSpawnCount || group.prefab == null) return;

        Vector3 spawnPos = new Vector3(
            Random.Range(-group.fieldSize.x / 2, group.fieldSize.x / 2),
            player != null ? player.position.y : 0f, // use player height
            Random.Range(-group.fieldSize.y / 2, group.fieldSize.y / 2)
        );

        Instantiate(group.prefab, spawnPos, Quaternion.identity);
        group.currentCount++;
    }
}