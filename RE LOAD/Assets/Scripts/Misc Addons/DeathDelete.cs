using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDelete : MonoBehaviour
{
    [SerializeField] GameObject baton;
    [SerializeField] GameObject shield;
    
    public void DeathDestroy()
	{
        Destroy(baton);
        Destroy(shield);
	}
}
