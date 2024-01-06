using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip rabbitHitAndScream;
    [SerializeField] AudioClip rabbitHitAndDie;

    private Animator animator;
    public bool isDead;

    [SerializeField] ParticleSystem bloodSlashParticles;
    [SerializeField] GameObject bloodPuddle;

    enum AnimalType
    {
        Rabbit,
        Bear
    }

    [SerializeField] AnimalType thisAnimalType;

    private void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead == false)
        {
            currentHealth -= damage;

            bloodSlashParticles.Play();

            if (currentHealth <= 0)
            {
                PlayDyingSound();

                animator.SetTrigger("Die");
                GetComponent<AI_Movement>().enabled = false;

                StartCoroutine(PuddleDelay());

                isDead = true;
            }
            else
            {
                PlayHitSound();

            } 
        }
    }


    IEnumerator PuddleDelay()
    {
        yield return new WaitForSeconds(1f);
        bloodPuddle.SetActive(true);
    }





    private void PlayDyingSound()
    {
        switch (thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndDie);
                break;
            case AnimalType.Bear:
                soundChannel.PlayOneShot(rabbitHitAndDie);
                break;
            default:
                break;
        }

    }

    private void PlayHitSound()
    {
        switch (thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitAndScream);
                break;
            case AnimalType.Bear:
                soundChannel.PlayOneShot(rabbitHitAndScream);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


}
