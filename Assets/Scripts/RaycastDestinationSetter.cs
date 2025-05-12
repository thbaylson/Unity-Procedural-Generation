using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastDestinationSetter : MonoBehaviour
{
    [SerializeField] DirectedAgent agent;

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            {
                agent.MoveToLocation(hit.point);
            }
        }
    }
}
