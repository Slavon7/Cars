using Photon.Pun;
using UnityEngine;

public class NetworkedCar : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    [SerializeField] private float positionLerpSpeed = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    // Состояние эффектов
    private bool networkIsDrifting;
    private bool networkIsTractionLocked;
    private float networkCarSpeed;

    // Ссылки на эффекты
    public PrometeoCarController carController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!photonView.IsMine)
        {
            rb.isKinematic = true; // Физика отключена для других игроков
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Интерполяция для плавного движения
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * positionLerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * rotationLerpSpeed);

            // Обновление эффектов на основе сетевых данных
            ApplyEffects();
        }
    }

    private void ApplyEffects()
    {
        if (carController != null)
        {
            carController.isDrifting = networkIsDrifting;
            carController.isTractionLocked = networkIsTractionLocked;
            carController.carSpeed = networkCarSpeed;

            carController.DriftCarPS(); // Обновляем эффекты
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Отправляем данные локального игрока
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);

            // Состояние эффектов
            stream.SendNext(carController.isDrifting);
            stream.SendNext(carController.isTractionLocked);
            stream.SendNext(carController.carSpeed);
        }
        else
        {
            // Получаем данные от других игроков
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            // Получаем состояние эффектов
            networkIsDrifting = (bool)stream.ReceiveNext();
            networkIsTractionLocked = (bool)stream.ReceiveNext();
            networkCarSpeed = (float)stream.ReceiveNext();
        }
    }
}