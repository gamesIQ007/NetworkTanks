using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Тип поведения ИИ
    /// </summary>
    public enum AIBehaviourType
    {
        /// <summary>
        /// Патрулирование
        /// </summary>
        Patrol,
        /// <summary>
        /// Поддержка
        /// </summary>
        Support,
        /// <summary>
        /// Захват базы
        /// </summary>
        InvaderBase,
        /// <summary>
        /// Захват базы по указанному пути
        /// </summary>
        InvaderBaseByPath
    }

    /// <summary>
    /// ИИ танка
    /// </summary>
    public class TankAI : NetworkBehaviour
    {
        /// <summary>
        /// Тип поведения ИИ
        /// </summary>
        [SerializeField] private AIBehaviourType behaviourType;

        /// <summary>
        /// Шанс выбора патрулирования
        /// </summary>
        [Range(0, 1)]
        [SerializeField] private float patrolChance;
        /// <summary>
        /// Шанс выбора поддержки
        /// </summary>
        [Range(0, 1)]
        [SerializeField] private float supportChance;
        /// <summary>
        /// Шанс выбора захвата базы
        /// </summary>
        [Range(0, 1)]
        [SerializeField] private float invaderBaseChance;
        /// <summary>
        /// Шанс выбора захвата базы по указанному пути
        /// </summary>
        [Range(0, 1)]
        [SerializeField] private float invaderBaseByPathChance;

        /// <summary>
        /// Транспорт
        /// </summary>
        [SerializeField] private Vehicle vehicle;
        /// <summary>
        /// ИИ движения
        /// </summary>
        [SerializeField] private AIMovement movement;
        /// <summary>
        /// ИИ стрельбы
        /// </summary>
        [SerializeField] private AIShooter shooter;

        /// <summary>
        /// Цель
        /// </summary>
        private Vehicle fireTarget;
        /// <summary>
        /// Цель перемещения
        /// </summary>
        private Vector3 movementTarget;
        /// <summary>
        /// Индекс точки пути к базе
        /// </summary>
        private int indexOfPathToBase;

        /// <summary>
        /// Начальное количество членов команды
        /// </summary>
        private int startCountTeamMember;
        /// <summary>
        /// Текущее количество членов команды
        /// </summary>
        private int countTeamMember;


        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            vehicle.Destroyed += OnVehicleDestroyed;

            movement.enabled = false;
            shooter.enabled = false;

            CalcTeamMember();
            SetStartBehaviour();
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            }

            vehicle.Destroyed -= OnVehicleDestroyed;
        }

        private void Update()
        {
            if (isServer)
            {
                UpdateBehaviour();
            }
        }


        /// <summary>
        /// При начале матча
        /// </summary>
        private void OnMatchStart()
        {
            movement.enabled = true;
            shooter.enabled = true;
        }

        /// <summary>
        /// При уничтожении транспорта
        /// </summary>
        /// <param name="dest">Дестрактибл</param>
        private void OnVehicleDestroyed(Destructible dest)
        {
            movement.enabled = false;
            shooter.enabled = false;
        }

        /// <summary>
        /// Задать стартовое поведение
        /// </summary>
        private void SetStartBehaviour()
        {
            float chance = Random.Range(0, patrolChance + supportChance + invaderBaseChance + invaderBaseByPathChance);

            if (chance >= 0.0f && chance <= patrolChance)
            {
                StartBehaviour(AIBehaviourType.Patrol);
                return;
            }

            if (chance > patrolChance && chance <= patrolChance + supportChance)
            {
                StartBehaviour(AIBehaviourType.Support);
                return;
            }

            if (chance > patrolChance + supportChance && chance <= patrolChance + supportChance + invaderBaseChance)
            {
                StartBehaviour(AIBehaviourType.InvaderBase);
                return;
            }

            if (chance > patrolChance + supportChance + invaderBaseChance && chance <= patrolChance + supportChance + invaderBaseChance + invaderBaseByPathChance)
            {
                StartBehaviour(AIBehaviourType.InvaderBaseByPath);
                return;
            }
        }

        /// <summary>
        /// Подсчитать количество членов команды
        /// </summary>
        private void CalcTeamMember()
        {
            Vehicle[] v = FindObjectsOfType<Vehicle>();

            for (int i = 0; i < v.Length; i++)
            {
                if (v[i].TeamID == vehicle.TeamID)
                {
                    if (v[i] != vehicle)
                    {
                        startCountTeamMember++;
                        v[i].Destroyed += OnTeamMemberDestroyed;
                    }
                }
            }

            countTeamMember = startCountTeamMember;
        }


        #region Behaviour

        /// <summary>
        /// Начать поведение
        /// </summary>
        /// <param name="type">Тип поведения</param>
        private void StartBehaviour(AIBehaviourType type)
        {
            behaviourType = type;

            if (behaviourType == AIBehaviourType.InvaderBase)
            {
                movementTarget = AIPath.Instance.GetBasePoint(vehicle.TeamID);
            }

            if (behaviourType == AIBehaviourType.InvaderBaseByPath)
            {
                movementTarget = AIPath.Instance.GetPathToBasePoint(vehicle.TeamID, 0);
            }

            if (behaviourType == AIBehaviourType.Patrol)
            {
                movementTarget = AIPath.Instance.GetRandomPatrolPoint();
            }

            if (behaviourType == AIBehaviourType.Support)
            {
                movementTarget = AIPath.Instance.GetRandomFirePoint(vehicle.TeamID);
            }

            movement.ResetPath();
        }

        /// <summary>
        /// Обновить поведение
        /// </summary>
        private void UpdateBehaviour()
        {
            shooter.FindTarget();

            if (movement.ReachedDestination)
            {
                OnReachedDestination();
            }

            if (movement.HasPath == false)
            {
                movement.SetDestination(movementTarget);
            }
        }

        /// <summary>
        /// При достижении точки назначения
        /// </summary>
        private void OnReachedDestination()
        {
            if (behaviourType == AIBehaviourType.Patrol)
            {
                movementTarget = AIPath.Instance.GetRandomPatrolPoint();
            }

            if (behaviourType == AIBehaviourType.InvaderBaseByPath)
            {
                indexOfPathToBase++;
                movementTarget = AIPath.Instance.GetPathToBasePoint(vehicle.TeamID, indexOfPathToBase);
            }

            movement.ResetPath();
        }

        /// <summary>
        /// При уничтожении члена команды
        /// </summary>
        /// <param name="dest">Дестрактибл</param>
        private void OnTeamMemberDestroyed(Destructible dest)
        {
            countTeamMember--;
            dest.Destroyed -= OnTeamMemberDestroyed;

            if ((float)countTeamMember / (float)startCountTeamMember < 0.4f)
            {
                StartBehaviour(AIBehaviourType.Patrol);
            }

            if (countTeamMember <= 2)
            {
                StartBehaviour(AIBehaviourType.Patrol);
            }
        }

        #endregion
    }
}