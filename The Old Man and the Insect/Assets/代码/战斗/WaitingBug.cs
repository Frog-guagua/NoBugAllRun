using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingBug : MonoBehaviour
{
    [SerializeField]List<GameObject> WaitingBugs = new List<GameObject>();
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void BugUp(int index)
    {
        WaitingBugs[index].SetActive(true);
    }
}
