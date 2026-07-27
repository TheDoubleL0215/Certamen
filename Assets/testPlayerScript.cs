using System.Collections;
using UnityEngine;

public class testPlayerScript : MonoBehaviour
{
    public float novekedesSebesseg = 20f;
    public float smoothTime = 1.5f;
    private float currentVelocity = 0.0f;
    private float maxFlyHeight = 90f;
    private float currentSpeed;
    private float smoothSpeed;
    public CharacterController controller;
    public Rigidbody rb;
    public float walkSpeed = 20f;
    public float flySpeed = 50f;

    private bool isFlying = false;
    
    public float gravity = 30f;  // A gravitáció értéke

    private bool isFloating = false;  // Változó az ugrás és repülés állapotának tárolására

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.TransformDirection(new Vector3(x, 0, z));

        if (!isFlying)  // Csak akkor mozogjon, ha nem repül
        {
            controller.Move(move * walkSpeed * Time.deltaTime);
        }

        // Repülés ellenőrzése Raycast segítségével
        bool grounded = controller.isGrounded;

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Ugrás és repülés közötti váltás
            isFlying = !isFlying;

            if (isFlying)
            {
                // Repülési üzemmód bekapcsolása
                Debug.Log("Repülés bekapcsolva");
            }
            else
            {
                // Ugrási üzemmód bekapcsolása
                Debug.Log("Ugrás bekapcsolva");
            }
        }

        if (isFlying)
        {
            // Repülési logika
            // ...
            controller.Move(move * walkSpeed * Time.deltaTime);

            // Példa: Repülési sebesség beállítása
            if (Input.GetKey(KeyCode.Space)){
                move.y += 1 * flySpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftControl)){
                move.y -= 1 * flySpeed * Time.deltaTime;
            }
        }
        else
        {
            // Ugrási logika
            // ...

            // Példa: Ugrás kezelése
            if (Input.GetButtonDown("Jump") && grounded)
            {
                rb.AddForce(transform.up * 3, ForceMode.Impulse);
            }
        }

        if (isFloating)
        {
            move.y -= gravity * Time.deltaTime;

            // Ha a karakter a talajon van vagy elértük a maximális lebegési magasságot
            if (grounded || controller.transform.position.y <= maxFlyHeight)
            {
                isFloating = false;
            }
        }

        controller.Move(move * Time.deltaTime);

        controller.Move(move * Time.deltaTime);

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = 80;
            smoothSpeed = Mathf.SmoothDamp(walkSpeed, currentSpeed, ref currentVelocity, smoothTime);
        }
        else
        {
            currentSpeed = 20;
            smoothSpeed = Mathf.SmoothDamp(walkSpeed, currentSpeed, ref currentVelocity, smoothTime);
        }

        walkSpeed = smoothSpeed;
    }
}
