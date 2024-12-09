using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarSpeedText : MonoBehaviour
{
    public TextMeshProUGUI CarSpeed;
    public Rigidbody Car;
    // Start is called before the first frame update
    void Start()
    {
        Car = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CarSpeed.text = (Car.linearVelocity.magnitude * 2.23693629f).ToString("0");
    }
}
