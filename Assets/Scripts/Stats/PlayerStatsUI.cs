using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public Transform playerParent;

    public Slider playerGrit;
    public Slider playerGrace;
    public Text gritNum;
    public Text graceNum;

    public TextMeshProUGUI ammoCount;
    public BaseRangedWeapon rangedWeapon;

    public PlayerController player;
    public PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        playerStats = FindObjectOfType<PlayerStats>();
        playerStats.OnUIChange += UpdateVisuals;

        player.GetComponent<EquipmentManager>().onEquipmentChanged += ChangeRangedWeapon;


        playerGrit = playerParent.Find("GritSlider").GetComponent<Slider>();
        playerGrace = playerParent.Find("GraceSlider").GetComponent<Slider>();

        gritNum = playerGrit.gameObject.transform.Find("GritNum").GetComponent<Text>();
        graceNum = playerGrace.gameObject.transform.Find("GraceNum").GetComponent<Text>();

        playerGrit.maxValue = playerStats.stats[(int)Stats.Grit].GetValue();
        playerGrace.maxValue = playerStats.stats[(int)Stats.Grace].GetValue();

        UpdateVisuals();

        ammoCount = playerParent.Find("AmmoCount").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateVisuals () {
        playerGrit.maxValue = playerStats.stats[(int)Stats.Grit].GetValue();
        playerGrace.maxValue = playerStats.stats[(int)Stats.Grace].GetValue();

        playerGrit.value = playerStats.currentGrit;

        // Situations were Grace can be above max
        playerGrace.value = playerStats.currentGrace;

        gritNum.text = playerGrit.value + " / " + playerGrit.maxValue;
        graceNum.text = playerStats.currentGrace + " / " + playerGrace.maxValue;
    }

    public void UpdateAmmo () {
        ammoCount.text = rangedWeapon.ammo.ToString();
    }

    public void ChangeRangedWeapon(BaseEquipment newItem, BaseEquipment oldItem) {
        if (oldItem is BaseRangedWeapon) {
            ((BaseRangedWeapon)oldItem).OnAmmoChange -= UpdateAmmo;
        }
        
        if (newItem is BaseRangedWeapon) {
            ((BaseRangedWeapon)newItem).OnAmmoChange += UpdateAmmo;
            rangedWeapon = (BaseRangedWeapon)newItem;
            UpdateAmmo();
        } else {
            rangedWeapon = null;
            ammoCount.text = "-";
        }
    }

}
