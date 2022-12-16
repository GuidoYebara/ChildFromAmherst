using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Winning : MonoBehaviour
{
    [SerializeField] private GameObject gamecontroller;

    private GameController menu;

    private void Start()
    {
        menu = gamecontroller.GetComponent<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            menu.WonGame();
        }
        
    }
}
