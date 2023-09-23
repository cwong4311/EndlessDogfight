using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsMasterController : MonoBehaviour
{
    [SerializeField]
    private BoidsAgent _boidsAgentPF;
    private List<BoidsAgent> _allBoidsAgents = new List<BoidsAgent>();
    private List<BoidsAgent> _destroyedAgents = new List<BoidsAgent>();

    public int WaveSize = 5;

    public Vector3 EnemyAreaOrigin;
    public float EnemyAreaRadius;

    private const float enemyDensity = 3f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < WaveSize; i++)
        {
            var enemy = ObjectPoolManager.Spawn(_boidsAgentPF.gameObject, transform.position + (Vector3)(Random.insideUnitCircle * WaveSize * enemyDensity) + Vector3.up * 100f, Quaternion.Euler(Vector3.forward * Random.Range(0, 360)));
            enemy.transform.parent = transform;

            var enemyBoid = enemy.GetComponent<BoidsAgent>();
            if (enemyBoid != null)
            {
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

            List<Transform> nearbyEnemies = new List<Transform>();

            Collider[] contacts = Physics.OverlapSphere(enemy.transform.position, enemy.NeighbourRadius);
            foreach (var c in contacts)
            {
                if (c != enemy.Collider && c.name.Contains("Enemy"))
                {
                    nearbyEnemies.Add(c.transform);
                }
            }

            Vector3 movement = enemy.Behaviour.GetMove(enemy, nearbyEnemies, this);
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
