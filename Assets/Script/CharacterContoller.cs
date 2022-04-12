using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterContoller : MonoBehaviour
{
    public Tilemap tilemap;

    [SerializeField]
    private float MOVE_SPEED = 5.0f;
    [SerializeField]
    private float DASH_SPEED = 15.0f;
    [SerializeField]
    private float GRAVITY_VALUE = 5.0f;
    [SerializeField]
    private float RAYCAST_RANGE = 0.5f;
    [SerializeField]
    private float JUMP_VALUE = 10.0f;

    private float FORCE_DOWN = 2.0f;

    CameraController cameraController;

    private SpriteRenderer renderer;
    private Animator animator;


    private bool isPlatformOrGround = false;
    private bool isGround = false;
    private bool isPlatform = false;
    private bool blockLeft = false;
    private bool blockRight = false;
    private bool isJumping = false;
    private bool isMoveCameraLeft = false;
    private bool isMoveCameraRight = false;
    private bool isMoveCameraUp = false;
    private bool isMoveCameraDown = false;
    private bool isAttack = false;
    private float jumpingPower = 0;
    private bool isForceDown = false;
    private bool isDash = false;
    private bool isMoveDown = false;

    private float positionY = 0.0f;
    void Awake()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        if(renderer == null)
        {
            Debug.LogError("CharacterCotroller.Awake() : renderer is Null");
        }
        animator = GetComponentInChildren<Animator>();
        if(animator == null)
        {
            Debug.LogError("CharacterCotroller.Awake() : animator is Null");
        }
        cameraController = Camera.main.GetComponent<CameraController>();
        if(cameraController == null)
        {
            Debug.LogError("CharacterCotroller.Awake() : cameraController is Null");
        }
    }

    void FixedUpdate()
    {
        CheckGroundAndPlatform();
        CheckHorizontalColliding();
        CheckCamereMove();
    }
    // Update is called once per frame
    void Update()
    {
        CharacterMoveHorizontal();
        CharacterDash();
        CharacterJump();
        CharacterAttack();
        CharacterDownPlatform();
        
        GravityEffect();
    }
    void CharacterMoveHorizontal()
    {
        float move = Input.GetAxisRaw("Horizontal");
        
        if(isDash == false)
        {
            if(move > 0)
            {
                renderer.flipX = false;

                if(blockRight == false)
                {
                    float moveSpeed =  move * Time.deltaTime * MOVE_SPEED;
                    transform.position = transform.position + new Vector3(moveSpeed, 0, 0 );
                    
                    if( isMoveCameraRight) cameraController.MoveCameraHorizontal(moveSpeed);
                }
            }
            else if ( move < 0 )
            {
                renderer.flipX = true;
                if(blockLeft == false)
                {
                    float moveSpeed =  move * Time.deltaTime * MOVE_SPEED;
                    transform.position = transform.position + new Vector3(moveSpeed, 0, 0 );
                    
                    if( isMoveCameraLeft) cameraController.MoveCameraHorizontal(moveSpeed);
                }
            }
            
            animator.SetBool("isWalk", move != 0);
        }
    }
    void CharacterDash()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && isPlatformOrGround == true)
        {
            isDash = true;
            StartCoroutine(StopDash());
        }

        if(isDash == true)
        {
            animator.SetBool("isDash",true);
            if( renderer.flipX.Equals(true) )
            {
                if(blockLeft == false)
                {
                    float moveSpeed = -Time.deltaTime * DASH_SPEED;
                    transform.position = transform.position + new Vector3(moveSpeed, 0, 0 );
                    
                    if( isMoveCameraLeft) cameraController.MoveCameraHorizontal(moveSpeed);
                }
            }
            else
            {
                if(blockRight == false)
                {
                    float moveSpeed = Time.deltaTime  * DASH_SPEED;
                    transform.position = transform.position + new Vector3(moveSpeed, 0, 0 );
                    
                    if( isMoveCameraRight) cameraController.MoveCameraHorizontal(moveSpeed);
                }
            }
        }
    }

    void CharacterJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isJumping == false && isPlatformOrGround == true && isAttack == false && isDash == false)
        {
            animator.SetBool("isFall", false);
            animator.SetBool("isJumping", true);
            animator.SetBool("isGround", false);
            animator.SetBool("isAttack",false);
            
             isJumping = true;
             isPlatformOrGround = false;
             isGround = false;
             isPlatform = false;
             jumpingPower = JUMP_VALUE;
             isForceDown = false;
        }

       if(isJumping)
       {
            float moveFactor = jumpingPower * Time.deltaTime * 1.2f;
            if(isMoveCameraUp == true)
            {
                cameraController.MoveCameraVertical(moveFactor + 0.015f);
            }
            transform.position = transform.position + new Vector3(0, moveFactor, 0 );
            
            jumpingPower -= GRAVITY_VALUE * Time.deltaTime * 2f;

            if(jumpingPower <= GRAVITY_VALUE)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFall", true);
                isJumping = false;
            }
       }
    }
    void CharacterAttack()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && isDash == false)
        {
            if(isPlatformOrGround == true && isJumping == false)
            {
                animator.SetBool("isAttack",true);
                isAttack = true;
            }
        }
        if(Input.GetKeyUp(KeyCode.LeftControl)&& isDash == false)
        {
            animator.SetBool("isAttack",false);
            isAttack = false;
        }
    }

    void CharacterDownPlatform()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) && isDash == false)
       {
           if(isPlatformOrGround == true && isPlatform == true)
           {
                isMoveDown = true;
                animator.SetBool("isJumping", false);
                animator.SetBool("isFall", true);
                isJumping = false;

                StartCoroutine(StopDownPlatform());
           }
       }
    }

    void GravityEffect()
    {
            if(isPlatformOrGround == false )
            {
                float moveValue = isForceDown ? -GRAVITY_VALUE * Time.deltaTime * FORCE_DOWN : -GRAVITY_VALUE * Time.deltaTime;
                transform.position = transform.position + new Vector3(0, moveValue , 0 );
                if(isMoveCameraDown)
                {
                    cameraController.MoveCameraVertical(moveValue);
                }
            }
    }
    void CheckGroundAndPlatform()
    {
        if(isMoveDown == true)
        {
            isPlatformOrGround = false;
            isGround = false;
            isPlatform = false;
            isJumping = true;

            return;
        }
        RaycastHit2D[] hitDown = Physics2D.RaycastAll(new Vector2(transform.position.x,transform.position.y - RAYCAST_RANGE),
                                                      Vector2.down,0.1f);
        isPlatformOrGround = false;
        for(int i = 0 ; i< hitDown.Length; i++)
        {
            if( hitDown[i].transform.gameObject.layer == LayerMask.NameToLayer("Ground") )
            {
                positionY = hitDown[i].collider.bounds.max.y + 0.5f;
                isGround = true;
                isPlatformOrGround = true;
                break;
            }
            if( hitDown[i].transform.gameObject.layer == LayerMask.NameToLayer("Platform") )
            {
                positionY = hitDown[i].collider.bounds.max.y + 0.5f;
                isPlatform = true;
                isPlatformOrGround = true;
                break;
            }
        }
        if( isJumping == false &&
            isPlatformOrGround == true) 
        {
            isJumping = false;
            transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
        }
        else
        {
            isGround = false;
            isPlatform = false;
        }
        animator.SetBool("isGround", isPlatformOrGround);
    }
    void CheckHorizontalColliding()
    {
        RaycastHit2D[] hitsLeft =  Physics2D.RaycastAll(transform.position,Vector2.left ,RAYCAST_RANGE);
        RaycastHit2D[] hitsRight =  Physics2D.RaycastAll(transform.position,Vector2.right ,RAYCAST_RANGE);

        bool isHitLeft = false;
        bool isHitRight = false;
        for(int i = 0; i < hitsLeft.Length; i++)
        {
            if(hitsLeft[i].transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                isHitLeft = true;
                break;
            }
        }
        blockLeft = isHitLeft;

        for(int i = 0; i < hitsRight.Length; i++)
        {
            if(hitsRight[i].transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                isHitRight = true;
                break;
            }
        }
        blockRight = isHitRight;
    }
    void CheckCamereMove()
    {
        RaycastHit2D[] ray_CameraController = Physics2D.RaycastAll(transform.position,Vector2.up,RAYCAST_RANGE);
        

        isMoveCameraLeft = false;
        isMoveCameraRight = false;
        isMoveCameraUp = false;
        isMoveCameraDown = false;
        for(int i = 0; i < ray_CameraController.Length; i++)
        {
            if(ray_CameraController[i].transform.gameObject.layer == LayerMask.NameToLayer("CameraController"))
            {
                if( ray_CameraController[i].transform.name == "Left")
                    isMoveCameraLeft = true;
                else if ( ray_CameraController[i].transform.name == "Right")
                    isMoveCameraRight = true;
                else if ( ray_CameraController[i].transform.name == "Up")
                    isMoveCameraUp = true;
                else if ( ray_CameraController[i].transform.name == "Down")
                    isMoveCameraDown = true;
            }
        }
    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(0.15f);
        isDash = false;
        animator.SetBool("isDash",false);
    }
    IEnumerator StopDownPlatform()
    {
        yield return new WaitForSeconds(0.5f);
        isMoveDown = false;
    }
}
