using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public string respawnPointTag = "RespawnPoint"; // Тег для точек спавна

    private Transform[] respawnPoints; // Массив точек спавна
    private Rigidbody playerRigidbody; // Ригидбоди игрока
    private Vector3 initialPosition; // Начальная позиция игрока
    private Quaternion initialRotation; // Начальная ротация игрока

    void Start()
    {
        // Сохраняем начальную позицию и ротацию игрока
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Получаем ригидбоди игрока
        playerRigidbody = GetComponent<Rigidbody>();

        // Находим все точки спавна по тегу
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag(respawnPointTag);
        respawnPoints = new Transform[spawnObjects.Length];

        for (int i = 0; i < spawnObjects.Length; i++)
        {
            respawnPoints[i] = spawnObjects[i].transform;
        }

        if (respawnPoints.Length == 0)
        {
            Debug.LogError("No respawn points found on the scene!");
        }
    }

    void Update()
    {
        // Проверяем нажатие кнопки R
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if (respawnPoints != null && respawnPoints.Length > 0)
        {
            // Выбираем случайную точку спавна
            Transform respawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];

            // Перемещаем игрока в точку спавна
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            // Если точки спавна не заданы, возвращаем игрока на начальную позицию
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        // Сбрасываем скорость игрока
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
    }
}