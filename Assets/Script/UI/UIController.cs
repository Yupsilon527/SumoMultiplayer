using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController main;

    [Header("Components")]
    public PlayerInterfaceController[] UIList;

    public TMPro.TextMeshProUGUI GameTimer;
    public GameObject InGameWindow;
    public GameObject PostGameWindow;

    [Header("Winner Menu")]
    public TMPro.TextMeshProUGUI WinnerName;
    public Button RestartGameButton;

    private void Awake()
    {
        main = this;
        if (UIList == null || UIList.Length == 0)
        UIList = GetComponentsInChildren<PlayerInterfaceController>();
        foreach (PlayerInterfaceController plUI in UIList)
        {
            plUI.gameObject.SetActive(false);
        }
    }
    public void UpdateTimer()
    {
        if (GameTimer!=null && GameTimer.isActiveAndEnabled )
        {
            GameTimer.text = Mathf.Ceil(GameController.main.gameTimer.RemainingTime(GameController.main.Runner) ?? 0)+"s";
        }
    }
    private Dictionary<PlayerRef, PlayerInterfaceController> interfaceData = new Dictionary<PlayerRef, PlayerInterfaceController>();

    public void AddPlayer(PlayerRef playerRef, PlayerController controller)
    {
        if (interfaceData.ContainsKey(playerRef)) return;
        if (controller == null) return;

        PlayerInterfaceController table = null;
        foreach (PlayerInterfaceController plUI in UIList)
        {
            if (!plUI.gameObject.activeSelf)
            {
                plUI.gameObject.SetActive(true);
                table = plUI;
                break;
            }
        }
        interfaceData.Add(playerRef, table);
        table.AssignPlayer(controller);
        UpdatePlayerUI(playerRef);
    }
    public void RemovePlayer(PlayerRef playerRef)
    {
        if (interfaceData.TryGetValue(playerRef, out PlayerInterfaceController entry) == false) return;

        if (entry != null)
        {
            entry.gameObject.SetActive(false);
        }

        interfaceData.Remove(playerRef);
    }

    public void UpdatePlayerUI(PlayerRef player)
    {
        if (interfaceData.TryGetValue(player, out PlayerInterfaceController table) == false) return;
        table.UpdateData();
    }

    /* // Removes an existing Overview Entry

     public void UpdateLives(PlayerRef player, int lives)
     {
         if (_playerListEntries.TryGetValue(player, out var entry) == false) return;

         _playerLives[player] = lives;
         UpdateEntry(player, entry);
     }

     public void UpdateScore(PlayerRef player, int score)
     {
         if (_playerListEntries.TryGetValue(player, out var entry) == false) return;

         _playerScores[player] = score;
         UpdateEntry(player, entry);
     }

     public void UpdateNickName(PlayerRef player, string nickName)
     {
         if (_playerListEntries.TryGetValue(player, out var entry) == false) return;

         _playerNickNames[player] = nickName;
         UpdateEntry(player, entry);
     }

     private void UpdateEntry(PlayerRef player, TextMeshProUGUI entry)
     {
         var nickName = _playerNickNames[player];
         var score = _playerScores[player];
         var lives = _playerLives[player];

         entry.text = $"{nickName}\nScore: {score}\nLives: {lives}";
     }*/

    public void ShowInGameScreen()
    {
        if (InGameWindow != null)
            InGameWindow.SetActive(true);
        if (PostGameWindow != null)
            PostGameWindow.SetActive(false);
    }
    public void ShowEndOfGameScreen()
    {
        if (InGameWindow != null)
            InGameWindow.SetActive(false);
        if (PostGameWindow != null)
            PostGameWindow.SetActive(true);

        if (RestartGameButton!=null)
        {
            RestartGameButton.interactable = GameController.main.Object.HasStateAuthority;
        }
        if (WinnerName != null)
        {
            WinnerName.text = GameController.main.WinningPlayer.NickName.Value;
        }
    }
}
