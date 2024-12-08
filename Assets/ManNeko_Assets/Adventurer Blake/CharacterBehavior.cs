using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehavior : MonoBehaviour
{
    public Transform[] foodStations; // Referenser till matstationer
    public Transform[] cashRegisters; // Referenser till kassor

    private NavMeshAgent agent;
    private Transform chosenFoodStation; // Den slumpmässigt valda matstationen
    private float waitTime = 5f; // Tid vid matstation
    private float startTime; // Tidpunkt när karaktären startade sin resa
    private Animator animator; // Animator för att hantera karaktärens rörelser
    private bool isWaitingAtStation = false; // Kontroll om vi är vid matstationen och väntar
    private bool hasLoggedTime = false; // Flagga för att se om tiden har loggats

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent saknas på karaktären!", this);
            return;
        }

        if (foodStations == null || foodStations.Length == 0)
        {
            Debug.LogError("Inga matstationer är tilldelade!", this);
            return;
        }

        if (cashRegisters == null || cashRegisters.Length == 0)
        {
            Debug.LogError("Inga kassor är tilldelade!", this);
            return;
        }

        startTime = Time.time; // Startar timer

        // Välj en slumpmässig matstation
        chosenFoodStation = GetRandomFoodStation();
        if (chosenFoodStation != null)
        {
            agent.SetDestination(chosenFoodStation.position);
            //animator.SetBool("IsWalking", true); // Sätt animation till gång
        }
        else
        {
            Debug.LogError("Ingen giltig matstation hittades!", this);
        }
    }

    private Transform GetRandomFoodStation()
    {
        int randomIndex = Random.Range(0, foodStations.Length);
        return foodStations[randomIndex];
    }

    private Transform FindClosestCashRegister()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var register in cashRegisters)
        {
            if (register == null) continue;

            float distance = Vector3.Distance(transform.position, register.position);

            if (distance < minDistance)
            {
                closest = register;
                minDistance = distance;
            }
        }

        return closest;
    }

    private void Update()
    {
        // Uppdatera animationen baserat på agentens hastighet
        float velocityMagnitude = agent.velocity.magnitude;

        if (agent.velocity.magnitude > 0.05f)
        {
            animator.SetBool("IsWalking", true); // Växla till gånganimation
        }
        else
        {
            animator.SetBool("IsWalking", false); // Växla till idle-animation
        }

        // Kontrollera om karaktären har nått matstationen och är redo att vänta
        if (Vector3.Distance(transform.position, chosenFoodStation.position) <= agent.stoppingDistance + 2.3f && !isWaitingAtStation)
        {
            if (!agent.isStopped)  // Kolla om agenten fortfarande rör sig
            {
                StartCoroutine(WaitAtStation());
            }
        }

        // Kontrollera om karaktären har nått kassan
        if (Vector3.Distance(transform.position, FindClosestCashRegister().position) <= agent.stoppingDistance + 1f && !agent.pathPending)
        {
            StopTimerAndLog(); // Stoppa timern och logga tiden när karaktären når kassan
        }
    }


    private IEnumerator WaitAtStation()
    {
        isWaitingAtStation = true; // Sätt att vi nu väntar vid stationen
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime); // Vänta vid stationen

        // Gå vidare till närmaste kassa
        Transform closestCashRegister = FindClosestCashRegister();
        if (closestCashRegister != null)
        {
            agent.isStopped = false;
            agent.SetDestination(closestCashRegister.position);
        }
        else
        {
            Debug.LogError("Ingen kassa hittades!");
        }
    }

    private void StopTimerAndLog()
    {
        // Om timern inte redan har loggats för denna karaktär
        if (!hasLoggedTime)
        {
            // Stoppa timern och logga den totala tiden när karaktären når kassan
            float totalTime = Time.time - startTime;
            Debug.Log($"Karaktärens totala kötid: {totalTime} sekunder");

            // Förstör karaktären efter 3 sekunder
            Destroy(gameObject, 3f);

            // Sätt flaggan till true för att indikera att tiden har loggats
            hasLoggedTime = true;
        }
    }
}