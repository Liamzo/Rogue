using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public Image icon;

    public BaseSkill baseSkill;
    public Skill skill;

    public static PlayerSkills playerSkills;

    void Start () {
        //skill = new BaseSkill(Game.instance.allSkills[0]);
        if (playerSkills == null) {
            playerSkills = FindObjectOfType<PlayerSkills>();
        }

        baseSkill = new BaseSkill(skill);

        icon.sprite = baseSkill.skill.icon;
        icon.enabled = true;
    }

    public void ClickSlot () {
        Debug.Log("click");
        if (baseSkill != null) {
            playerSkills.TryUnlockSkill(baseSkill);
        }
    }
}
