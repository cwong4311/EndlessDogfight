using UnityEngine;

public class HealToken : MonoBehaviour
{
    public LayerMask PlayerLayer;

    public int HealAmount;
    public float Lifetime;

    private float _timeSinceAwake;

    public AudioClip HealSFX;

    [SerializeField]
    private AudioSource _audioSource;

    public void OnEnable()
    {
        _timeSinceAwake = Time.time;
    }

    private void Update()
    {
        if (Time.time - _timeSinceAwake > Lifetime)
        {
            Destroy();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        var target = collision.gameObject;
        if (((1 << target.layer) & PlayerLayer.value) == 0) return;

        if (target.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.Heal(HealAmount);

            PlayCollectionSound();
            Destroy();
        }
    }

    private void Destroy()
    {
        if (ObjectPoolManager.ReturnToPool(gameObject) == false)
        {
            Destroy(gameObject);
        }
    }

    private void PlayCollectionSound()
    {
        if (_audioSource != null)
        {
            AudioSource.PlayClipAtPoint(HealSFX, _audioSource.transform.position);
        }
    }
}
