using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public Tree[] trees;


    public void Start()
    {
        foreach (Tree tree in trees)
        {
            foreach (var item in tree.branch)
            {
                item.unlocked = false;
            }
            tree.branch[0].unlocked = true;
        }
    }


    public void UnlockNextInBranch(SkillHolder skill, SkillTreeBranch branchName)
    {
        Vector2Int temp = SearchForSkillNumber(skill, branchName);
        int skillID = temp.x;
        int branchID = temp.y;

        //Unlock
        if (skillID < trees[branchID].branch.Length)
        {
            trees[branchID].branch[skillID + 1].unlocked = true;
        }
    }


    private int SearchForBranchByEnum(SkillTreeBranch inputBranchName)
    {
        for (int i = 0; i < trees.Length; i++)
        {
            if (trees[i].branchName.Equals(inputBranchName)) return i;
        }
        return 0;
    }

    private Vector2Int SearchForSkillNumber(SkillHolder skillHolder, SkillTreeBranch inputBranchName)
    {
        int branchID = SearchForBranchByEnum(inputBranchName);
        for (int i = 0; i < trees[branchID].branch.Length; i++)
        {
            if (trees[branchID].branch[i].Equals(skillHolder)) return Vector2Int.right * i + Vector2Int.up * branchID;
        }
        return Vector2Int.zero;
    }
}

[System.Serializable]
public class Tree
{
    public SkillTreeBranch branchName;
    public GameObject branchParent;
    public SkillHolder[] branch;
}
