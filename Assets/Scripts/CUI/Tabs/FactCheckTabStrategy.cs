using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactCheckTabStrategy : MonoBehaviour, ITabStrategy
{
    public void Activate()
    {
        UnityClientSender.Instance.ReceiveButtonName("fact check");
    }

    public void Deactivate()
    {
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

}
