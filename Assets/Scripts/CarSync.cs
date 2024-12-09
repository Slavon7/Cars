using UnityEngine;
using Photon.Pun;

public class CarSync : MonoBehaviourPun, IPunObservable
{
    public Transform carBody; // Основной объект машины
    public Transform[] wheelMeshes; // Визуальные модели колёс
    public WheelCollider[] wheelColliders; // Коллайдеры колёс

    private Vector3 carPosition;
    private Quaternion carRotation;

    private Vector3[] wheelPositions;
    private Quaternion[] wheelRotations;

    private void Start()
    {
        wheelPositions = new Vector3[wheelColliders.Length];
        wheelRotations = new Quaternion[wheelColliders.Length];
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            SyncCarAndWheels(); // Обновление данных машины и колёс
        }
        else
        {
            ApplySyncedData();
        }
    }

    private void SyncCarAndWheels()
    {
        // Обновляем положение и вращение машины
        carPosition = carBody.position;
        carRotation = carBody.rotation;

        // Обновляем положение и вращение колёс
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].GetWorldPose(out wheelPositions[i], out wheelRotations[i]);
            wheelMeshes[i].position = wheelPositions[i];
            wheelMeshes[i].rotation = wheelRotations[i];
        }
    }

    private void ApplySyncedData()
    {
        // Интерполяция для плавного обновления машины
        carBody.position = Vector3.Lerp(carBody.position, carPosition, Time.deltaTime * 15f);
        carBody.rotation = Quaternion.Lerp(carBody.rotation, carRotation, Time.deltaTime * 15f);

        // Интерполяция для колёс
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            wheelMeshes[i].position = Vector3.Lerp(wheelMeshes[i].position, wheelPositions[i], Time.deltaTime * 15f);
            wheelMeshes[i].rotation = Quaternion.Lerp(wheelMeshes[i].rotation, wheelRotations[i], Time.deltaTime * 15f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Передаём положение и вращение машины
            stream.SendNext(carPosition);
            stream.SendNext(carRotation);

            // Передаём данные о колёсах
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                stream.SendNext(wheelPositions[i]);
                stream.SendNext(wheelRotations[i]);
            }
        }
        else
        {
            // Получаем положение и вращение машины
            carPosition = (Vector3)stream.ReceiveNext();
            carRotation = (Quaternion)stream.ReceiveNext();

            // Получаем данные о колёсах
            for (int i = 0; i < wheelMeshes.Length; i++)
            {
                wheelPositions[i] = (Vector3)stream.ReceiveNext();
                wheelRotations[i] = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}