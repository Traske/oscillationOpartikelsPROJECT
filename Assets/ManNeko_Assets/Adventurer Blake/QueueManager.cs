using System.Collections;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public Transform[] queuePositions; // Lista med köpositioner framför kassorna
    private int currentPosition = 0;   // Håller reda på vilken köposition som är ledig

    // När karaktären närmar sig kassan, ska den ställa sig i en ledig köposition
    public Transform GetNextQueuePosition()
    {
        if (queuePositions.Length == 0)
        {
            Debug.LogWarning("Ingen köposition tillgänglig!");
            return null;
        }

        Transform position = queuePositions[currentPosition];
        currentPosition = (currentPosition + 1) % queuePositions.Length; // Nästa position i kön
        return position;
    }
}
