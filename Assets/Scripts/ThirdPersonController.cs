using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField]private CharacterController controller;
    private Animator anim;
    [SerializeField]private Transform cam;
    public Transform LookAtTransform;
    private Vector3 playerVelocity;
    public Transform groundSensor;
    public LayerMask ground;
    public float sensorRadius = 0.1f;
    [Header("Fisicas")]
    public float speed = 5f;
    public float jumpForce = 20f;
    public float jumpHeight = 1f;
    private float gravity = -9.81f;
    [Header("Sensor Suelo")]
    public bool isGrounded;
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    public GameObject[] cameras;

    public LayerMask rayLayer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        //Cursor.lockState = CursorLockMode.Locked;
    }
   
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();
        MovementTPS();
        //MovementTPS2();

        Jump();

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 20f, rayLayer))
        {
            Vector3 hitPosition = hit.point;
            
            float hitDistance = hit.distance;

            string hitName = hit.transform.name;

            //Animator hitAnimator = hit.transform.GameObject.GetComponent<Animator>();

            //hit.transform.GameObject.GetComponent<ScriptRandom>().FunctionRandom();

            Debug.DrawRay(transform.position, transform.forward * 20f, Color.blue);

            Debug.Log("Posicion impacto:" + hitPosition + "Distancia impacto:" + hitDistance + "Nombre objeto:" + hitName);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit2;
            if(Physics.Raycast(ray, out hit2))
            {
                Debug.Log(hit2.point);
                transform.position = new Vector3(hit2.point.x, transform.position.y, hit2.point.z);
            } 
        }

        
    }

    void Movement()
    {
         Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if(move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }

    }
    //Movimiento TPS con free look camara
    void MovementTPS()
    {
        float z = Input.GetAxisRaw("Vertical");
        anim.SetFloat("VelZ", z);
        float x = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("VelX", x);
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if(move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }

    }
    //Movimiento TPS con camara virutal
     void MovementTPS2()
    {
         Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

         xAxis.Update(Time.deltaTime);
         yAxis.Update(Time.deltaTime);

         transform.rotation = Quaternion.Euler(0, xAxis.Value, 0);

         LookAtTransform.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, LookAtTransform.eulerAngles.z);

         if(Input.GetButton("Fire2"))
         {
             cameras[0].SetActive(false);

             cameras[1].SetActive(true);
         }

         else
         {
            cameras[0].SetActive(true);

            cameras[1].SetActive(false);
         }

        if(move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }

    }

    

    void Jump()
    {
        //isGrounded = controller.isGrounded;
        //isGrounded = Physics.CheckSphere(groundSensor.position, sensorRadius, ground);
        //isGrounded = Physics.Raycast(groundSensor.position, Vector3.down, sensorRadius, ground);
        if(Physics.Raycast(groundSensor.position, Vector3.down, sensorRadius, ground))
        {
            isGrounded = true;
            Debug.DrawRay(groundSensor.position, Vector3.down * sensorRadius, Color.blue);
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(groundSensor.position, Vector3.down * sensorRadius, Color.red);
        }

        anim.SetBool("Jump",!isGrounded);
               
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //playerVelocity.y += jumpForce;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 20);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundSensor.position, sensorRadius);
    }
        
    
}
