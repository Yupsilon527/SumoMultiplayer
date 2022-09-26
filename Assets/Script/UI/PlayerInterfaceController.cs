using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInterfaceController : MonoBehaviour
{
    PlayerController trackedPlayer;

    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;
    public GameObject playerHealth;
    public GameObject playerCharge;
    private void Awake()
    {
        if (playerName == null)
        playerName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        if (playerScore == null)
            playerScore = transform.Find("Score").GetComponent<TextMeshProUGUI>();
        if (playerHealth == null)
            playerHealth = transform.Find("HealthBar").gameObject;
        if (playerCharge == null)
            playerCharge = transform.Find("Charge Bar").gameObject;
    }

    public void AssignPlayer(PlayerController player)
    {
        trackedPlayer = player;
        UpdateData();
    }
    public void UpdateData()
    {
        if (trackedPlayer == null) return;
        playerName.text = trackedPlayer.NickName.Value;
        playerScore.text = "Score: "+trackedPlayer.Score;
    }
}
