using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingBug : MonoBehaviour
{
    [SerializeField]List<GameObject> WaitingBugs = new List<GameObject>();
    
    void Start()
    {
        foreach (var waitbug in  WaitingBugs)
        {
            
            waitbug.GetComponent<Collider2D>().enabled = false;
            waitbug.GetComponent<SpriteRenderer>().enabled = false;
            waitbug.transform.GetChild(0).gameObject.SetActive(false);
            
        }
    }

    
    void Update()
    {
        
    }

    public void BugUp(int index)
    {
        WaitingBugs[index].GetComponent<Collider2D>().enabled = true;
        WaitingBugs[index].GetComponent<SpriteRenderer>().enabled = true;
        WaitingBugs[index].transform.GetChild(0).gameObject.SetActive(true);
    }
}
