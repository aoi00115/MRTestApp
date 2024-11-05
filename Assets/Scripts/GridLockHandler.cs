using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridLockHandler : MonoBehaviour
{
    bool isGridLocked = false;
    public BoxCollider grid;
    public TextMeshPro gridLockText;
    public GameObject lockIcon;
    public GameObject unlockIcon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGridLockHandlerClicked()
    {
        if(isGridLocked)
        {
            isGridLocked = false;
            grid.enabled = true;
            gridLockText.text = "Lock Grid";
            lockIcon.SetActive(true);
            unlockIcon.SetActive(false);
        }
        else
        {
            isGridLocked = true;
            grid.enabled = false;
            gridLockText.text = "Unlock Grid";
            lockIcon.SetActive(false);
            unlockIcon.SetActive(true);
        }
    }
}
