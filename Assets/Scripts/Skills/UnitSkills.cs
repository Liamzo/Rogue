using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkills : MonoBehaviour {
    
    Game game;
    public UnitController unitController;

    protected List<BaseSkill> unlockedSkillsList;
    public event System.Action OnSkillUnlocked;

    // Start is called before the first frame update
    protected virtual void Start() {
        game = Game.instance;

        unlockedSkillsList = new List<BaseSkill>();
    }

    public void UnlockSkill (BaseSkill skill) {
        unlockedSkillsList.Add(skill);

        skill.OnUnlock();
        skill.hotKey = (KeyCode) (48+unlockedSkillsList.Count);
        skill.owner = unitController;
        OnSkillUnlocked();
    }

    public bool TryUnlockSkill (BaseSkill skill) {
        if (!CanUnlock(skill)) {
            return false;
        }

        if (IsSkillUnlocked(skill)) {
            return false;
        }

        UnlockSkill(skill);
        return true;
    }

    public bool IsSkillUnlocked(BaseSkill skill) {
        foreach (BaseSkill unlocked in unlockedSkillsList) {
            if (skill.skill.Equals(unlocked.skill)) {
                return true;
            }
        }
        return false;
    }

    public List<BaseSkill> GetUnlockedSkills() {
        return unlockedSkillsList;
    } 

    public bool CanUnlock (BaseSkill skill) {
        if (skill != null) {
            foreach (Skill require in skill.skill.requiredSkills) {
                if (!IsSkillUnlocked(new BaseSkill(require))) {
                    return false;
                }
            }
        }

        return true;
    }
}
