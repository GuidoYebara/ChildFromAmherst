using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LadderZone : MonoBehaviour
{
    private Player player;
    private Vector3 otherLadderEnd;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        String othername = (this.name == "UpZone") ? "DownZone" : "UpZone";
        
        otherLadderEnd = transform.parent.gameObject.transform.Find(othername).transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.ChangeTriggerState(Player.triggerState.ladder);
            player.ChangeLadderEndPosition(otherLadderEnd);
        }
        
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.ChangeTriggerState(Player.triggerState.none);
        }
    }
}
