using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform player;

    public Vector3 offset;

    public float damping;
    public float height;
    public float depth;

    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 movePosition = player.position + offset;

        //transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
        
        Vector3 pos = new Vector3();
        var playerpos = player.position;
        
        pos.x = playerpos.x + depth;
        pos.z = playerpos.z;
        pos.y = playerpos.y + height;
        
        transform.position = pos;
    }
}
    
    
  
    
    
