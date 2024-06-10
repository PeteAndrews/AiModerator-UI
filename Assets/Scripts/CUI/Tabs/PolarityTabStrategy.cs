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
        throw new System.NotImplementedException();
    }

}
