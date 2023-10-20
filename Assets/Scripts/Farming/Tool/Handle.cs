using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Handle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI grabText;

    private GameObject _player;

    private bool _canTake;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            grabText.gameObject.SetActive(true);
            _canTake = true;
            _player = other.GetComponentInChildren<Camera>().gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            grabText.gameObject.SetActive(false);
            _canTake = false;
        }
    }

    private void Update()
    {
        if (_canTake)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                this.transform.SetParent(_player.transform);
                this.transform.localPosition = new Vector3(0, -.5f, 1);
                this.transform.localRotation = Quaternion.Euler(new Vector3(-90, 90, 90));
                GetComponentInChildren<Axe>().holding = true;
            }
        }
    }
}
