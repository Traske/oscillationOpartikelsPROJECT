using UnityEngine;
using System.Collections;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject characterPrefab; // Prefab för karaktär
    public Transform spawnPoint; // Plats där karaktärerna skapas
    public int characterCount = 10; // Antal karaktärer att skapa
    public float spawnInterval = 2f; // Tid mellan spawns

    private void Start()
    {
        StartCoroutine(SpawnCharacters());
    }

    private IEnumerator SpawnCharacters()
    {
        for (int i = 0; i < characterCount; i++)
        {
            Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
