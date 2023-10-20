using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private float _damage;
    private Rigidbody myRB;
    public bool holding = false;
    private bool _inContact = false;

    private AudioSource _thisAudioSource;
    [SerializeField] private AudioClip contactWoodClip;
    [SerializeField] private AudioClip takeBackAxeClip;

    [SerializeField] private GameObject treeChip;
    
    private void Start()
    {
        myRB = GetComponent<Rigidbody>();
        _thisAudioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (holding)
        {
            if (other.CompareTag("Wood"))
            {
                Debug.Log("Sending Damage");
                other.GetComponent<Wood>().TakeDamage(_damage);
                Instantiate(treeChip,
                    other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position),
                    Quaternion.identity);
                _thisAudioSource.PlayOneShot(contactWoodClip);
                _inContact = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(("Wood")) && _inContact == true)
        {
            _thisAudioSource.PlayOneShot(takeBackAxeClip);
            _inContact = false;
        }
    }

    private void Update()
    {
        if (holding)
        {
            Debug.Log(myRB.velocity.ToString());
        }
    }
}
