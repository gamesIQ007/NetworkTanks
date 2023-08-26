using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Движение текстуры трака
    /// </summary>
    [RequireComponent(typeof(TrackTank))]
    public class TankTrackTextureMovement : MonoBehaviour
    {
        /// <summary>
        /// Танк
        /// </summary>
        private TrackTank tank;

        /// <summary>
        /// Рендерер левого трака
        /// </summary>
        [SerializeField] private Renderer leftTrackRenderer;
        /// <summary>
        /// Рендерер правого трака
        /// </summary>
        [SerializeField] private Renderer rightTrackRenderer;

        /// <summary>
        /// Направление
        /// </summary>
        [SerializeField] private Vector2 direction;
        /// <summary>
        /// Модификатор
        /// </summary>
        [SerializeField] private float modifier;


        private void Start()
        {
            tank = GetComponent<TrackTank>();
        }

        private void FixedUpdate()
        {
            float speed = tank.LeftWheelRpm / 60.0f * modifier * Time.fixedDeltaTime;
            leftTrackRenderer.material.SetTextureOffset("_MainTex", leftTrackRenderer.material.GetTextureOffset("_MainTex") + direction * speed);

            speed = tank.RightWheelRpm / 60.0f * modifier * Time.fixedDeltaTime;
            rightTrackRenderer.material.SetTextureOffset("_MainTex", rightTrackRenderer.material.GetTextureOffset("_MainTex") + direction * speed);
        }
    }
}