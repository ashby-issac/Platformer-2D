using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	[SerializeField] private List<Transform> healthContainers = new List<Transform>();
	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private GameObject levelEndPanel;
    [SerializeField] private TextMeshProUGUI orbCountText;
    [SerializeField] private TextMeshProUGUI coinCountText;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button continueButton;

    private int healthIndex;
	public Action OnPlayerRespawned;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		healthIndex = healthContainers.Count;
    }

    private void OnEnable()
    {
        gameOverPanel.SetActive(false);
		levelEndPanel.SetActive(false);
	}

    private void Start()
    {
        restartButton.onClick.AddListener(() => GameController.Instance.RestartLevel());
        continueButton.onClick.AddListener(() => GameController.Instance.OnContinueClicked());
		GameController.Instance.OnLevelComplete += () => levelEndPanel.SetActive(false);

		GameController.Instance.OnRestartResetUI += ResetHealthUI;
		GameController.Instance.OnRestartResetUI += ResetCollectiblesText;
    }

    public void ReduceHealthUI(float currentHealth, float maxHealth) => healthContainers[--healthIndex]?.GetChild(0).gameObject.SetActive(false);

    public void AddHealthUI()
	{
		foreach (Transform healthContainer in healthContainers)
		{
			if (healthContainer.GetChild(0).gameObject.activeInHierarchy)
				continue;

			healthContainer.GetChild(0).gameObject.SetActive(true);
			healthIndex++;
			break;
		}
    }

    void ResetHealthUI() => healthContainers.ForEach(healthContainer => healthContainer.GetChild(0).gameObject.SetActive(true));

    public void SetGameOverPanelState(bool state) => gameOverPanel.SetActive(state);

    public void DisplayOrbUI(int orbCount) => orbCountText.text = orbCount.ToString();

    public void DisplayCoinUI(int coinCount) => coinCountText.text = coinCount.ToString();

    void ResetCollectiblesText()
	{
		DisplayCoinUI(0);
		DisplayOrbUI(0);
    }

	public void SetHealthIndex(float health)
	{
		healthIndex = (int)health;
	}

    public void ShowLevelEndUI() => levelEndPanel.SetActive(true);
}
