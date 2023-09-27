using System.Collections.Generic;
using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Vehicle))]

    /// <summary>
    /// Стрельба ИИ
    /// </summary>
    public class AIShooter : MonoBehaviour
    {
        /// <summary>
        /// Смотрящий
        /// </summary>
        [SerializeField] private VehicleViewer viewer;

        /// <summary>
        /// Позиция выстрела
        /// </summary>
        [SerializeField] private Transform firePosition;

        /// <summary>
        /// Транспорт
        /// </summary>
        private Vehicle vehicle;
        /// <summary>
        /// Цель
        /// </summary>
        private Vehicle target;
        /// <summary>
        /// Позиция, в которую смотрим
        /// </summary>
        private Transform lookTransform;

        /// <summary>
        /// Есть ли цель
        /// </summary>
        public bool HasTarget => target != null;


        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
        }

        private void Update()
        {
            FindTarget();
            LookOnTarget();
            TryFire();
        }


        /// <summary>
        /// Найти цель
        /// </summary>
        public void FindTarget()
        {
            List<Vehicle> v = viewer.GetAllVisibleVehicles();

            float minDist = float.MaxValue;
            int index = -1;

            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].HitPoint == 0) continue;

                if (v[i].TeamID == vehicle.TeamID) continue;

                float dist = Vector3.Distance(transform.position, v[i].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }

            if (index != -1)
            {
                target = v[index];

                VehicleDimentions vehicleDimentions = target.GetComponent<VehicleDimentions>();

                if (vehicleDimentions == null) return;

                lookTransform = vehicleDimentions.GetPriorityFirePoint();
            }
            else
            {
                target = null;
                lookTransform = null;
            }
        }

        /// <summary>
        /// Смотреть на цель
        /// </summary>
        private void LookOnTarget()
        {
            if (lookTransform == null) return;

            vehicle.NetAimPoint = lookTransform.position;
        }

        /// <summary>
        /// Попытаться выстрелить
        /// </summary>
        private void TryFire()
        {
            if (target == null) return;

            RaycastHit hit;

            if (Physics.Raycast(firePosition.position, firePosition.forward, out hit, 1000))
            {
                if (hit.collider.transform.root == target.transform.root)
                {
                    vehicle.Turret.SvFire();
                }
            }
        }
    }
}