using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehavior : MonoBehaviour
{
    public Transform[] foodStations; // Referenser till matstationer
    public Transform[] cashRegisters; // Referenser till kassor

    private NavMeshAgent agent; // NavMeshAgent som styr karaktärens rörelse
    private Transform chosenFoodStation; // Den slumpmässigt valda matstationen
    private float waitTime = 5f; // Tid vid matstationen
    private float startTime; // Tidpunkt när karaktären startade
    private Animator animator; // Hantera karaktärens animationer
    private bool isTakingFood = false; // Kontroll om karaktären är vid matstationen
    private bool hasLoggedTime = false; //  Kontroll om tiden redan har loggats

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Hämtar NavMeshAgent-komponenten
        animator = GetComponent<Animator>(); // Hämtar Animator-komponenten
        startTime = Time.time; // Timer när karaktären börjar röra sig

        // Välj en slumpmässig matstation från listan
        chosenFoodStation = GetRandomFoodStation();
        if (chosenFoodStation != null)
        {
            agent.SetDestination(chosenFoodStation.position); // Sätter destination till den valda matstationen
        }
        else
        {
            Debug.LogError("Ingen giltig matstation hittades!", this);
        }
    }

    // Returnerar en slumpmässig matstation från listan
    private Transform GetRandomFoodStation()
    {
        int randomIndex = Random.Range(0, foodStations.Length); // Slumpmässig index från listan av matstationer
        return foodStations[randomIndex];
    }

    // Hittar den närmaste kassan baserat på karaktärens nuvarande position
    private Transform FindClosestCashRegister()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var register in cashRegisters) // Loopar igenom kassorna
        {
            if (register == null) continue;

            float distance = Vector3.Distance(transform.position, register.position); // Räkna avstånd till kassan

            if (distance < minDistance) // Om avståndet är närmre än tidigare väljs kassan
            {
                closest = register;
                minDistance = distance;
            }
        }

        return closest; // Returnera den närmaste kassan
    }

    private void Update()
    {
        // Uppdatera animationen baserat på agententens hastighet
        if (agent.velocity.magnitude > 0.05f)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        // Kontrollera om karaktären har nått matstationen
        if (Vector3.Distance(transform.position, chosenFoodStation.position) <= agent.stoppingDistance + 2.3f && !isTakingFood)
        {
            if (!agent.isStopped)  // Kolla om agenten fortfarande rör sig, om inte, börja mat tagning
            {
                StartCoroutine(TakingFood()); // Starta Coroutine för att ta mat
            }
        }

        // Kontrollera om karaktären har nått den närmaste kassan
        if (Vector3.Distance(transform.position, FindClosestCashRegister().position) <= agent.stoppingDistance + 1f && !agent.pathPending)
        {
            StopTimerAndLog(); // Stoppa timern och logga tiden när karaktären når kassan
        }
    }

    // Coroutine som hanterar mat tagning
    private IEnumerator TakingFood()
    {
        isTakingFood = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime); // Pausar till tiden att ta mat

        // Gå vidare till närmaste kassa
        Transform closestCashRegister = FindClosestCashRegister();
        if (closestCashRegister != null)
        {
            agent.isStopped = false;
            agent.SetDestination(closestCashRegister.position); // Destination till närmaste kassa
        }
        else
        {
            Debug.LogError("Ingen kassa hittades!");
        }
    }

    private void StopTimerAndLog()
    {
        if (!hasLoggedTime) // Kontroll om tiden redan har loggats
        {
            float totalTime = Time.time - startTime; // Beräkna den totala tiden för karaktären
            Debug.Log($"Karaktärens totala kötid: {totalTime} sekunder");

            // Delete karaktären efter 3 sekunder
            Destroy(gameObject, 3f); 

            hasLoggedTime = true;
        }
    }
}