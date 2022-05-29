using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinHolder : MonoBehaviour
{
    public int skinID = 0;
    public Material[] handSkins;
    public Material[] shurikenSkins;

    public SkinnedMeshRenderer handModel, shurikenModel1, shurikenModel2;

    private void Start()
    {
        //UpdateSkin();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchSkin();
        }
    }

    public void SwitchSkin()
    {
        if (skinID < handSkins.Length)
            skinID++;
        else
            skinID = 0;

        UpdateSkin();
    }

    public void UpdateSkin()
    {
        handModel.material = handSkins[skinID];
        shurikenModel1.material = shurikenSkins[skinID];
        shurikenModel2.material = shurikenSkins[skinID];
    }
}
