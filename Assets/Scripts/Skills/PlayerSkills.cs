using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : UnitSkills {

    public SkillsUI skillsUI;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
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
