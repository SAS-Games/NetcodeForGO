using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private Transform m_TurretTransform;

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        Vector2 aimScreenPosition = m_inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        m_TurretTransform.up = new Vector2(
            aimWorldPosition.x - m_TurretTransform.position.x,
            aimWorldPosition.y - m_TurretTransform.position.y);
    }
}
