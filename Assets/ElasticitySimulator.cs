using UnityEngine;

public class ElasticitySimulator : MonoBehaviour
{
    public float springConstant = 10f; // Fjäderkonstant
    public float damping = 0.9f; // Dämpning för stabilitet
    public float vertexMass = 0.1f; // Massa per hörnpunkt
    public float gravity = -9.82f; // Tyngdkraft

    private Mesh mesh;
    private Vector3[] originalVertices; // Ursprunglig position för varje vertex
    private Vector3[] deformedVertices; // Deformade positioner
    private Vector3[] velocities; // Hastighetsvektorer för varje vertex
    private int width, height; // Plan meshens bredd och höjd

    void Start()
    {
        // Få tillgång till mesh-data
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        // Skapa en kopia av de ursprungliga vertexpositionerna för deformationen
        deformedVertices = (Vector3[])originalVertices.Clone();
        // Initiera hastighetsvektorerna för varje vertex
        velocities = new Vector3[deformedVertices.Length];

        // Bestäm bredden och höjden
        width = Mathf.RoundToInt(Mathf.Sqrt(originalVertices.Length));
        height = width; // (kvadrat)
    }

    void Update()
    {   
        // Loopar genom varje vertex för att beräkna krafterna och uppdatera positionerna
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            if (IsEdgeVertex(i)) continue; // Hoppa över kanter

            Vector3 force = Vector3.zero;

            // Beräkna fjäderkrafter från grannar
            foreach (int neighbor in GetNeighbors(i))
            {
                // Beräkna riktning från den aktuella vertexen till en granne
                Vector3 direction = deformedVertices[neighbor] - deformedVertices[i];
                // Beräkna hur mycket vertexen har förskjutit sig i förhållande till sin ursprungliga position
                float displacement = direction.magnitude - (originalVertices[neighbor] - originalVertices[i]).magnitude;
                // Lägg till fjäderkraften
                force += direction.normalized * (springConstant * displacement);
            }

            // Applicera tyngdkraften
            force += Vector3.up * gravity * vertexMass;

            // Beräkna acceleration och uppdatera hastighet
            Vector3 acceleration = force / vertexMass;
            velocities[i] += acceleration * Time.deltaTime;

            // Dämpning
            velocities[i] *= damping;

            // Uppdatera positioner
            deformedVertices[i] += velocities[i] * Time.deltaTime;
        }

        // Uppdatera meshen
        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
    }

    bool IsEdgeVertex(int index)
    {
        // Kontrollera om vertex är en kant
        int x = index % width;
        int y = index / width;

        return (x == 0 || x == width - 1 || y == 0 || y == height - 1);
    }

    int[] GetNeighbors(int index)
    {
        // Hitta grannarnas index
        int x = index % width;
        int y = index / width;

        System.Collections.Generic.List<int> neighbors = new System.Collections.Generic.List<int>();

        if (x > 0) neighbors.Add(index - 1); // Vänster granne
        if (x < width - 1) neighbors.Add(index + 1); // Höger
        if (y > 0) neighbors.Add(index - width); // Upp
        if (y < height - 1) neighbors.Add(index + width); // Ner

        return neighbors.ToArray();
    }
}