using UnityEngine;
using UnityEngine.UI;

public class SkillHolder : MonoBehaviour
{
    private SkillTreeManager skillTreeManager;
    public SkillBase skill;
    public Image image;
    public Button button;

    public bool unlocked = true;
    public bool isActive = false;

    void Start()
    {
        skillTreeManager = GetComponentInParent<SkillTreeManager>();
        image = GetComponent<Image>();
        image.sprite = skill.icon;
    }

    public void Update()
    {
        if (unlocked) image.color = button.colors.disabledColor;

    }

    public void Activate()
    {
        if (unlocked)
        {
            if (GameManager.instance.tokenCount >= skill.requiredTokens)
            {
                GameManager.instance.tokenCount -= skill.requiredTokens;
                skillTreeManager.UnlockNextInBranch(this ,skill.type);
                Debug.Log(name + " activated");
                isActive = true;
            }
            else
            {
                Debug.Log("not enough");
            }
        }
        //button.interactable = unlocked;
    }

    public void Deactivate()
    {
        GameManager.instance.tokenCount += skill.requiredTokens;
        isActive = false;
        //button.interactable = unlocked;
    }
}
