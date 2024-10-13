using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader m_inputReader;
    // Start is called before the first frame update
    void Start()
    {
        m_inputReader.PrimaryFireEvent += val =>
        {
            Debug.Log(val);
        };

        m_inputReader.MoveEvent += val =>
        {
            Debug.Log(val);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
