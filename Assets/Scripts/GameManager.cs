using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
            {
                return gameData.playerPositions[levelIndex];
            }
            return default;
        }
    }

    public static GameManager Instance;

    public Action OnGameRestart;
    public Action OnRestartEnemy;
    public Action OnLevelComplete;

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
            default:
                break;
        }
    }

    public void OnGamerOver()
    {
        UIManager.Instance.SetGameOverPanelState(true);
    }

    public void RestartLevel()
    {
        Invoke("Reset", 2f);
    }

    private void Reset()
    {
        OnGameRestart();

        foreach (List<Collectible> collectibleList in collectibles.Values)
            foreach (Collectible collectible in collectibleList)
                collectible.gameObject.SetActive(true);

        OnRestartEnemy?.Invoke();
        UIManager.Instance.SetGameOverPanelState(false);
    }

    public void OnContinueClicked()
    {
        Invoke("LoadNextLevel", 2f);
    }

    private void LoadNextLevel()
    {
        collectibles.Clear();
        gameData.ClearEnemyData();

        platforms[levelIndex].SetActive(false);
        levelIndex++;
        platforms[levelIndex].SetActive(true);
    }
}
