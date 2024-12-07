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
            animator.SetBool("IsWalking", true); // Sätt animation till gång
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodStation") && other.transform == chosenFoodStation)
        {
            StartCoroutine(WaitAtStation());
        }
        else if (other.CompareTag("CashRegister"))
        {
            StopTimer();
        }
    }

    private IEnumerator WaitAtStation()
    {
        agent.isStopped = true; // Stoppar karaktären vid stationen

        // Vänta tills agenten faktiskt har nått stationen
        while (Vector3.Distance(agent.transform.position, chosenFoodStation.position) > agent.stoppingDistance)
        {
            yield return null; // Vänta tills karaktären är nära stationen
        }

        // Vänta i 3 sekunder vid matstationen
        yield return new WaitForSeconds(waitTime);

        // Återupptar rörelsen
        agent.isStopped = false;

        // Gå vidare till närmaste kassa
        Transform closestCashRegister = FindClosestCashRegister();
        if (closestCashRegister != null)
        {
            agent.SetDestination(closestCashRegister.position);
            animator.SetBool("IsWalking", true); // Sätt animation till gång igen
        }
        else
        {
            Debug.LogError("Ingen kassa hittades!", this);
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
