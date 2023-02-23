using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPlatform : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D _characterController;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_characterController.Grounded) return;
        GameController.i.WinGame();
    }
}
