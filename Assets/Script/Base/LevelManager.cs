using UnityEngine;
using NaughtyAttributes;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [Header("Modules")]
    [SerializeField] private SpawnLevel spawnLevel;

    [Header("Game State")]
    public int currentLevel = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        UIManager.Instance.ShowStartPanel();
    }

    public void OnEnable()
    {
        EventManager.SubscribeTo<LevelStartEvent>(OnLevelStart);
        EventManager.SubscribeTo<NextLevelEvent>(OnNextLevel);
        EventManager.SubscribeTo<RestartLevelEvent>(OnRestartLevel);
        EventManager.SubscribeTo<LevelWinEvent>(OnLevelWin);
        EventManager.SubscribeTo<LevelLoseEvent>(OnLevelLose);
    }

    public void OnDisable()
    {
        EventManager.UnsubscribeFrom<LevelStartEvent>(OnLevelStart);
        EventManager.UnsubscribeFrom<NextLevelEvent>(OnNextLevel);
        EventManager.UnsubscribeFrom<RestartLevelEvent>(OnRestartLevel);
        EventManager.UnsubscribeFrom<LevelWinEvent>(OnLevelWin);
        EventManager.UnsubscribeFrom<LevelLoseEvent>(OnLevelLose);
    }

    public void OnLevelStart(ref LevelStartEvent data)
    {
        currentLevel = 1;         
        UIManager.Instance.HideAllPanels();
        LoadLevel(currentLevel);
    }

    public void OnNextLevel(ref NextLevelEvent data)
    {
        currentLevel++;        
        UIManager.Instance.HideAllPanels();
        LoadLevel(currentLevel);
    }

    public void OnRestartLevel(ref RestartLevelEvent data)
    {
        
        UIManager.Instance.HideAllPanels();
        LoadLevel(currentLevel);
    }

    public void OnLevelWin(ref LevelWinEvent data)
    {
        UIManager.Instance.ShowWinPanel(data.StackCount);
    }
    public void OnLevelLose(ref LevelLoseEvent data)
    {
        UIManager.Instance.ShowLosePanel();
    }

    [Button("Load Current Level")]
    public void LoadLevel()
    {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        spawnLevel.ClearMap();
        spawnLevel.CreateGround();
        spawnLevel.StartLevelSpawn(levelIndex);
            UIManager.Instance.ShowInGamePanel(levelIndex);
    }


    public void NextLevel()
    {
        currentLevel++;
        LoadLevel();
    }

    public void RestartLevel()
    {
        LoadLevel();
    }
}