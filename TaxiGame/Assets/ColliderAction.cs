using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderTypes
{
    Side,
    DestroyableBarrier,
    Push
}
public class ColliderAction : MonoBehaviour
{
    public ColliderTypes colliderType;
    private bool enableCollider = true;
    void OnTriggerEnter(Collider collider)
    {
        if(enableCollider == false)
        return;

        PlayerController playerController = collider.GetComponentInParent<PlayerController>();
        if(playerController != null)
        {
            StartColliderEffect(playerController);
        }
    }

    private void StartColliderEffect(PlayerController player)
    {
        enableCollider = false;
        if(colliderType == ColliderTypes.Side)
        {
            GameOverEffect(player);
            return;
        }
        if(colliderType == ColliderTypes.Push)
        {
            PushEffect(player);
            return;
        }
    }

    private void PushEffect(PlayerController player)
    {
        //player.StartStableWheel();
        // Rigidbody rgd = GetComponentInChildren<Rigidbody>();
        // if(rgd && player.GetRigidBody() != null)
        // {
        //     rgd.AddForce(player.GetRigidBody().velocity* Time.deltaTime*30000, ForceMode.Force);
        // }
    }

    private void GameOverEffect(PlayerController player)
    {
        player.StopMovement();
    }
}
