using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject popUpBox;
    public Animator animator;

    public void PopUp()
    {
        popUpBox.SetActive(true);
        animator.SetTrigger("pop");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
