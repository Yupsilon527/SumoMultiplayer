using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceController : MonoBehaviour
{
    PlayerController trackedPlayer;

    public Sprite[] CharacterIcons;

    public Image playerIcon;
    public TextMeshProUGUI playerDamage;
    public GameObject playerHeart;
    public GameObject playerProgress;
    public GameObject playerRage;
    private void Awake()
    {
        if (playerDamage == null)
            playerDamage = transform.Find("Damage").GetComponent<TextMeshProUGUI>();
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


            float progressPercent = trackedPlayer.Score / GameController.main.WinScore;
        if (playerHeart!=null)
        playerHeart.transform.localScale = Vector3.one * progressPercent;
        if (playerProgress != null)
            playerProgress.transform.localScale = new Vector3(progressPercent, 1, 1);

        if (playerRage != null)
        {
            float manaPercent = trackedPlayer.damageable.Rage / trackedPlayer.damageable.RageMax;
            playerRage.transform.localScale = new Vector3(manaPercent, 1, 1);
        }
    }
}
