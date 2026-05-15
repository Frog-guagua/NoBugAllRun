using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grassController : MonoBehaviour
{   
    public static grassController Instance { get; private set; }
    public GameObject target;
    Animator animator;
    public Transform rightpos;
    public List<GameObject> objList=new List<GameObject>();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void MovetoLeft()
    {
        animator.SetTrigger("toLeft");
        
    }

    public void MoveFromRight()
    {
        
        animator.SetTrigger("fromRight");
    }

   
}