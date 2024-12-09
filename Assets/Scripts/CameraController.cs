using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraController : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera[] cameras; // Основные виды камеры
    public CinemachineVirtualCamera rearViewCamera; // Камера заднего вида
    public CinemachineFreeLook freeCamera; // Свободная камера

    [Header("Free Camera Settings")]
    public float sensitivity = 2f; // Чувствительность мыши
    public float freeCameraMoveSpeed = 5f; // Скорость перемещения свободной камеры

    private int currentCameraIndex = 0;
    private bool isRearViewActive = false;
    private bool isFreeCameraActive = false;

    private PhotonView photonView;

    private Vector3 freeCameraRotation; // Хранение текущего угла вращения

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Если объект не принадлежит локальному игроку, отключаем камеры
        if (!photonView.IsMine)
        {
            DisableAllCameras();
            if (freeCamera != null) freeCamera.enabled = false;
            return;
        }

        // Убедимся, что только первая камера активна при старте
        ActivateCamera(0);

        // Убедимся, что свободная камера изначально отключена
        if (freeCamera != null)
        {
            freeCamera.enabled = false;
            freeCameraRotation = freeCamera.transform.eulerAngles;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Переключение между камерами по клавише V
        if (Input.GetKeyDown(KeyCode.V) && !isRearViewActive && !isFreeCameraActive)
        {
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
            ActivateCamera(currentCameraIndex);
        }

        // Включение камеры заднего вида при удержании C
        if (Input.GetKey(KeyCode.C))
        {
            ActivateRearViewCamera(true);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            ActivateRearViewCamera(false);
        }

        // Управление свободной камерой при удержании ПКМ
        if (Input.GetMouseButtonDown(1)) // ПКМ нажата
        {
            ActivateFreeCamera(true);
        }
        else if (Input.GetMouseButtonUp(1)) // ПКМ отпущена
        {
            ActivateFreeCamera(false);
        }

        // Если активна свободная камера, обрабатываем ввод мыши
        if (isFreeCameraActive)
        {
            HandleFreeCameraMovement();
        }
    }

    private void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == index) ? 10 : 0; // Камера с приоритетом 10 становится активной
        }

        if (rearViewCamera != null)
        {
            rearViewCamera.Priority = 0; // Убедимся, что камера заднего вида отключена
        }

        if (freeCamera != null)
        {
            freeCamera.enabled = false; // Отключаем свободную камеру
        }
    }

    private void ActivateRearViewCamera(bool activate)
    {
        if (rearViewCamera == null) return;

        isRearViewActive = activate;
        rearViewCamera.Priority = activate ? 10 : 0;

        if (activate)
        {
            foreach (var cam in cameras)
            {
                cam.Priority = 0;
            }
        }
        else
        {
            ActivateCamera(currentCameraIndex);
        }
    }

    private void ActivateFreeCamera(bool activate)
    {
        isFreeCameraActive = activate;

        if (freeCamera != null)
        {
            freeCamera.Priority = activate ? 10 : 0; // Устанавливаем приоритет для свободной камеры
            freeCamera.enabled = activate;

            // Отключаем все другие камеры
            foreach (var cam in cameras)
            {
                cam.Priority = 0;
            }
            if (rearViewCamera != null) rearViewCamera.Priority = 0;
        }

        // Убираем курсор мыши при активации свободной камеры
        Cursor.lockState = activate ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !activate;
    }

    private void HandleFreeCameraMovement()
    {
        // Проверяем, что камера включена
        if (freeCamera == null || !freeCamera.enabled) return;

        // Получаем ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Обновляем вращение
        freeCameraRotation.x = Mathf.Clamp(freeCameraRotation.x - mouseY, -90f, 90f); // Ограничиваем наклон по вертикали
        freeCameraRotation.y += mouseX;
        freeCamera.transform.eulerAngles = new Vector3(freeCameraRotation.x, freeCameraRotation.y, 0f);

        // Передвижение свободной камеры
        float moveX = Input.GetAxis("Horizontal") * freeCameraMoveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * freeCameraMoveSpeed * Time.deltaTime;

        freeCamera.transform.Translate(new Vector3(moveX, 0, moveZ), Space.Self);
    }

    private void DisableAllCameras()
    {
        foreach (var cam in cameras)
        {
            cam.Priority = 0;
        }

        if (rearViewCamera != null)
        {
            rearViewCamera.Priority = 0;
        }

        if (freeCamera != null)
        {
            freeCamera.enabled = false;
        }
    }
}
