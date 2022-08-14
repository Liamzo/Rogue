using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkills : MonoBehaviour {
    
    Game game;

    public BaseUnitSkills baseUnitSkills;

    public UnitController unitController;

    protected List<BaseSkill> unlockedSkillsList = new List<BaseSkill>();

    // Start is called before the first frame update
    protected virtual void Start() {
        game = Game.instance;
    }

    public void SetDefaultSkills() {
        foreach (Skill skill in baseUnitSkills.skills) {
            UnlockSkill(new BaseSkill(skill));
        }
    }

    public virtual void UnlockSkill (BaseSkill skill) {
        unlockedSkillsList.Add(skill);

        skill.owner = unitController;
        skill.OnUnlock(skill);
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

    public BaseSkill GetSkill(int i) {
        if (unlockedSkillsList.Count == 0 || unlockedSkillsList.Count <= i) {
            return null;
        }
        BaseSkill skill = unlockedSkillsList[i];

        if (skill.CanBeActivated()) {
            return unlockedSkillsList[i];
        }

        return null;
    }
}
