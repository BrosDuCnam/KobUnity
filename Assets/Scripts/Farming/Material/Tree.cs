using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Wood _woodScript;

    [SerializeField] private GameObject completeTree;
    [SerializeField] private GameObject topPartTree;
    [SerializeField] private GameObject bottomPartTree;


    private void Start()
    {
        _woodScript = GetComponentInChildren<Wood>();
    }

    public void TreeHasBeenCut()
    {
        completeTree.SetActive(false);
        bottomPartTree.SetActive(true);
        topPartTree.SetActive(true);
    }
}
