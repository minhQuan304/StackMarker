using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject inGamePanel;

    [Header("Win Panel Elements")]
    [SerializeField] private TextMeshProUGUI stackResultText;

    [Header("In-Game Display")]
    [SerializeField] private TextMeshProUGUI levelText;
 


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void HideAllPanels()
    {
               startPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
            inGamePanel.SetActive(false);
    }

    public void ShowStartPanel()
    {
        HideAllPanels();
        startPanel.SetActive(true);
    }
    public void ShowWinPanel(int stackCount)
    {
        HideAllPanels();
        stackResultText.text = $"You collected {stackCount} stacks!";
        winPanel.SetActive(true);
    }
    public void ShowLosePanel()
    {
        HideAllPanels();
        losePanel.SetActive(true);
    }

    public void ShowInGamePanel(int level)
    {
        HideAllPanels();
        levelText.text = $"Level {level}";
        inGamePanel.SetActive(true);
    }

    public void OnClickPlayButton()
    {
        EventManager.Raise(new LevelStartEvent());
    }

    public void OnClickNextLevelButton()
    {
        EventManager.Raise(new NextLevelEvent());
    }

    public void OnClickRestartButton()
    {
        EventManager.Raise(new RestartLevelEvent());
    }


}
