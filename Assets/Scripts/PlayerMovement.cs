using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform rifle;
    [SerializeField] public int MaxHealth;
    public TextMeshProUGUI dyingText;
    public Slider slider;
    public int currentHealth;
    private CharacterController controller;
    private PlayerControls controls;
    float RifleRoty;

    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    // Geet Global Vollume
    public Volume globalVolume;
    private Vignette vignette;


    void Awake()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0f;
        }
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();
        controls.Enable();
        RifleRoty = rifle.rotation.y;
        currentHealth = MaxHealth;
    }

    void Update()
    {
        Vector2 moveInput = controls.PlayerMovement.Move.ReadValue<Vector2>();

        Vector3 move = transform.right * moveInput.x +
                    transform.forward * moveInput.y;

        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // keep grounded
        }

        // Jump input
        if (controller.isGrounded && controls.PlayerMovement.Jump.triggered)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animations
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Horizontal", moveInput.x);

        // Vignette health effect
        float healthPercent = Mathf.Clamp01((float)currentHealth / MaxHealth);
        vignette.intensity.value = (1f - healthPercent) * 0.45f;
        if (slider != null)
        {
            slider.value = healthPercent;
        }
    }

    void LateUpdate()
    {
        // Sync rifle pitch with camera pitch
        if (rifle != null && cameraController != null)
        {
            Quaternion camRotation = cameraController.transform.rotation;
            Vector3 rifleEuler = rifle.rotation.eulerAngles;
            rifleEuler.z = camRotation.eulerAngles.x;
            rifle.rotation = Quaternion.Euler(rifleEuler);
        }
    }
    public void Die()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("You Died");
            controls.Disable();
            dyingText.alpha = Mathf.Lerp(0,1,1.4f);

        }

    }


}