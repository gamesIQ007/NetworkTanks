using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Таймер матча
    /// </summary>
    public class MatchTimer : NetworkBehaviour, IMatchCondition
    {
        /// <summary>
        /// Время матча
        /// </summary>
        [SerializeField] private float matchTime;

        /// <summary>
        /// Оставшееся время
        /// </summary>
        [SyncVar]
        private float timeLeft;
        public float TimeLeft => timeLeft;

        /// <summary>
        /// Завершился ли таймер
        /// </summary>
        private bool timerEnd = false;


        private void Start()
        {
            timeLeft = matchTime;

            if (isServer)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (isServer)
            {
                timeLeft -= Time.deltaTime;

                if (timeLeft <= 0)
                {
                    timeLeft = 0;
                    timerEnd = true;
                }
            }
        }


        bool IMatchCondition.IsTriggered => timerEnd;

        void IMatchCondition.OnServerMatchStart(MatchController controller)
        {
            Reset();
        }

        void IMatchCondition.OnServerMatchEnd(MatchController controller)
        {
            enabled = false;
        }


        /// <summary>
        /// Сброс
        /// </summary>
        private void Reset()
        {
            enabled = true;
            timeLeft = matchTime;
            timerEnd = false;
        }
    }
}