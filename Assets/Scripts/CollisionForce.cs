using UnityEngine;
using Photon.Pun;

public class CollisionForce : MonoBehaviour
{
    public float impactMultiplier = 10f; // Умножитель силы

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRigidbody = collision.rigidbody;
        if (otherRigidbody != null && photonView.IsMine)
        {
            // Направление силы - от центра текущего объекта к точке столкновения
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection = -forceDirection.normalized;

            // Применяем силу локально
            otherRigidbody.AddForce(forceDirection * impactMultiplier, ForceMode.Impulse);

            // Синхронизируем силу через RPC
            photonView.RPC("ApplyCollisionForce", RpcTarget.Others, collision.transform.position, forceDirection);
        }
    }

    [PunRPC]
    private void ApplyCollisionForce(Vector3 collisionPosition, Vector3 forceDirection)
    {
        // Получаем Rigidbody другого игрока
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(forceDirection * impactMultiplier, ForceMode.Impulse);
        }
    }
}
