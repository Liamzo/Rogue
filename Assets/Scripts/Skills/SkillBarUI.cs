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
    
    void Start()
    {
        playerSkills = FindObjectOfType<PlayerSkills>();

        playerSkills.OnSkillUnlocked += UpdateVisuals;

        UpdateVisuals();
    }

    public void UpdateVisuals () {
        List<BaseSkill> skills = playerSkills.GetUnlockedSkills();

        int i = 0;

        if (skills != null) {
            for (i = 0; i < skills.Count; i++) {
                icons[i].sprite = skills[i].skill.icon;
                icons[i].enabled = true;
            }
        }

        for (int j = i; j < 9; j++) {
            icons[i].sprite = null;
            icons[i].enabled = false;
        }
        
    }
}
