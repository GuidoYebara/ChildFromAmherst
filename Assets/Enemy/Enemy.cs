using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Color patrollingColor,chasingColor;
    [SerializeField] private GameObject gamecontrollerGo;
    
    private Renderer myrenderer,myVisionRenderer,myEyeRenderer;
    private NavMeshAgent agent;
    private GameObject myVision,myEye;
    private GameController gamecontroller;
    //Chasing player
    [SerializeField] private float chasespeed,chaseangularspeed,resetCaptureTimer,timerCaptureFactor;
    private float captureTimer;
    private bool touched;
    
    //NAV
    [SerializeField] GameObject[] waypoints;
    private int waypointIndex;
    private Vector3 target;
    private Vector3 lastAgentVel;
    private float navTimer;
    private bool waiting;

    
    //EnemyStates
    private enemyState currentState;
    private enum enemyState
    {
        patrolling,
        waiting,
        chasing
    };
    void Start()
    {
        myVision = transform.Find("Cone").gameObject;
        myEye = transform.Find("Eye").gameObject;
        
        myrenderer = GetComponent<Renderer>();
        myVisionRenderer = myVision.GetComponent<Renderer>();
        myEyeRenderer= myEye.GetComponent<Renderer>();
        
        agent = GetComponent<NavMeshAgent>();
        gamecontroller = gamecontrollerGo.GetComponent<GameController>();
        
        UpdateDestination();

        UpdateWaypointProperties(waypointIndex);
        
        ResetCapture();
        
        ChangeColor(patrollingColor);
        
        //Start state machine
        currentState = enemyState.patrolling;
        
    }
    
    void Update()
    {
        switch (currentState)
        {
            case enemyState.patrolling:
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                    IterateWaypointIndex();

                break;
            }
            case enemyState.waiting:
            {
                navTimer = (navTimer <= 0) ? 0 : navTimer -= Time.deltaTime;
                
                if(navTimer==0)
                    ResumeAgent();
                
                break;
            }
            case enemyState.chasing:
            {
                if(!touched)
                    captureTimer = (captureTimer >= resetCaptureTimer) ? resetCaptureTimer : captureTimer += Time.deltaTime*timerCaptureFactor;

                Debug.Log(touched.ToString());
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        DetectRotation();
    }

    private void UpdateDestination()
    {
        target = waypoints[waypointIndex].transform.position;
        agent.SetDestination(target);
    }

    private void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
        
        PauseAgent();
    }
    
    private void PauseAgent()
    {
        currentState = enemyState.waiting;
        //lastAgentVel = agent.velocity;
        //agent.velocity = Vector3.zero;
        agent.ResetPath();
    }
     
    private void ResumeAgent()
    {
        currentState = enemyState.patrolling;
        
        //agent.velocity = lastAgentVel;
        UpdateDestination();

        UpdateWaypointProperties(waypointIndex);
    }

    private void UpdateWaypointProperties(int index)
    {
        Waypoint wayp = waypoints[index].GetComponent<Waypoint>();
        navTimer = wayp.timeafterchecked;
        agent.speed = wayp.agentspeed;
        agent.angularSpeed = wayp.angularspeed;
    }
    
    public void DetectPlayer()
    {
        ChangeColor(chasingColor);
        
        currentState = enemyState.chasing;
        
        agent.SetDestination(player.transform.position);
        agent.speed = chasespeed;
        agent.angularSpeed = chaseangularspeed;
    }

    public void PlayerOutOfSight()
    {
        //renderer.material.color = Color.red;
    }

    private void DetectRotation()
    {
        float rot = transform.rotation.eulerAngles.y;

        if ((rot > -15 && rot < 15 || rot > 345) || rot > 165 && rot < 190 ) 
        {
            myVisionRenderer.enabled = true;
        }
        else
        {
            myVisionRenderer.enabled = false;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
            touched = true;
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (currentState)
            {
                case enemyState.patrolling:
                case enemyState.waiting:
                {
                    DetectPlayer();
                    break;
                }
                case enemyState.chasing:
                {
                    captureTimer = (captureTimer <= 0) ? 0 : captureTimer -= Time.deltaTime;
        
                    if(captureTimer==0)
                        CapturePlayer();
                    break;
                }
            }
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            touched = false;
        }
    }

    private void CapturePlayer()
    {
        gamecontroller.LostGame();
    }

    private void ResetCapture()
    {
        captureTimer = resetCaptureTimer;
    }

    private void ChangeColor(Color color)
    {
        myrenderer.material.color = color;
        myVisionRenderer.material.color = color;
        myEyeRenderer.material.color = color;
    }
    
}
