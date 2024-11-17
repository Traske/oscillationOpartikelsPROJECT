using UnityEngine;

public class WaveSimulation : MonoBehaviour
{
    // Variabler för att ställa in vågornas parametrar
    public float amplitude;     // Amplitud (höjd på vågorna)
    public float frequency;     // Frekvens (hur snabbt vågorna rör sig)
    public float wavelength;    // Våglängd (avstånd mellan två toppar)
    public float phase;         // Fasvinkel (var vågen börjar)

    // Referens till meshen
    private Mesh mesh;
    private Vector3[] vertices;

    // Uppdatera vågorna varje frame
    void Start()
    {
        // Hämta meshen från Planen
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    void Update()
    {
        // Uppdatera vertexpositionerna baserat på harmoniska svängningen
        AnimateWaves();
    }

    void AnimateWaves()
    {
        // Hämta nuvarande tid
        float time = Time.time;

        // Gå igenom varje vertex och ändra y-koordinat
        for (int i = 0; i < vertices.Length; i++)
        {
            // Hämta x-positionen för vertexen
            float x = vertices[i].x;

            // Beräkna nya y-positionen med formeln för enkel harmonisk svängning
            float y = amplitude * Mathf.Sin((x - 2 * Mathf.PI * frequency * time) / wavelength + phase);

            // Uppdatera vertexens y-position
            vertices[i].y = y;
        }

        // Uppdatera meshens vertexdata för att visa ändringarna
        mesh.vertices = vertices;
        mesh.RecalculateNormals();  // För att få rätt belysningseffekter efter ändringarna
    }
}