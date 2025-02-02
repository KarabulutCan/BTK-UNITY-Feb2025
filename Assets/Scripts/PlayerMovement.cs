using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Input System üzerinden "Move" eylemine bağlı
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Input System üzerinden "Jump" eylemine bağlı
    public void OnJump(InputValue value)
    {
        // Butona basıldıysa ve yerdeysek zıplıyoruz
        if (value.isPressed && isGrounded)
        {
            // Rigidbody2D'ye yukarı doğru ani bir kuvvet uyguluyoruz
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // X eksenindeki hareketi ayarla, Y ekseninde ise mevcut velocity değerini koru
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    // Basit şekilde "Ground" tag'ine sahip bir obje ile çarpıştığımızda yere indiğimizi varsayıyoruz
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // "Ground" tag'inden ayrıldığımızda havada olduğumuzu varsayıyoruz
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
