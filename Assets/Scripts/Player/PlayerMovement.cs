using SAS.Utilities.TagSystem;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [FieldRequiresSelf] private Rigidbody2D _rigidbody;
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private Transform m_BodyTransform;

    [SerializeField] private float m_movementSpeed = 4;
    [SerializeField] private float m_turningSpeed = 270;

    [SerializeField] private ParticleSystem m_DustTrail;
    [SerializeField] private float m_EmissionRate = 10f;

    private Vector2 previousMoveInput;
    private Vector3 previousPos;
    private ParticleSystem.EmissionModule emissionModule;
    private const float ParticleStopThreshhold = 0.005f;



    private void Awake()
    {
        this.Initialize();
        emissionModule = m_DustTrail.emission;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        m_inputReader.MoveEvent += HandleMove;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        m_BodyTransform.Rotate(previousMoveInput.x * -m_turningSpeed * Time.deltaTime * Vector3.forward);
    }

    private void FixedUpdate()
    {
        if ((transform.position - previousPos).sqrMagnitude > ParticleStopThreshhold)
            emissionModule.rateOverTime = m_EmissionRate;
        else
            emissionModule.rateOverTime = 0;

        previousPos = transform.position;
        if (!IsOwner)
            return;
        _rigidbody.velocity = previousMoveInput.y * m_movementSpeed * (Vector2)m_BodyTransform.up;
    }

    private void HandleMove(Vector2 newMoveInput)
    {
        previousMoveInput = newMoveInput;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
        m_inputReader.MoveEvent += HandleMove;
    }
}
