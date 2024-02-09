using UnityEngine;

public enum CollectibleType
{
    Orb,
    Coin,
    Heart
}

public class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleType type;

    public CollectibleType CollectibleType => type;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        if (type == CollectibleType.Heart)
            if (PlayerHealth.IsFull)
                return;
            else
                GameController.Instance.OnHealthPickup?.Invoke();
        
        AudioManager.Instance.OrbColletionClip();
        gameObject.SetActive(false);
        GameController.Instance.AddCollectible(CollectibleType, this, 1);
    }
}
