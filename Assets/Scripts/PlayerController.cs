using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テスト的なキャラクターコントロール
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("移動速度(m/sec)")]
    public float movementSpeed = 5.0f;

    public float rotateSpeed = 0.01f;

    protected Vector3                   moveVec = Vector3.zero;
    protected Vector3                   inputVelocity = Vector3.zero;
    protected Animator                  animator = null;
    protected PlayerCameraController    camControl = null;
    protected Rigidbody                 rigidBody = null;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        camControl = Camera.main.GetComponent<PlayerCameraController>();
        rigidBody  = GetComponent<Rigidbody>();
        if(rigidBody)
        {
            rigidBody.velocity = Vector3.zero;
        }

        var inputController = GetComponent<InputController>();
        if(inputController)
        {
            inputController.RegisterDelegateWithControllerInfo(UpdateFromInputController);
        }
    }

    void OnDestroy()
    {
        var inputController = GetComponent<InputController>();
        if (inputController)
        {
            inputController.UnRegisterDelegateWithControllerInfo(UpdateFromInputController);
        }
    }

    private void FixedUpdate()
    {
        if (inputVelocity.magnitude > 0.0f)
        {
            rigidBody.velocity = new Vector3(inputVelocity.x, rigidBody.velocity.y, inputVelocity.z);
        }
        else
        {
            rigidBody.velocity = new Vector3(0.0f, rigidBody.velocity.y, 0.0f);
        }

        if (rigidBody.velocity.y > 0.0f)
        {
            Debug.Log(rigidBody.velocity.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputVelocity.magnitude > 0.0f)
        {
            // 今の回転と目標とする回転までを補完
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                         Quaternion.LookRotation(inputVelocity),
                                                         rotateSpeed);
        }
    }

    void UpdateFromInputController(InputController.InputInfo inputInfo)
    {
        if (!animator) return;
        if(inputInfo.currentBit.Get(0))
        {
            animator.SetTrigger("Attack");
        }

        var inputVec = new Vector3(inputInfo.LStick.x, 0.0f, inputInfo.LStick.y);
        moveVec = inputVec * movementSpeed;
        animator.SetFloat("inputVec", inputVec.magnitude);
    }

    public void UpdateMovement(AnimatorStateInfo stateInfo)
    {
        if(stateInfo.IsName("Run"))
        {
            inputVelocity = camControl.camTrans.hRotation * moveVec;
        }
    }
}
