using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : UnitSkills {

    public SkillsUI skillsUI;
    public event System.Action OnSkillUnlocked;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    public override void UnlockSkill(BaseSkill skill) {
        base.UnlockSkill(skill);
        skill.hotKey = (KeyCode) (48+unlockedSkillsList.Count);
        if (OnSkillUnlocked != null) {
            OnSkillUnlocked();
        }
    }



    public BaseSkill CheckSkillInput () {
        foreach(BaseSkill skill in unlockedSkillsList) {
            if (Input.GetKeyDown(skill.hotKey)) {
                if (skill.CanBeActivated()) {
                    return skill;
                }
            }
        }

        return null;
    }



    // UI

    public void ToggleSkills() {
        skillsUI.ToggleSkills();
    }
    
}
