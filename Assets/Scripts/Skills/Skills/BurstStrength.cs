using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BoS", menuName = "Skills/BoS")]
public class BurstStrength : Skill
{
    public StatValue sv;

    public override bool Use (BaseSkill baseSkill) {
        BaseEffect effect = new StatEffect(sv, baseSkill.owner, 3);

        return true;
    }
}
