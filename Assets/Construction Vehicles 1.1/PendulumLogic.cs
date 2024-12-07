using UnityEngine;

public class PendulumLogic : MonoBehaviour
{
    [Header("Referenser")]
    public Transform HookPoint;       // Punkten där vajern är fäst
    public Transform Sphere;          // Klotet som fungerar som en rivningskula
    public LineRenderer Line;         // För att rita vajern
    private Rigidbody sphereRigidbody; // Rigidbody för klotet

    [Header("Pendelinställningar")]
    [SerializeField] private float wireLength = 5f;  // Längden på vajern
    [SerializeField] private float adjustSpeed = 2f; // Hur snabbt vajern justeras
    [SerializeField] private float gravity = 12f;  // Gravitation
    [SerializeField] private float damping = 0.95f;  // Dämpning av pendelrörelsen
    [SerializeField] private float tensionForce = 80f; // Kraft som håller klotet i vajern

    private Vector3 velocity; // Hastigheten för pendeln
    private Vector3 previousHookPosition; // För att räkna ut HookPoint-rörelse

    void Start()
    {
        // Lägg till en Rigidbody till klotet om det inte redan finns
        if (Sphere.TryGetComponent(out Rigidbody rb))
        {
            sphereRigidbody = rb;
        }
        else
        {
            sphereRigidbody = Sphere.gameObject.AddComponent<Rigidbody>();
        }

        // Konfigurera Rigidbody
        sphereRigidbody.useGravity = false; // Gravitation hanteras manuellt
        sphereRigidbody.constraints = RigidbodyConstraints.FreezeRotation; // Förhindra rotation

        previousHookPosition = HookPoint.position; // Initiera föregående position
    }

    void Update()
    {
        // Räkna ut HookPoint-rörelse
        Vector3 hookVelocity = (HookPoint.position - previousHookPosition) / Time.deltaTime;
        previousHookPosition = HookPoint.position;

        // Beräkna riktning från HookPoint till klot
        Vector3 direction = Sphere.position - HookPoint.position;
        float currentLength = direction.magnitude;
        direction.Normalize();

        // Spänning i vajern
        Vector3 tension = Vector3.zero;
        if (currentLength > wireLength)
        {
            tension = -direction * (currentLength - wireLength) * tensionForce;
        }

        // Gravitationens kraft
        Vector3 gravityForce = Vector3.down * gravity;

        // Uppdatera hastigheten (inklusive HookPoint-rörelse)
        velocity += (tension + gravityForce) * Time.deltaTime;
        velocity *= damping; // Applicera dämpning
        velocity += hookVelocity * Time.deltaTime;

        // Uppdatera positionen
        sphereRigidbody.linearVelocity = velocity;

        // Begränsa klotet till vajerns längd
        if (currentLength > wireLength)
        {
            Sphere.position = HookPoint.position + direction * wireLength;
            velocity = Vector3.ProjectOnPlane(velocity, direction); // Justera hastighet
        }

        // Rita vajern med LineRenderer
        if (Line != null)
        {
            Line.SetPosition(0, HookPoint.position);
            Line.SetPosition(1, Sphere.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Kontrollera om rivningskulan träffar något
        Rigidbody otherRigidbody = collision.rigidbody;

        if (otherRigidbody != null)
        {
            // Applicera en impuls på det objekt som träffas av rivningskulan
            Vector3 impactForce = velocity * sphereRigidbody.mass;
            otherRigidbody.AddForce(impactForce, ForceMode.Impulse);

            Debug.Log($"Rivningskulan träffade: {collision.gameObject.name} med kraft {impactForce.magnitude}");
        }
    }

    // Publik metod för att justera vajerlängden
    public void AdjustWireLength(float delta)
    {
        wireLength = Mathf.Max(1f, wireLength + delta * adjustSpeed);
    }

    // Publika egenskaper för justering via Unity
    public float WireLength
    {
        get => wireLength;
        set => wireLength = Mathf.Max(1f, value);
    }

    public float AdjustSpeed
    {
        get => adjustSpeed;
        set => adjustSpeed = Mathf.Max(0.1f, value);
    }

    public float Gravity
    {
        get => gravity;
        set => gravity = Mathf.Clamp(value, 0f, 20f);
    }

    public float Damping
    {
        get => damping;
        set => damping = Mathf.Clamp(value, 0.5f, 1f);
    }

    public float TensionForce
    {
        get => tensionForce;
        set => tensionForce = Mathf.Max(0f, value);
    }
}