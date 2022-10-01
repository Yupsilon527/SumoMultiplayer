using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInterfaceController : MonoBehaviour
{
    PlayerController trackedPlayer;

    public TextMeshProUGUI playerDamage;
    public TextMeshProUGUI playerScore;
    public GameObject playerRage;
    private void Awake()
    {
        if (playerDamage == null)
            playerDamage = transform.Find("Damage").GetComponent<TextMeshProUGUI>();
        if (playerScore == null)
            playerScore = transform.Find("Score").GetComponent<TextMeshProUGUI>();
    }

    public void AssignPlayer(PlayerController player)
    {
        trackedPlayer = player;
        UpdateData();
    }
    public void UpdateData()
    {
        if (trackedPlayer == null) return;
        playerDamage.text = Mathf.Ceil(trackedPlayer.damageable.Damage) + "%";
        playerScore.text = Mathf.CeilToInt(trackedPlayer.Score / GameController.main.WinScore * 100f) + "%";

        if (playerRage != null)
        {
            float manaPercent = trackedPlayer.damageable.Rage / trackedPlayer.damageable.RageMax;
            playerRage.transform.localScale = new Vector3(manaPercent, 1, 1);
        }
    }
}
