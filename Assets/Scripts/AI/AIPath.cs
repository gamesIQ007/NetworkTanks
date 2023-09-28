using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Путевые точки для ИИ
    /// </summary>
    public class AIPath : MonoBehaviour
    {
        /// <summary>
        /// Синглтон
        /// </summary>
        public static AIPath Instance;

        /// <summary>
        /// Расположение красной базы
        /// </summary>
        [SerializeField] private Transform baseRedPoint;
        /// <summary>
        /// Расположение синей базы
        /// </summary>
        [SerializeField] private Transform baseBluePoint;

        /// <summary>
        /// Красные огневые точки
        /// </summary>
        [SerializeField] private Transform[] fireRedPoints;
        /// <summary>
        /// Синие огневые точки
        /// </summary>
        [SerializeField] private Transform[] fireBluePoints;

        /// <summary>
        /// Точки патрулирования
        /// </summary>
        [SerializeField] private Transform[] patrolPoints;


        private void Awake()
        {
            Instance = this;
        }


        /// <summary>
        /// Получить точку базы врага
        /// </summary>
        /// <param name="teamID">ID команды</param>
        /// <returns>Точка базы врага</returns>
        public Vector3 GetBasePoint(int teamID)
        {
            if (teamID == TeamSide.TeamRed)
            {
                return baseBluePoint.position;
            }
            if (teamID == TeamSide.TeamBlue)
            {
                return baseRedPoint.position;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Получить случайную огневую точку
        /// </summary>
        /// <param name="teamID">ID команды</param>
        /// <returns>Огневая точка</returns>
        public Vector3 GetRandomFirePoint(int teamID)
        {
            if (teamID == TeamSide.TeamRed)
            {
                return fireRedPoints[Random.Range(0, fireRedPoints.Length)].position;
            }
            if (teamID == TeamSide.TeamBlue)
            {
                return fireBluePoints[Random.Range(0, fireBluePoints.Length)].position;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Получить случайную точку патрулирования
        /// </summary>
        /// <returns>Точка патрулирования</returns>
        public Vector3 GetRandomPatrolPoint()
        {
            return patrolPoints[Random.Range(0, patrolPoints.Length)].position;
        }
    }
}