using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    [RequireComponent(typeof(TrackTank))]

    /// <summary>
    /// Модуль траков
    /// </summary>
    public class TrackModule : NetworkBehaviour
    {
        [Header("Visual")]
        /// <summary>
        /// Меш левого трака
        /// </summary>
        [SerializeField] private GameObject leftTrackMesh;
        /// <summary>
        /// Меш левого сломанного трака
        /// </summary>
        [SerializeField] private GameObject leftTrackRuinedMesh;
        /// <summary>
        /// Меш правого трака
        /// </summary>
        [SerializeField] private GameObject rightTrackMesh;
        /// <summary>
        /// Меш правого сломанного трака
        /// </summary>
        [SerializeField] private GameObject rightTrackRuinedMesh;

        [Space(5)]
        /// <summary>
        /// Модуль левого трака
        /// </summary>
        [SerializeField] private VehicleModule leftTrack;
        /// <summary>
        /// Модуль правого трака
        /// </summary>
        [SerializeField] private VehicleModule rightTrack;

        /// <summary>
        /// Танк
        /// </summary>
        private TrackTank tank;


        private void Start()
        {
            tank = GetComponent<TrackTank>();

            leftTrack.Destroyed += OnLeftTrackDestroyed;
            leftTrack.Recovered += OnLeftTrackRecovered;

            rightTrack.Destroyed += OnRightTrackDestroyed;
            rightTrack.Recovered += OnRightTrackRecovered;
        }

        private void OnDestroy()
        {
            leftTrack.Destroyed -= OnLeftTrackDestroyed;
            leftTrack.Recovered -= OnLeftTrackRecovered;

            rightTrack.Destroyed -= OnRightTrackDestroyed;
            rightTrack.Recovered -= OnRightTrackRecovered;
        }


        /// <summary>
        /// При уничтожении левого трака
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        private void OnLeftTrackDestroyed(Destructible destructible)
        {
            ChangeActiveObjects(leftTrackMesh, leftTrackRuinedMesh);

            TakeAwayMobility();
        }

        /// <summary>
        /// При восстановлении левого трака
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        private void OnLeftTrackRecovered(Destructible destructible)
        {
            ChangeActiveObjects(leftTrackMesh, leftTrackRuinedMesh);

            if (rightTrack.HitPoint > 0)
            {
                RegainMobility();
            }
        }

        /// <summary>
        /// При уничтожении правого трака
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        private void OnRightTrackDestroyed(Destructible destructible)
        {
            ChangeActiveObjects(rightTrackMesh, rightTrackRuinedMesh);

            TakeAwayMobility();
        }

        /// <summary>
        /// При восстановлении правого трака
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        private void OnRightTrackRecovered(Destructible destructible)
        {
            ChangeActiveObjects(rightTrackMesh, rightTrackRuinedMesh);

            if (leftTrack.HitPoint > 0)
            {
                RegainMobility();
            }
        }

        /// <summary>
        /// Поменять активные объекты
        /// </summary>
        /// <param name="a">Первый объект</param>
        /// <param name="b">Второй объект</param>
        private void ChangeActiveObjects(GameObject a, GameObject b)
        {
            a.SetActive(b.activeSelf);
            b.SetActive(!b.activeSelf);
        }

        /// <summary>
        /// Отобрать подвижность
        /// </summary>
        private void TakeAwayMobility()
        {
            tank.enabled = false;
        }

        /// <summary>
        /// Вернуть подвижность
        /// </summary>
        private void RegainMobility()
        {
            tank.enabled = true;
        }
    }
}