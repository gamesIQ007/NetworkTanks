using System.Collections.Generic;
using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Collider))]

    /// <summary>
    /// База команды
    /// </summary>
    public class TeamBase : MonoBehaviour
    {
        /// <summary>
        /// Уровень захвата
        /// </summary>
        [SerializeField] private float captureLevel;
        public float CaptureLevel => captureLevel;

        /// <summary>
        /// Количество захвата на транспорт
        /// </summary>
        [SerializeField] private float captureAmountPerVehicle;

        /// <summary>
        /// ID команды
        /// </summary>
        [SerializeField] private int teamID;

        /// <summary>
        /// Все захватывающие транспорты
        /// </summary>
        [SerializeField] private List<Vehicle> allVehicles = new List<Vehicle>();


        private void Update()
        {
            if (NetworkSessionManager.Instance.IsServer)
            {
                bool isAllDead = true;

                for (int i = 0; i < allVehicles.Count; i++)
                {
                    if (allVehicles[i].HitPoint != 0)
                    {
                        isAllDead = false;

                        captureLevel += captureAmountPerVehicle * Time.deltaTime;
                        captureLevel = Mathf.Clamp(captureLevel, 0, 100);
                    }
                }

                if (allVehicles.Count == 0 || isAllDead)
                {
                    captureLevel = 0;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Vehicle v = other.transform.root.GetComponent<Vehicle>();

            if (v == null) return;
            if (v.HitPoint == 0) return;
            if (allVehicles.Contains(v)) return;
            if (v.Owner.GetComponent<MatchMember>().TeamID == teamID) return;

            v.HitPointChanged += OnHitPointChange;

            allVehicles.Add(v);
        }

        private void OnTriggerExit(Collider other)
        {
            Vehicle v = other.transform.root.GetComponent<Vehicle>();

            if (v == null) return;

            v.HitPointChanged -= OnHitPointChange;

            allVehicles.Remove(v);
        }


        /// <summary>
        /// Сброс
        /// </summary>
        public void Reset()
        {
            captureLevel = 0;

            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].HitPointChanged -= OnHitPointChange;
            }

            allVehicles.Clear();
        }


        /// <summary>
        /// При изменении здоровья
        /// </summary>
        /// <param name="hitpoint">Здоровье</param>
        private void OnHitPointChange(int hitpoint)
        {
            captureLevel = 0;
        }
    }
}