using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderTypes
{
    Side,
    DestroyableBarrier,
    Push,
    Halt,
    Finish
}
public class ColliderAction : MonoBehaviour
{
    public ColliderTypes colliderType;
    public bool isObstacle = false;
    public float pushForce = 1000;
    public float fearIncreaseAmount = 0.1f;
    private bool enableCollider = true;
    void OnTriggerEnter(Collider collider)
    {
        if(enableCollider == false)
        return;

        PlayerController playerController = collider.GetComponentInParent<PlayerController>();
        if(playerController != null)
        {
            if(playerController.enableCollision == true)
            {
                StartColliderEffect(playerController);
            }
        }
    }

    void Update()
    {
        if(isObstacle == true)
        {
            if(GameManager.Instance.player.transform.position.z - 20> transform.position.z)
            {
                gameObject.SetActive(false);
            }
        }
    }
    private void StartColliderEffect(PlayerController player)
    {
        if(isObstacle == true)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Ground");
            }
        }
        enableCollider = false;
        if(colliderType == ColliderTypes.Side)
        {
            GameOverEffect(player);
            return;
        }
        else if(colliderType == ColliderTypes.Push)
        {
            PushEffect(player);
            return;
        }
        else if(colliderType == ColliderTypes.Halt)
        {
            HaltEffect(player);
            return;
        }
        else if(colliderType == ColliderTypes.Finish)
        {
            FinishEffect(player);
            return;
        }
    }

    private void FinishEffect(PlayerController player)
    {
        if(player.GetRigidBody() != null)
        {
            GameManager.Instance.isFinished = true;
            Vector3 velocity = player.GetRigidBody().velocity;
            player.StopMovement();
            player.GetRigidBody().velocity = velocity;
            //player.GetRigidBody().AddForce(velocity.normalized*-1*pushForce, ForceMode.Force);
            player.StopAfterSecs(2, true);
        }
    }

    private void PushEffect(PlayerController player)
    {
        //player.StartStableWheel();
        if(isObstacle == true)
        {
            player.IncreaseFear(fearIncreaseAmount);
        }
        Rigidbody rgd = GetComponentInChildren<Rigidbody>();
        if(rgd && player.GetRigidBody() != null)
        {
            rgd.useGravity = true;
            rgd.AddForce(player.GetRigidBody().velocity.normalized*pushForce, ForceMode.Force);
            player.PlayScreamSound();
        }
    }

    private void HaltEffect(PlayerController player)
    {
        if(player.GetRigidBody() != null)
        {
            GameManager.Instance.PlayCarCrashSound();
            Vector3 velocity = player.GetRigidBody().velocity;
            player.StopMovement();
            player.GetRigidBody().AddForce(velocity.normalized*-1*pushForce, ForceMode.Force);
            player.StopAfterSecs(1.8f, false);
        }
    }

    private void GameOverEffect(PlayerController player)
    {
        if(player.GetRigidBody() != null)
        {
            GameManager.Instance.PlayCarCrashSound();
            Vector3 velocity = player.GetRigidBody().velocity;
            player.StopMovement();
            player.GetRigidBody().AddForce(velocity.normalized*-1*pushForce, ForceMode.Force);
            player.StopAfterSecs(1.8f, false);
        }

        if(GameManager.Instance.isFinished == false)
        {
            GameManager.Instance.OpenGameOverMenu();
        }
    }

}
