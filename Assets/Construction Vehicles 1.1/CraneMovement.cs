using UnityEngine;

public class CraneMovement : MonoBehaviour
{
    public float Speed;
    public float RotSpeed;
    public Transform HookPoint;
    public PendulumLogic Pendulum;

    void Update()
    {
        // Rörelse av kranen
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Speed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(-Speed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -RotSpeed * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, RotSpeed * Time.deltaTime, 0);
        }

        // Vajerns längd
        if (Input.GetKey(KeyCode.Q))
        {
            Pendulum.AdjustWireLength(Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            Pendulum.AdjustWireLength(-Time.deltaTime);
        }
    }
}
