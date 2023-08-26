using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Эффекты танка
    /// </summary>
    [RequireComponent(typeof(TrackTank))]
    public class TankEffect : MonoBehaviour
    {
        /// <summary>
        /// Танк
        /// </summary>
        private TrackTank tank;

        /// <summary>
        /// Выхлоп
        /// </summary>
        [SerializeField] private ParticleSystem[] exhaust;
        /// <summary>
        /// Выхлоп при старте
        /// </summary>
        [SerializeField] private ParticleSystem[] exhaustAtMovementStart;
        /// <summary>
        /// Минимальное и максимальное рассеивание выхлопа
        /// </summary>
        [SerializeField] private Vector2 minMaxExhaustEmission;

        /// <summary>
        /// Танк остановился
        /// </summary>
        private bool isTankStopped;


        private void Start()
        {
            tank = GetComponent<TrackTank>();
        }

        private void Update()
        {
            float exhaustEmission = Mathf.Lerp(minMaxExhaustEmission.x, minMaxExhaustEmission.y, tank.NormalizedLinearVelocity);

            for (int i = 0; i < exhaust.Length; i++)
            {
                ParticleSystem.EmissionModule emission = exhaust[i].emission;
                emission.rateOverTime = exhaustEmission;
            }

            if (tank.LinearVelocity < 0.1f)
            {
                isTankStopped = true;
            }

            if (tank.LinearVelocity > 1)
            {
                if (isTankStopped)
                {
                    for (int i = 0; i < exhaustAtMovementStart.Length; i++)
                    {
                        exhaustAtMovementStart[i].Play();
                    }
                }

                isTankStopped = false;
            }
        }
    }
}