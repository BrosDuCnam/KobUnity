using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    [SerializeField] private float _treeHealth;

    private Tree _thisTreeScript;


    private void Start()
    {
        _thisTreeScript = GetComponentInParent<Tree>();
    }

    public void TakeDamage(float damage)
    {
        _treeHealth -= damage;
         Debug.Log("Taking damage");
        if (_treeHealth <= 0)
        {
            _thisTreeScript.TreeHasBeenCut();
        }
    }
}
