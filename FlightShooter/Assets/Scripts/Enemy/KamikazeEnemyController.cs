using UnityEngine;

public class KamikazeEnemyController : EnemyController
{
    public GameObject KamikazePF;
    public float KamikazeRange;
    public float KamikazeDamage;
 
    public LayerMask TargetLayers;
    public EnemyHealth EnemyHealth;    

    private float _sqrKamikazeRange;
    private GameObject _originalDeathPF;

    public void Awake()
    {
        _originalDeathPF = EnemyHealth.DeathEffectPF;
        _sqrKamikazeRange = KamikazeRange * KamikazeRange;
    }

    public new void OnEnable()
    {
        EnemyHealth.DeathEffectPF = _originalDeathPF;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (InKamikazeRange())
        {
            TriggerExplosion();
            Die();
        }
    }

    private bool InKamikazeRange()
    {
        var dirToPlayer = transform.position - PlayerTarget.position;
        if (dirToPlayer.sqrMagnitude < _sqrKamikazeRange)
        {
            return true;
        }

        return false;
    }

    private void TriggerExplosion()
    {
        if (KamikazePF != null)
        {
            var kamikazeGO = ObjectPoolManager.Spawn(
                KamikazePF,
                transform.position,
                transform.rotation
            ).GetComponent<DamageOnTriggerEnter>();

            kamikazeGO.Damage = KamikazeDamage;
            kamikazeGO.TargetCollisionLayer = TargetLayers;
        }
    }

    private void Die()
    {
        if (EnemyHealth != null)
        {
            EnemyHealth.DeathEffectPF = null;
            EnemyHealth.Die();
        }
    }
}
