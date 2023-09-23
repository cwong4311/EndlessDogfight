using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoidsController : MonoBehaviour
{
    [SerializeField]
    private BoidsAgent _boidsAgentPF;
    private List<BoidsAgent> _allBoidsAgents = new List<BoidsAgent>();
    private List<BoidsAgent> _destroyedAgents = new List<BoidsAgent>();

    [SerializeField]
    private Transform[] spawnAreas;

    public Transform PlayerTarget;

    public int WaveSize = 5;

    public Vector3 EnemyAreaOrigin;
    public float EnemyAreaRadius;

    private const float enemyDensity = 3f;
    private int assignedGroup;

    private Coroutine _enemySpawnCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
    }

    public void SpawnEnemies(List<BoidsAgent> enemiesToSpawn, float modifier = 1f)
    {
        if (_enemySpawnCoroutine != null)
        {
            StopCoroutine(_enemySpawnCoroutine);
        }

        _enemySpawnCoroutine = StartCoroutine(SpawnEnemyCoroutine(enemiesToSpawn, modifier));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var enemy in _allBoidsAgents)
        {
            // If enemy is inactive, it means it has been destroyed. Queue for gc and continue
            if (enemy == null || enemy.gameObject.activeSelf == false)
            {
                _destroyedAgents.Add(enemy);
                continue;
            }

            List<Transform> nearbyColliders = new List<Transform>();

            Collider[] contacts = Physics.OverlapSphere(enemy.transform.position, enemy.NeighbourRadius);
            foreach (var c in contacts)
            {
                if (c != enemy.Collider && c.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    nearbyColliders.Add(c.transform);
                }
            }
            // Add player as a potential target. Most Boids behaviours will have filters to ignore this.
            if (PlayerTarget != null) nearbyColliders.Add(PlayerTarget);

            Vector3 movement = enemy.Behaviour.GetMove(enemy, nearbyColliders, this);
            enemy.Move(movement);
        }

        foreach (var destroyed in _destroyedAgents)
        {
            _allBoidsAgents.Remove(destroyed);
        }
    }

    protected IEnumerator SpawnEnemyCoroutine(List<BoidsAgent> enemiesToSpawn, float modifier = 1f)
    {
        yield return new WaitUntil(() => PlayerTarget != null);

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            var randomSpawnPoint = Random.Range(0, spawnAreas.Length);
            assignedGroup = randomSpawnPoint;

            var enemy = ObjectPoolManager.Spawn(
                enemiesToSpawn[i].gameObject,
                spawnAreas[randomSpawnPoint].position + (Vector3.up * 18f),
                Quaternion.Euler(new Vector3(Random.Range(-120, 120), Random.Range(-120, 120), Random.Range(-120, 120)))
            );
            enemy.transform.parent = transform;

            var enemyBoid = enemy.GetComponent<BoidsAgent>();
            if (enemyBoid != null)
            {
                enemyBoid.Init(assignedGroup);
                _allBoidsAgents.Add(enemyBoid);
            }

            var enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.SetTarget(PlayerTarget);
                enemyController.Buff(modifier);
            }

            yield return new WaitForSeconds(Random.Range(0.05f, 0.5f));
        }

        _enemySpawnCoroutine = null;
    }

    private void FindPlayer()
    {
        var player = GameObject.FindFirstObjectByType<PlayerController>();
        if (player != null) PlayerTarget = player.transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(EnemyAreaOrigin, EnemyAreaRadius);
    }
}
