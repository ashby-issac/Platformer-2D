using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Dictionary<CollectibleType, List<Collectible>> collectibles = new Dictionary<CollectibleType, List<Collectible>>();

    [SerializeField] GameObject player;
    [SerializeField] GameData gameData;
    [SerializeField] GameObject[] platforms;

    public GameData GameData => gameData;

    private int levelIndex = 0;
    public Vector3 CurrentLevelPlayerPos
    {
        get
        {
            if (levelIndex < gameData.playerPositions.Count)
                return gameData.playerPositions[levelIndex];
            return default;
        }
    }

    public static GameController Instance;

    public Action OnGameRestart;
    public Action OnRestartEnemy;
    public Action OnLevelComplete;
    public Action OnRestartResetUI;
    public Action OnHealthPickup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < platforms.Length; i++)
            platforms[i].SetActive(false);

        platforms[0].SetActive(true);
    }

    public void AddCollectible(CollectibleType collectibleType, Collectible collectible, int count)
    {
        if (!collectibles.ContainsKey(collectibleType))
            collectibles.Add(collectibleType, new List<Collectible>() { collectible });
        else
            collectibles[collectibleType].Add(collectible);

        BroadcastToUI(collectibleType, collectibles[collectibleType].Count);
    }

    private void BroadcastToUI(CollectibleType collectibleType, int count)
    {
        switch (collectibleType)
        {
            case CollectibleType.Orb:
                UIManager.Instance.DisplayOrbUI(count);
                break;
            case CollectibleType.Coin:
                UIManager.Instance.DisplayCoinUI(count);
                break;
            case CollectibleType.Heart:
                UIManager.Instance.AddHealthUI();
                break;
            default:
                break;
        }
    }

    public void OnGamerOver() => UIManager.Instance.SetGameOverPanelState(true);

    public void RestartLevel() => Invoke("Reset", 3f);

    // Called through Invoke
    private void Reset()
    {
        OnGameRestart?.Invoke();
        OnRestartResetUI?.Invoke();

        foreach (List<Collectible> collectibleList in collectibles.Values)
            foreach (Collectible collectible in collectibleList)
                collectible.gameObject.SetActive(true);

        collectibles.Clear();
        UIManager.Instance.SetGameOverPanelState(false);
        OnRestartEnemy?.Invoke();
    }

    public void OnContinueClicked() => Invoke("LoadNextLevel", 2f);
    
    // Called through Invoke
    private void LoadNextLevel()
    {
        collectibles.Clear();
        gameData.ClearEnemyData();

        OnRestartResetUI?.Invoke();

        platforms[levelIndex].SetActive(false);
        levelIndex++;

        if (levelIndex == platforms.Length)
            return;
        
        platforms[levelIndex].SetActive(true);
        OnLevelComplete?.Invoke();
    }
}
