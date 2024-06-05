using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarityTabStrategy : MonoBehaviour, ITabStrategy
{
    public void Activate()
    {
        UnityClientSender.Instance.ReceiveButtonName("polarity");

    }

    public void Deactivate()
    {
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

}
