using UnityEngine;

public class PowerupToken : MonoBehaviour
{
    public LayerMask PlayerLayer;

    public string PowerupName;
    public bool isPrimaryWeapon;

    public float Lifetime;

    private float _timeSinceAwake;

    public AudioClip PowerupSFX;

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

        if (target.TryGetComponent<WeaponsMananger>(out var playerWeaponManager))
        {
            if (isPrimaryWeapon)
            {
                playerWeaponManager.ChangeWeapon(PowerupName);
            }
            else
            {
                playerWeaponManager.ChangeSecondary(PowerupName);
            }

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
            AudioSource.PlayClipAtPoint(PowerupSFX, _audioSource.transform.position);
        }
    }
}
