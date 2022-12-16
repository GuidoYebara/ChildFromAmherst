using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private float resetDetectionTimer;
    [SerializeField] private GameObject _player;
    [SerializeField] private float visionDistance;
    [SerializeField] private float timerCdFactor;
    
    private float detectionTimer;
    
    private Enemy parent;
    private Transform enemyEye;
    
    private bool seen;
    private bool solidInTheMiddle;
    
    private int layers;

    private Transform playerTrans;
    private void Update()
    {
        if(!seen)
            detectionTimer = (detectionTimer >= resetDetectionTimer) ? resetDetectionTimer : detectionTimer += Time.deltaTime*timerCdFactor;
        
        Vector3 direction = playerTrans.position - enemyEye.position;
        
        Debug.DrawRay(enemyEye.position, direction);
        
    }

    private void Start()
    {
        //Reference to father
        GameObject _parentObject = gameObject.transform.parent.gameObject;
        parent = _parentObject.GetComponent<Enemy>();
        enemyEye = _parentObject.transform.Find("Eye");

        ResetDetectionTimer();
        
        playerTrans = _player.transform;
        //Obtain solid mask
        layers = LayerMask.GetMask("Solid","Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            seen = true;
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player") && !solidInTheMiddle)
        {
            detectionTimer = (detectionTimer <= 0) ? 0 : detectionTimer -= Time.deltaTime;
        
            if(detectionTimer==0)
                parent.DetectPlayer();
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            seen = false;
            parent.PlayerOutOfSight();
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = playerTrans.position - enemyEye.position;
        Vector3.Normalize(direction);
        
        RaycastHit hit;
        if (Physics.Raycast(enemyEye.position, direction,out hit, visionDistance, layers) && 
            hit.transform.gameObject.CompareTag("Solid"))
        {
            solidInTheMiddle = true;
        }
        else
            solidInTheMiddle = false;
    }

    public void ResetDetectionTimer()
    {
        detectionTimer = resetDetectionTimer;
    }
}
