using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keen Eye", menuName = "Skills/Sword/KeenEye")]
public class KeenEye : Skill
{
    public int duration;

    public override bool Use (BaseSkill baseSkill) {
        BaseEffect effect = new CounterEffect(baseSkill.owner, duration);

        return true;
    }
}
