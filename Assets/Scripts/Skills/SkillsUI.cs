using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsUI : MonoBehaviour
{
    public Transform skillsParent;
    public GameObject skillUI;
    public PlayerSkills playerSkills;

    public Material skillLockedMaterial;
    public Material skillUnlockableMaterial;

    SkillSlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        playerSkills = FindObjectOfType<PlayerSkills>();
        playerSkills.OnSkillUnlocked += OnSkillUnlocked;
        slots = skillsParent.GetComponentsInChildren<SkillSlot>();
        skillUI.SetActive(!skillUI.activeSelf);
    }

    public void ToggleSkills() {
        skillUI.SetActive(!skillUI.activeSelf);
        UpdateVisuals();
    }

    public void UpdateVisuals () {
        foreach (SkillSlot slot in slots) {
            if (playerSkills.IsSkillUnlocked(slot.baseSkill)) {
                slot.icon.material = null;
            } else if (playerSkills.CanUnlock(slot.baseSkill)) {
                slot.icon.material = skillUnlockableMaterial;
            } else  {
                slot.icon.material = skillLockedMaterial;
            }
        }
    }

    void OnSkillUnlocked() {
        UpdateVisuals();
    }
}
