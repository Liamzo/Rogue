using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour
{
    public Transform skillbarTransform;

    public SkillBarSlot[] slots;

    public PlayerSkills playerSkills;
    public PlayerController playerController;

    public Material skillUnusableMaterial;
    
    void Start()
    {
        playerSkills = FindObjectOfType<PlayerSkills>();
        playerController = FindObjectOfType<PlayerController>();

        playerSkills.OnSkillUnlocked += UpdateVisuals;
        playerController.UpdateUI += UpdateVisuals;

        UpdateVisuals();
    }

    public void UpdateVisuals () {
        List<BaseSkill> skills = playerSkills.GetUnlockedSkills();

        int i = 0;

        if (skills != null) {
            for (i = 0; i < skills.Count; i++) {
                slots[i].icon.sprite = skills[i].skill.icon;
                slots[i].icon.enabled = true;

                if (skills[i].CanBeActivated()) {
                    slots[i].icon.material = null;
                    slots[i].coolDown.SetActive(false);
                } else {
                    slots[i].icon.material = skillUnusableMaterial;

                    if (skills[i].coolDownTimer > 0) {
                        slots[i].coolDown.SetActive(true);
                        slots[i].coolDownText.text = skills[i].coolDownTimer.ToString();
                    }
                }
            }
        }

        for (int j = i; j < 9; j++) {
            slots[i].icon.sprite = null;
            slots[i].icon.enabled = false;
        }
        
    }
}
