using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour
{
    public Transform skillbarTransform;

    GameObject[] slots;
    public Image[] icons;

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
                icons[i].sprite = skills[i].skill.icon;
                icons[i].enabled = true;

                if (skills[i].CanBeActivated()) {
                    icons[i].material = null;
                } else {
                    icons[i].material = skillUnusableMaterial;
                }
            }
        }

        for (int j = i; j < 9; j++) {
            icons[i].sprite = null;
            icons[i].enabled = false;
        }
        
    }
}
