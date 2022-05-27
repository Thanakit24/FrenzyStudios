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

    [Header("Backgrounds")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject background2;
    [SerializeField] private GameObject background3;
    [SerializeField] private GameObject background4;
    [SerializeField] private GameObject background5;
    [SerializeField] private GameObject background6;

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

                interacted = true;
			}

            if (textid == 2 && interacted == false)
            {
                StartCoroutine(Two());
                Destroy(background);

                interacted = true;
            }

            if (textid == 3 && interacted == false)
            {
                Destroy(background2);
                StartCoroutine(ThreeFour());

                interacted = true;
            }

            if (textid == 5 && interacted == false)
            {
                Destroy(background3);
                StartCoroutine(Five());

                interacted = true;
            }

            if (textid == 6 && interacted == false)
            {
                Destroy(background4);
                StartCoroutine(Six());

                interacted = true;
            }

            if (textid == 7 && interacted == false)
            {
                Destroy(background5);
                StartCoroutine(Seven());

                interacted = true;
            }
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

        Destroy(text1);
        Destroy(wKey);
        Destroy(aKey);
		Destroy(sKey);
        Destroy(dKey);
        Destroy(mouse);
        Destroy(background);
		Destroy(this.gameObject);
    }

    IEnumerator Two()
	{
        Destroy(text1);
        Destroy(wKey);
        Destroy(aKey);
        Destroy(sKey);
        Destroy(dKey);
        Destroy(mouse);

        text2.gameObject.SetActive(true);
        spaceKey.gameObject.SetActive(true);
        background2.gameObject.SetActive(true);


        yield return new WaitForSeconds(5);

        Destroy(text2);
        Destroy(spaceKey);
        Destroy(background2);
        Destroy(this.gameObject);
    }

    IEnumerator ThreeFour()
	{
        Destroy(text2);
        Destroy(spaceKey);

        text3.gameObject.SetActive(true);
        RMB.gameObject.SetActive(true);
        background3.gameObject.SetActive(true);

        yield return new WaitForSeconds(6);

        Destroy(text3);
        RMB.gameObject.SetActive(false);

        text4.gameObject.SetActive(true);
        eKey.gameObject.SetActive(true);

        yield return new WaitForSeconds(6);

        Destroy(text4);
        Destroy(eKey);
        Destroy(background3);

        Destroy(this.gameObject);
    }

    IEnumerator Five()
	{
        Destroy(text3);
        RMB.gameObject.SetActive(false);
        Destroy(text4);
        Destroy(eKey);

        text5.gameObject.SetActive(true);
        rKey.gameObject.SetActive(true);
        background4.gameObject.SetActive(true);

        yield return new WaitForSeconds(7);

		Destroy(text5);
        Destroy(rKey);
        Destroy(background4);

        Destroy(this.gameObject);
    }

    IEnumerator Six()
    {
        Destroy(text5);
        Destroy(rKey);

        text6.gameObject.SetActive(true);
        RMB.gameObject.SetActive(true);
        LMB.gameObject.SetActive(true);
        background5.gameObject.SetActive(true);

        yield return new WaitForSeconds(7);

        Destroy(text6);
        Destroy(RMB);
        Destroy(LMB);
        Destroy(background5);
        Destroy(this.gameObject);
    }

    IEnumerator Seven()
	{
        Destroy(text6);
        Destroy(RMB);
        Destroy(LMB);

        text7.gameObject.SetActive(true);
        background6.gameObject.SetActive(true);

        yield return new WaitForSeconds(6);

        Destroy(text7);
        Destroy(background6);
        Destroy(this.gameObject);
    }
}
