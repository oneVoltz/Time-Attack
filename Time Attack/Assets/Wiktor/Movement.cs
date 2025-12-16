using UnityEngine;

public class Movement : MonoBehaviour
{
    InputSystemActions inputSystemActions;
    Vector2 movement;
    bool jump;
    void Awake()
    {
        inputSystemActions = new InputSystemActions();
        inputSystemActions.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Move.canceled += _ => movement = Vector2.zero;

        inputSystemActions.Player.Jump.performed += _ => jump = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
