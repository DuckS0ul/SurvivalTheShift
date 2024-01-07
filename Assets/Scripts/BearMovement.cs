using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

  
public class BearMovement : MonoBehaviour
{
    [SerializeField] private  Transform target;
    private  GameObject stateSystem;
    [SerializeField] private float stoppingDistance = 8.0f;
    private float currentDistance;
    [SerializeField] private  NavMeshAgent nav;
    private  float updateDelay = 0.3f;
    private float updateDeadline;

    [SerializeField] private int attackDamage = 10;

    private float declineInterval = 8f;
    private float curTime = 0;


    private Vector3 originalPos;
    [SerializeField] private float alertRadius = 30.0f;
    [SerializeField] private float wanderingRadius = 25.0f;

    private enum MovementState { idle, walking, attack, running, death };
    private Animator anim;
    private MovementState state;

    private bool isDead = false;


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
        UpdateAnimation();
    }

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        stateSystem = GameObject.Find("PlayerStateSystem");

        originalPos = transform.position;
        InvokeRepeating("Wandering", 0.0f, 8.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead) {
            currentDistance = Vector3.Distance(transform.position, target.position);
            //Debug.Log("currentDistance: " + currentDistance);
            if(currentDistance <= alertRadius){
                if(currentDistance <= stoppingDistance){
                    LookAtTarget();
                    state = MovementState.attack;
                    //Debug.Log("looking at player");

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
        else{
            state = MovementState.death;
            UpdateAnimation(); 
        }

        
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

        else if(state == MovementState.death)
        {
            anim.SetBool("Idle", false);
            anim.SetBool("WalkForward", false);
            anim.SetBool("Run Forward", false);
            anim.SetTrigger("Die");
        }
    }


    public void AttackPlayer()
    {
        if (stateSystem != null)
        {
            stateSystem.GetComponent<PlayerState>().currentHealth -= attackDamage;
            //Debug.Log("Attacked player, new health: " + stateSystem.GetComponent<PlayerState>().currentHealth);
        }
    }

    public void afterDeath()
    {
        isDead = true;
        CancelInvoke();
        nav.isStopped = true;
        nav.velocity = Vector3.zero;
    }
}


