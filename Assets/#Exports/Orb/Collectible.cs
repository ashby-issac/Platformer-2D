// This script controls the orb collectables. It is responsible for detecting collision
// with the player and reporting it to the game manager. Additionally, since the orb
// is a part of the level it will need to register itself with the game manager

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
		{ 
			if (PlayerHealth.IsFull)
			{
				return;
			}
			else
			{
				GameManager.Instance.OnHealthPickup?.Invoke();
			}
		}

        gameObject.SetActive(false);
		GameManager.Instance.AddCollectible(CollectibleType, this, 1);
    }
}
