using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManifestoTabStrategy : MonoBehaviour, ITabStrategy
{
    /*
     Thinking will need a ToogleEvent which is triggered by a click event
     The toggle event brings to the next followup message
     */
    public void Activate()
    {
        CuiManager.Instance.HandleManifestoActivation();
        //Grab and display viewpoint
        //CuiManager.Instance.HandleManifestoActivation();
        //print out the summary
        //throw new System.NotImplementedException();
        Debug.Log("Manifesto Tab Activated");
    }

    public void Deactivate()
    {

        //DESTROY AND ADD MESSAGE "What do you think? <link=opinion><color=blue>Share your opinion</color></link>. Want more info? <link=followUp><color=blue>Find out more</color></link>.";
        //DO through CuiManager
        //throw new System.NotImplementedException();
        //Destroy(gameObject);
    }
}
