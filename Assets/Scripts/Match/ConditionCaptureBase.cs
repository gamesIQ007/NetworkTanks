using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Условие захвата базы
    /// </summary>
    public class ConditionCaptureBase : NetworkBehaviour, IMatchCondition
    {
        /// <summary>
        /// Красная база
        /// </summary>
        [SerializeField] TeamBase redBase;
        /// <summary>
        /// Синяя база
        /// </summary>
        [SerializeField] TeamBase blueBase;

        /// <summary>
        /// Уровень захвата красной базы
        /// </summary>
        [SyncVar]
        private float redBaseCaptureLevel;
        public float RedBaseCaptureLevel => redBaseCaptureLevel;
        /// <summary>
        /// Уровень захвата синей базы
        /// </summary>
        [SyncVar]
        private float blueBaseCaptureLevel;
        public float BlueBaseCaptureLevel => blueBaseCaptureLevel;

        /// <summary>
        /// Условие выполнено?
        /// </summary>
        private bool triggered;


        bool IMatchCondition.IsTriggered => triggered;

        void IMatchCondition.OnServerMatchStart(MatchController controller)
        {
            Reset();
        }

        void IMatchCondition.OnServerMatchEnd(MatchController controller)
        {
            enabled = false;
        }


        private void Start()
        {
            enabled = false;
        }

        private void Update()
        {
            if (isServer)
            {
                redBaseCaptureLevel = redBase.CaptureLevel;
                blueBaseCaptureLevel = blueBase.CaptureLevel;

                if (redBaseCaptureLevel == 100 || blueBaseCaptureLevel == 100)
                {
                    triggered = true;
                }
            }
        }


        /// <summary>
        /// Сброс
        /// </summary>
        private void Reset()
        {
            redBase.Reset();
            blueBase.Reset();

            redBaseCaptureLevel = 0;
            blueBaseCaptureLevel = 0;

            triggered = false;
            enabled = true;
        }
    }
}