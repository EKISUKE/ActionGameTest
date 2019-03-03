using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テスト的なキャラクターコントロール
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputController))]
public class CharacterController : MonoBehaviour
{
    [Tooltip("移動速度(m/sec)")]
    public float movementSpeed = 5.0f;

    public float rotateSpeed = 0.01f;
    public Camera                       camera = null;

    protected Vector3                   moveVec = Vector3.zero;
    protected Animator                  animator = null;
    protected PlayerCameraController    camControl = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        camControl = camera.GetComponent<PlayerCameraController>();
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
    
    // Update is called once per frame
    void Update()
    {
        if (moveVec.magnitude > 0.0f) {
            var velocity = camControl.camTrans.hRotation * moveVec;
            this.transform.rotation = Quaternion.LookRotation(velocity);
            this.transform.Translate(velocity, Space.World);
        }
    }

    void UpdateFromInputController(InputController.InputInfo inputInfo)
    {
        if (!animator) return;
        if (inputInfo.currentBit.Get(0))
        {
            animator.SetTrigger("Attack");
        }

        var inputVec = new Vector3(inputInfo.LStick.x, 0.0f, inputInfo.LStick.y);
        moveVec = inputVec * (movementSpeed * Time.deltaTime);
    }
}
