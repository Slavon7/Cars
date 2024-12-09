using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player; // Префаб игрока

    [Space]
    public Transform[] spawnPoints; // Массив точек спавна

    private List<Transform> availableSpawnPoints = new List<Transform>(); // Список доступных точек спавна

    void Start()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();

        // Инициализируем список доступных точек спавна
        availableSpawnPoints.AddRange(spawnPoints);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("We`re in the lobby");
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We`re connected and in a room");

        // Выбираем случайную точку спавна
        Transform selectedSpawnPoint = GetRandomAvailableSpawnPoint();

        if (selectedSpawnPoint != null)
        {
            // Спавним игрока в выбранной точке
            GameObject _player = PhotonNetwork.Instantiate(player.name, selectedSpawnPoint.position, selectedSpawnPoint.rotation);

            // Инициализируем локального игрока
            _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        }
        else
        {
            Debug.LogError("No available spawn points!");
        }
    }

    // Метод для выбора случайной доступной точки спавна
    private Transform GetRandomAvailableSpawnPoint()
    {
        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogError("No available spawn points!");
            return null;
        }

        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        Transform spawnPoint = availableSpawnPoints[randomIndex];

        // Удаляем выбранную точку из доступных
        availableSpawnPoints.RemoveAt(randomIndex);

        return spawnPoint;
    }
}