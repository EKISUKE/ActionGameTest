using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 入力からCharacterControllerに情報を通知する
/// </summary>
public class InputController : MonoBehaviour
{
    protected const int bitBufferCount = 2;

    public class InputInfo
    {
        public List<BitArray> controllerBits = new List<BitArray>(bitBufferCount);
        public Vector2 LStick { get; set; }
        public Vector2 RStick { get; set; }
        public BitArray currentBit
        {
            get
            {
                return controllerBits[GetCurrentBufferIndex()];
            }
        }
        public BitArray prevBit
        {
            get
            {
                return controllerBits[GetPrevBufferIndex()];
            }
        }


        public InputInfo()
        {
            for (int i = 0; i < controllerBits.Capacity; ++i)
            {
                controllerBits.Add(new BitArray(64));
            }
        }

        int GetCurrentBufferIndex()
        {
            return Time.frameCount % bitBufferCount;
        }

        int GetPrevBufferIndex()
        {
            return (bitBufferCount + (GetCurrentBufferIndex() - 1)) % bitBufferCount;
        }

        public void UpdateControllerInfo()
        {
            var currentBuffer = GetCurrentBufferIndex();
            controllerBits[currentBuffer].Set(0, Input.GetButtonDown("Fire1"));
            //RStick = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            RStick = new Vector2(Input.GetAxis("CamX"), Input.GetAxis("CamY"));
            LStick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    };
    

    /// <summary>
    /// ビット更新後のビットを使った処理呼び出し
    /// </summary>
    /// <param name="controllerBits"></param>
    public delegate void DoSomethingWithBits(InputInfo inputInfo);

    protected InputInfo inputInfo = null;

    protected DoSomethingWithBits delegateDoSomething = null;

    public void RegisterDelegateWithControllerInfo(DoSomethingWithBits function)
    {
        delegateDoSomething += function;
    }

    public void UnRegisterDelegateWithControllerInfo(DoSomethingWithBits function)
    {
        delegateDoSomething -= function;
    }


    // Start is called before the first frame update
    void Start()
    {
        inputInfo = new InputInfo();
    }

    

    

    // Update is called once per frame
    void Update()
    {
        inputInfo.UpdateControllerInfo();
        delegateDoSomething?.Invoke(inputInfo);
    }
}
