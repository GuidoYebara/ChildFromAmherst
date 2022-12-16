using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject child;
    
    [SerializeField] private float speed;
    [SerializeField] private float maxvel;
    
    private Rigidbody rb;
    private Animator anim;
    private CapsuleCollider collid;
    private Transform trans;
    
    public enum triggerState
    {
        none,
        ladder,
        door
    };

    private triggerState currentTriggerState;
    
    //Player states
    
    private playerState currentState;
    private enum playerState
    {
        moving,
        waiting,
        idle,
        climbingLadder
    };
    
    
    //Controls
    public PlayerControls.NormalActions _ControlsNormalActions;
    
    private Vector3 movVector;
    private Vector3 mousePos;
    private bool useKey;
    private bool cantMove;
    
    private Vector3 ladderEndPosition;

    [SerializeField] private float idleTimerMax;
    private float idleTimer;
    
    [SerializeField] private float ladderCdTimerMax;
    private float ladderCdTimer;
    
    [SerializeField] private float climbingTimerMax;
    private float climbingTimer;

    private float myscaleZ;
    
    void Start()
    {
        _ControlsNormalActions = new PlayerControls().Normal;
        _ControlsNormalActions.Enable();

        rb = gameObject.GetComponent<Rigidbody>();
       
        collid = gameObject.GetComponent<CapsuleCollider>();

        anim = gameObject.GetComponent<Animator>();
        
        movVector = Vector3.zero;

        currentTriggerState = triggerState.none;
        currentState = playerState.waiting;

        idleTimer = idleTimerMax;
        ladderCdTimer = ladderCdTimerMax;
        climbingTimer = climbingTimerMax;

        myscaleZ = transform.localScale.z;
    }
    
    void Update()
    {
        Debug.Log(currentState.ToString());
        
        SetAnimations();
        
        switch (currentState)
        {
            case playerState.moving:
            {
                ReadInput();
                PerformUseAction();
                UpdateLadderTimer();
                //Moving animations
                
                //Detect if stopped moving
                if (movVector == Vector3.zero)
                    currentState = playerState.waiting;
                
                Flip();
                
                break;
            }
            case playerState.waiting:
            {
                ReadInput();
                PerformUseAction();
                UpdateLadderTimer();
                
                idleTimer = (idleTimer <= 0) ? 0 : idleTimer -= Time.deltaTime;

                if (idleTimer == 0)
                {
                    idleTimer = idleTimerMax;
                    currentState = playerState.idle;
                }
                
                break;
            }
            case playerState.idle:
            {
                ReadInput();
                PerformUseAction();
                UpdateLadderTimer();
                
                break;
            }
            
            case playerState.climbingLadder:
            {
                cantMove = true;
                
                climbingTimer = (climbingTimer <= 0) ? 0 : climbingTimer -= Time.deltaTime;

                if (climbingTimer == 0)
                {
                    transform.position = ladderEndPosition;
                    climbingTimer = climbingTimerMax;
                    ladderCdTimer = ladderCdTimerMax;
                    currentState = playerState.waiting;
                    cantMove = false;
                }
                
                break;
            }
        }
  
    }
    
    void FixedUpdate()
    {
        if(!cantMove)
            Move();
    }
    
    private void ReadInput()
    {
        //read AD, movement
        Vector2 _GetmovVec = _ControlsNormalActions.Move.ReadValue<Vector2>();
        movVector.Set(0,0,_GetmovVec.x);
        
        //read mouse coordinates
        Vector2 _GetmousePos = _ControlsNormalActions.Look.ReadValue<Vector2>();
        mousePos.Set(_GetmousePos.x,_GetmousePos.y,0);
        
        //Read W for certain uses
        useKey = _ControlsNormalActions.Use.IsPressed();
    }
    private void Move()
    {
        Vector3 targetvelocity = movVector * speed;

        if (rb.velocity.z < maxvel && rb.velocity.z > maxvel * -1)
            rb.AddForce(targetvelocity, ForceMode.Impulse);
            
        if(targetvelocity!=Vector3.zero)
            currentState = playerState.moving;

    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;

        if (movVector.z < 0)
            scale.z = myscaleZ * -1;

        if (movVector.z > 0)
            scale.z = myscaleZ;
        
        
        transform.localScale = scale;
    }

    private void SetAnimations()
    {
        anim.SetBool("Climb",currentState==playerState.climbingLadder);
        anim.SetBool("Wait",currentState==playerState.waiting);
        anim.SetBool("Idle",currentState==playerState.idle);
        anim.SetBool("Walk",currentState==playerState.moving);
    }
    public void ChangeTriggerState(triggerState newState)
    {
        currentTriggerState = newState;
    }
    public void ChangeLadderEndPosition(Vector3 NewPos)
    {
        ladderEndPosition = NewPos;
    }

    private void UpdateLadderTimer()
    {
        ladderCdTimer = (ladderCdTimer <= 0) ? 0 : ladderCdTimer -= Time.deltaTime;
    }
    private void PerformUseAction()
    {
        if (useKey)
        {
            switch (currentTriggerState)
            {
                case triggerState.none:
                {
                    break;
                }
                case triggerState.ladder:
                {
                    if(ladderCdTimer==0)
                        currentState = playerState.climbingLadder;
                    
                    break;
                }
                case triggerState.door:
                {
                    break;
                }
            }
        }
    }
    
}
