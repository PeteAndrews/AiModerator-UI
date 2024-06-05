using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManifestoTabStrategy : MonoBehaviour, ITabStrategy
{
    public void Activate()
    {
        
        //Grab and display viewpoint
        CuiManager.Instance.HandleManifestoActivation();
    }

    public void Deactivate()
    {
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }


}
