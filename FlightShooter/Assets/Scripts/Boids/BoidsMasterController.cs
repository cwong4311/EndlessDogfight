using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsMasterController : MonoBehaviour
{
    [SerializeField]
    private BoidsAgent _boidsAgentPF;
    private List<BoidsAgent> _allBoidsAgents = new List<BoidsAgent>();
    private List<BoidsAgent> _destroyedAgents = new List<BoidsAgent>();

    public Transform PlayerTarget;

    public int WaveSize = 5;

    public Vector3 EnemyAreaOrigin;
    public float EnemyAreaRadius;

    private const float enemyDensity = 3f;
    private int assignedGroup;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindFirstObjectByType<PlayerController>();
        if (player != null) PlayerTarget = player.transform;

        for (int i = 0; i < WaveSize; i++)
        {
            var enemy = ObjectPoolManager.Spawn(_boidsAgentPF.gameObject, transform.position + (Vector3)(Random.insideUnitCircle * WaveSize * enemyDensity) + Vector3.up * 300f, Quaternion.Euler(Vector3.forward * Random.Range(0, 360)));
            enemy.transform.parent = transform;

            var enemyBoid = enemy.GetComponent<BoidsAgent>();
            if (enemyBoid != null)
            {
                enemyBoid.Init(assignedGroup);
                _allBoidsAgents.Add(enemyBoid);
            }
        }
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
                if (c != enemy.Collider)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(EnemyAreaOrigin, EnemyAreaRadius);
    }
}
