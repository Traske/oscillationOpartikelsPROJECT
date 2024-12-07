using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehavior : MonoBehaviour
{
    public Transform[] foodStations; // Referenser till matstationer
    public Transform[] cashRegisters; // Referenser till kassor

    private NavMeshAgent agent;
    private Transform chosenFoodStation; // Den slumpmässigt valda matstationen
    private float waitTime = 3f; // Tid vid matstation
    private float startTime; // Tidpunkt när karaktären startade sin resa
    private Animator animator; // Animator för att hantera karaktärens rörelser

    private float logTimer = 0f; // Timer för att logga avstånd
    private float logInterval = 1f; // Intervallet för loggning (i sekunder)

    private bool isWaitingAtStation = false; // Kontroll om vi är vid matstationen och väntar

    private void Start()
    {
        Debug.Log("Start-metoden körs!");
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
            animator.SetBool("IsWalking", true); // Sätt animation till gång
            Debug.Log("Finding food");
            Debug.Log("Agentens hastighet är: " + agent.speed);
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
            Debug.Log($"Kassa position: {register.position}, Avstånd: {distance}");

            if (distance < minDistance)
            {
                closest = register;
                minDistance = distance;
                Debug.Log($"Närmaste kassa hittills: {closest.position}, Avstånd: {minDistance}");
            }
        }

        return closest;
    }

    private void Update()
    {
        // Uppdatera logg-timer
        logTimer += Time.deltaTime;

        // Logga distanserna varje sekund
        if (logTimer >= logInterval)
        {
            // Nollställ timer
            logTimer = 0f;

            // Logga avstånd till matstationen
            Debug.Log("Avstånd till matstationen: " + Vector3.Distance(transform.position, chosenFoodStation.position));

            // Logga kvarvarande avstånd till destinationen
            Debug.Log("Agentens kvarvarande avstånd: " + agent.remainingDistance);
        }

        // Kontrollera om karaktären har nått matstationen och är redo att vänta
        if (Vector3.Distance(transform.position, chosenFoodStation.position) <= agent.stoppingDistance + 2.6f && !isWaitingAtStation)
        {
            if (!agent.isStopped)  // Kolla om agenten fortfarande rör sig
            {
                Debug.Log("Agenten har nått matstationen");
                StartCoroutine(WaitAtStation());
            }
        }
    }

    private IEnumerator WaitAtStation()
    {
        isWaitingAtStation = true; // Sätt att vi nu väntar vid stationen
        agent.isStopped = true;
        Debug.Log("Väntar vid matstation...");

        animator.SetBool("IsWalking", false); // Växla till stillastående animation
        Debug.Log("Karaktären stannar vid matstationen.");
        yield return new WaitForSeconds(waitTime); // Vänta vid stationen

        // Gå vidare till närmaste kassa
        Transform closestCashRegister = FindClosestCashRegister();
        if (closestCashRegister != null)
        {
            Debug.Log($"Karaktären rör sig mot kassan: {closestCashRegister.position}");
            agent.isStopped = false;
            agent.SetDestination(closestCashRegister.position);
            Debug.Log($"Agentens hastighet: {agent.speed}");
            animator.SetBool("IsWalking", true); // Växla till gånganimation
        }
        else
        {
            Debug.LogError("Ingen kassa hittades!");
        }
    }

    private void StopTimer()
    {
        float totalTime = Time.time - startTime;
        Debug.Log($"Karaktärens totala kötidsresa: {totalTime} sekunder");
        animator.SetBool("IsWalking", false); // Sätt animation till stillastående när vi är framme
        Destroy(gameObject, 1f); // Förstör karaktären efter en sekund
    }
}
