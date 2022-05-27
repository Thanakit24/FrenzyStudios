using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] private int textid;

    private bool interacted = false;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;
    [SerializeField] private TextMeshProUGUI text4;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private TextMeshProUGUI text6;
    [SerializeField] private TextMeshProUGUI text7;

    [Header("Pics")]
    [SerializeField] private GameObject wKey;
    [SerializeField] private GameObject sKey;
    [SerializeField] private GameObject aKey;
    [SerializeField] private GameObject dKey;
    [SerializeField] private GameObject eKey;
    [SerializeField] private GameObject rKey;
    [SerializeField] private GameObject spaceKey;
    [SerializeField] private GameObject mouse;
    [SerializeField] private GameObject LMB;
    [SerializeField] private GameObject RMB;
    [SerializeField] private GameObject background;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == "Player")
		{
            if (textid == 1 && interacted == false)
			{
                StartCoroutine(One());
			}

            if (textid == 2 && interacted == false)
            {
                StopCoroutine(One());
                StartCoroutine(Two());
            }

            if (textid == 3 && interacted == false)
            {
                StopCoroutine(Two());
                StartCoroutine(ThreeFour());
            }

            if (textid == 5 && interacted == false)
            {
                StopCoroutine(ThreeFour());
                StartCoroutine(Five());
            }

            if (textid == 6 && interacted == false)
            {
                StopCoroutine(Five());
                StartCoroutine(Six());
            }

            if (textid == 7 && interacted == false)
            {
                StopCoroutine(Six());
                StartCoroutine(Seven());
            }

            interacted = true;
        }
    }

    IEnumerator One()
	{
        text1.gameObject.SetActive(true);
        wKey.gameObject.SetActive(true);
        sKey.gameObject.SetActive(true);
        aKey.gameObject.SetActive(true);
        dKey.gameObject.SetActive(true);
        mouse.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        yield return new WaitForSeconds(8);

        text1.gameObject.SetActive(false);
        wKey.gameObject.SetActive(false);
        sKey.gameObject.SetActive(false);
        aKey.gameObject.SetActive(false);
        dKey.gameObject.SetActive(false);
        mouse.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    IEnumerator Two()
	{
        text1.gameObject.SetActive(false);
        wKey.gameObject.SetActive(false);
        sKey.gameObject.SetActive(false);
        aKey.gameObject.SetActive(false);
        dKey.gameObject.SetActive(false);
        mouse.gameObject.SetActive(false);

        text2.gameObject.SetActive(true);
        spaceKey.gameObject.SetActive(true);
        background.gameObject.SetActive(true);


        yield return new WaitForSeconds(5);

        text2.gameObject.SetActive(false);
        spaceKey.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    IEnumerator ThreeFour()
	{
        text2.gameObject.SetActive(false);
        spaceKey.gameObject.SetActive(false);

        text3.gameObject.SetActive(true);
        RMB.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        yield return new WaitForSeconds(6);

        text3.gameObject.SetActive(false);
        RMB.gameObject.SetActive(false);

        text4.gameObject.SetActive(true);
        eKey.gameObject.SetActive(true);

        yield return new WaitForSeconds(6);

        text4.gameObject.SetActive(false);
        eKey.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        Destroy(this.gameObject);
    }

    IEnumerator Five()
	{
        text3.gameObject.SetActive(false);
        RMB.gameObject.SetActive(false);
        text4.gameObject.SetActive(false);
        eKey.gameObject.SetActive(false);

        text5.gameObject.SetActive(true);
        rKey.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        yield return new WaitForSeconds(7);

        text5.gameObject.SetActive(false);
        rKey.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        Destroy(this.gameObject);
    }

    IEnumerator Six()
    {
        text5.gameObject.SetActive(false);
        rKey.gameObject.SetActive(false);

        text6.gameObject.SetActive(true);
        RMB.gameObject.SetActive(true);
        LMB.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        yield return new WaitForSeconds(7);

        text6.gameObject.SetActive(false);
        RMB.gameObject.SetActive(false);
        LMB.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    IEnumerator Seven()
	{
        text6.gameObject.SetActive(false);
        RMB.gameObject.SetActive(false);
        LMB.gameObject.SetActive(false);

        text7.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        yield return new WaitForSeconds(6);

        text7.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
