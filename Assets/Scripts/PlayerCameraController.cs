using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
/// <summary>
/// プレイヤーの周りを周回するカメラ
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    public class CameraTransform
    {
        public Vector3 eulerAngle;
        public Quaternion vRotation;
        public Quaternion hRotation;

        public CameraTransform(Transform transform)
        {
            eulerAngle = transform.rotation.eulerAngles;
            vRotation = Quaternion.Euler(eulerAngle.x, 0.0f, 0.0f);
            hRotation = Quaternion.Euler(0.0f, eulerAngle.y, 0.0f);
        }

    };

    public CameraTransform camTrans = null;

    [Tooltip("注視対象")]
    public Transform       forcusTrans = null;

    [Tooltip("注視点からの距離")]
    public float           length = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        var inputController = forcusTrans.GetComponent<InputController>();
        if (inputController)
        {
            inputController.RegisterDelegateWithControllerInfo(UpdateFromInputController);
        }

        camTrans = new CameraTransform(this.transform);
    }

    void OnDestroy()
    {
        var inputController = forcusTrans.GetComponent<InputController>();
        if (inputController)
        {
            inputController.UnRegisterDelegateWithControllerInfo(UpdateFromInputController);
        }
    }

    Vector3 CalclateTargetPos(Vector3 forcusPos)
    {
        // プレイヤーの位置からカメラの位置を逆算
        var targetPos = forcusPos + (this.transform.rotation * Vector3.back) * length;
        return targetPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        camTrans.hRotation = Quaternion.Euler(0.0f,                  camTrans.eulerAngle.y, 0.0f);
        camTrans.vRotation = Quaternion.Euler(camTrans.eulerAngle.x, 0.0f,                  0.0f);

        this.transform.rotation = camTrans.hRotation * camTrans.vRotation;
        var forcusPos = forcusTrans.position + Vector3.up;
        this.transform.position = CalclateTargetPos(forcusPos);
    }

    void UpdateFromInputController(InputController.InputInfo inputInfo)
    {
        var factor = 100.0f;
        if (inputInfo.RStick.magnitude > 0.0f)
        {
            camTrans.eulerAngle.y += inputInfo.RStick.x * factor * Time.deltaTime;
            camTrans.eulerAngle.x += inputInfo.RStick.y * factor * Time.deltaTime;
        }
        camTrans.eulerAngle.x = Mathf.Repeat(camTrans.eulerAngle.x + 360.0f, 360.0f);
        camTrans.eulerAngle.y = Mathf.Repeat(camTrans.eulerAngle.y + 360.0f, 360.0f);
    }
}
