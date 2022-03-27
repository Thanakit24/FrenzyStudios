using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTreeBranch
{
    SHURIKEN,
    AGILITY,
    SURVIVAL
}

[CreateAssetMenu(fileName = "newSkill", menuName = "ScriptableObjects/Skills", order = 1)]
public class SkillBase : ScriptableObject
{
    public Sprite icon;
    public SkillTreeBranch type;

    public int requiredTokens = 1;
}
