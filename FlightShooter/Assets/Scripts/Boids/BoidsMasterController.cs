using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsMasterController : MonoBehaviour
{
    [SerializeField]
    private BoidsAgent _boidsAgentPF;
    private List<BoidsAgent> _allBoidsAgents = new List<BoidsAgent>();

    public int waveSize = 5;
    private const float enemyDensity = 3f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < waveSize; i++)
        {
            var enemy = ObjectPoolManager.Spawn(_boidsAgentPF.gameObject, Random.insideUnitCircle * waveSize * enemyDensity, Quaternion.Euler(Vector3.forward * Random.Range(0, 360)));
            enemy.name = "Enemy " + i;
            enemy.transform.parent = transform;

            var enemyBoid = enemy.GetComponent<BoidsAgent>();
            if (enemyBoid != null)
            {
                _allBoidsAgents.Add(enemyBoid);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var enemy in _allBoidsAgents)
        {
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
    }
}
