using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpspeed;
    [SerializeField] private float gravityScale;
    [SerializeField] private float drag;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private StrengthBar strengthBar;
    [SerializeField] PlayerControl controls;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        // Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        controls = new PlayerControl();
        controls.Gameplay.Jump.performed += ctx => Jump();
        //controls.Gameplay.Moving.performed += ctx => horizontalInput = ctx.ReadValue<float>();
        controls.Gameplay.Moving.canceled += ctx => horizontalInput = 0;
        controls.Gameplay.MainMenu.performed += ctx => SceneManager.LoadScene("Main Menu");

    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when moving left-right

        if (horizontalInput < -0.01f && !onWall(Vector2.left)) // If player is moving left, face left
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput > 0.01f && !onWall(Vector2.right)) // If player is moving right, face right
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        // anim.SetBool("grounded", isGrounded());

        //Wall jump logic
        if (onWall(new Vector2(transform.localScale.x, 0)) && !isGrounded() && strengthBar.getTicOne())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = gravityScale;
            // body.drag = drag;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Jump()
    {
        if(isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpspeed);
            // anim.SetTrigger("jump");
           
        }

        else if(onWall(new Vector2(transform.localScale.x, 0)) && !isGrounded() && strengthBar.getTicOne())
        {
            body.gravityScale = gravityScale;

            if(horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * jumpspeed, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else 
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 2, jumpspeed);
            
            strengthBar.reset();
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall(Vector2 xVec)
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, xVec, 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return isGrounded() && !onWall(new Vector2(transform.localScale.x, 0));
    }
   void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
    public void NextScene()
    {
        SceneManager.LoadScene("Main Menu");
    }
}