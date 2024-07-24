using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SecondaryButtonPair
{
    public string id;
    public GameObject reactObject;
}
public class EmojiBar : MonoBehaviour
{
    private Animator animator;
    private bool isUp = false;
    [SerializeField] private List<SecondaryButtonPair> secondaryButtonList = new List<SecondaryButtonPair>();
    private Dictionary<string, GameObject> secondaryButtonDict;
    GameObject currentSecondary = null;
    public float aliveTime;

    private void Awake()
    {
        secondaryButtonDict = secondaryButtonList.ToDictionary(pair => pair.id, pair => pair.reactObject);
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        this.gameObject.SetActive(false);
       


    }
    public void ActivateReactBar()
    {
        if (!isUp)  // Only activate if the bar isn't already up
        {
            this.gameObject.SetActive(true);
            StartCoroutine(ActivateObject());
        }
    }
    public void InstantiateSecondaryButtons(string id)
    {
        GameObject secondary;
        secondaryButtonDict.TryGetValue(id, out secondary);
        if (secondary != null)
        {
            currentSecondary = Instantiate(secondary, this.transform);
        }
    }
    IEnumerator ActivateObject()
    {
        animator.SetFloat("Speed", 1);
        animator.Play("EmojiBar", 0, 0);
        isUp = true;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(aliveTime);

        animator.SetFloat("Speed", -1);
        animator.Play("EmojiBar", 0, 1);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        this.gameObject.SetActive(false);
        if (currentSecondary != null)
        {
            Destroy(currentSecondary);
        }
        isUp = false;
    }
}
