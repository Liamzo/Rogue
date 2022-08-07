using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillBarSlot : MonoBehaviour
{
    public Image icon;

    public GameObject coolDown;
    public TextMeshProUGUI coolDownText;

    public BaseSkill baseSkill;

    public static PlayerSkills playerSkills;

    void Awake () {
        if (playerSkills == null) {
            playerSkills = FindObjectOfType<PlayerSkills>();
        }
    }

    // public void ClickSlot () {
    //     if (baseSkill != null) {
    //         playerSkills.TryUnlockSkill(baseSkill);
    //     }
    // }
}
