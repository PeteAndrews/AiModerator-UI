using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreInfoTabStrategy : MonoBehaviour, ITabStrategy
{
    public void Activate()
    {
        //UnityClientSender.Instance.SendMoreInfoRequest();
    }
    public void Deactivate()
    {
        //Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
    }

}
// Activate - sendmoreinforequest - activate 