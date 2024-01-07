using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

  
public class BearMovement : MonoBehaviour
{
    [SerializeField] private  Transform target;
    [SerializeField] private float stoppingDistance = 8.0f;
    private float currentDistance;
    [SerializeField] private  NavMeshAgent nav;
    private  float updateDelay = 0.3f;
    private float updateDeadline;

    //[SerializeField] private  int enemyType;
    private Vector3 originalPos;
    [SerializeField] private float alertRadius = 30.0f;
    [SerializeField] private float wanderingRadius = 25.0f;

    private enum MovementState { idle, walking, attack, running };
    private Animator anim;
    private MovementState state;

    private void LookAtTarget() {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }

    private void TowardsTarget() {
        if ( Time.time >= updateDeadline ){
            updateDeadline = Time.time + updateDelay;
            nav.SetDestination(target.position);
        }
    }

    private void Wandering() {
        Vector3 randomDirection = Random.insideUnitSphere * wanderingRadius;
        nav.SetDestination(originalPos + randomDirection);
        LookAtTarget();
        state = MovementState.walking; 
        Debug.Log("wandering....walking");
        UpdateAnimation();
    }

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        //stoppingDistance = nav.stoppingDistance;

        anim = GetComponent<Animator>();

        originalPos = transform.position;
        InvokeRepeating("Wandering", 0.0f, 8.0f);
    }

    // Update is called once per frame
    void Update()
    {
        currentDistance = Vector3.Distance(transform.position, target.position);
        Debug.Log("currentDistance: " + currentDistance);
        if(currentDistance <= alertRadius){
            if(currentDistance <= stoppingDistance){
                LookAtTarget();
                state = MovementState.attack;
                Debug.Log("looking at player");

            }
            else{
                TowardsTarget();  
                state = MovementState.running; 
                Debug.Log("running to player");

            }
        }
        else {
            if(nav.remainingDistance <= nav.stoppingDistance){
                state = MovementState.idle;
            }
        }

        // Bear animation test
        UpdateAnimation();

        
    }

    private void UpdateAnimation()
    {
        if(state == MovementState.walking)
        {
            anim.SetBool("WalkForward", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Run Forward", false);

        }
        else if(state == MovementState.idle)
        {
            anim.SetBool("Idle", true);
            anim.SetBool("WalkForward", false);
            anim.SetBool("Run Forward", false);

        }
        else if(state == MovementState.running)
        {
            anim.SetBool("Run Forward", true);
            anim.SetBool("Idle", false);
            anim.SetBool("WalkForward", false);
        }
        else if(state == MovementState.attack)
        {
            anim.SetBool("Idle", true);
            anim.SetBool("WalkForward", false);
            anim.SetBool("Run Forward", false);
            anim.SetTrigger("Attack1");
        }


    }
}


