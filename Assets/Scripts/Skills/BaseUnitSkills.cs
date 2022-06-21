using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Block", menuName = "Skills/New Skill Block")]
public class BaseUnitSkills : ScriptableObject {
    public List<Skill> skills;
}