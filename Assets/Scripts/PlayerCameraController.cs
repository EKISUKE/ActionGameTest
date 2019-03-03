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
        public Vector3    eulerAngle;
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
    [Tooltip("現在目標とする注視点")]
    protected Vector3      currentForcusPos = Vector3.zero;
    [Tooltip("入力によって回転角度が変更されたかどうか")]
    protected bool         isChangeRotateByInput   = false;

    // Start is called before the first frame update
    void Start()
    {
        var inputController = forcusTrans.GetComponent<InputController>();
        if (inputController)
        {
            inputController.RegisterDelegateWithControllerInfo(UpdateFromInputController);
        }

        camTrans = new CameraTransform(this.transform);
        currentForcusPos = forcusTrans.position + Vector3.up;
    }

    void OnDestroy()
    {
        var inputController = forcusTrans.GetComponent<InputController>();
        if (inputController)
        {
            inputController.UnRegisterDelegateWithControllerInfo(UpdateFromInputController);
        }
    }

    Vector3 CalclateTargetPos(Vector3 forcusPos, float minLength)
    {
        var targetPos = this.transform.position;
        var adjustLength = Vector3.Magnitude(forcusPos - this.transform.position);
        if(length < adjustLength)
        {
            adjustLength = length;
        }
        adjustLength = adjustLength < minLength ? minLength : adjustLength;
        // プレイヤーの位置からカメラの位置を逆算
        targetPos = forcusPos + (this.transform.rotation * Vector3.back) * adjustLength;
        return targetPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 注視点 + Y軸1m
        var forcusPos = forcusTrans.position + Vector3.up;
        var targetVec = forcusPos - this.transform.position;
        ref var euler     = ref camTrans.eulerAngle;
        // 入力による回転角度変更がない場合
        if (!isChangeRotateByInput) {
            // ターゲット方向への回転角度を求める
            euler = Quaternion.LookRotation(targetVec.normalized).eulerAngles;
            euler.x = euler.x - 360.0f < -180.0f ? euler.x : euler.x - 360.0f;
        }
        // 回転角度制限
        euler.x = Mathf.Clamp(euler.x, -50.0f, 70.0f);

        camTrans.hRotation = Quaternion.Euler(0.0f, euler.y, 0.0f);
        camTrans.vRotation = Quaternion.Euler(euler.x, 0.0f, 0.0f);
        this.transform.rotation = camTrans.hRotation * camTrans.vRotation;
        currentForcusPos = forcusPos;
        var targetPos = CalclateTargetPos(currentForcusPos, 1.0f);

        this.transform.position = targetPos;
    }

    void UpdateFromInputController(InputController.InputInfo inputInfo)
    {
        var factor = 100.0f;
        if (inputInfo.RStick.magnitude > 0.0f)
        {
            camTrans.eulerAngle.y += inputInfo.RStick.x * factor * Time.deltaTime;
            camTrans.eulerAngle.x += inputInfo.RStick.y * factor * Time.deltaTime;
            camTrans.eulerAngle.x = Mathf.Clamp(camTrans.eulerAngle.x, -80.0f, 80.0f);
            isChangeRotateByInput = true;
        }
        else
        {
            isChangeRotateByInput = false;
        }

        
    }
}
