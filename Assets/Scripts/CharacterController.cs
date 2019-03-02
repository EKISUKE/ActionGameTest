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
    protected Animator        animator = null;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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

    }

    void UpdateFromInputController(InputController.InputInfo inputInfo)
    {
        if (!animator) return;
        if (inputInfo.currentBit.Get(0))
        {
            animator.SetTrigger("Attack");
        }
    }
}
