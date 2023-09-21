﻿using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    [RequireComponent(typeof(Vehicle))]

    /// <summary>
    /// Смотритель транспорта (кто смотрит)
    /// </summary>
    public class VehicleViewer : NetworkBehaviour
    {
        /// <summary>
        /// Дистанция смотрения
        /// </summary>
        [SerializeField] private float viewDistance;

        /// <summary>
        /// Дистанция рентгена
        /// </summary>
        [SerializeField] private float xrayDistance;

        /// <summary>
        /// Время ухода из обнаружения
        /// </summary>
        [SerializeField] private float exitTimeFromDiscovery;

        /// <summary>
        /// Точки смотрения
        /// </summary>
        [SerializeField] private Transform[] viewPoints;

        /// <summary>
        /// Цвет, для дебага
        /// </summary>
        [SerializeField] private Color color;

        /// <summary>
        /// Все габаритные точки. Публик для дебага
        /// </summary>
        public List<VehicleDimentions> allVehicleDimentions = new List<VehicleDimentions>();

        /// <summary>
        /// Видимый транспорт
        /// </summary>
        public SyncList<NetworkIdentity> visibleVehicles = new SyncList<NetworkIdentity>();

        /// <summary>
        /// Массив оставшегося времени видимости
        /// </summary>
        public List<float> remainingTime = new List<float>();

        /// <summary>
        /// Транспорт
        /// </summary>
        private Vehicle vehicle;


        private void Update()
        {
            if (isServer == false) return;

            for (int i = 0; i < allVehicleDimentions.Count; i++)
            {
                if (allVehicleDimentions[i].Vehicle == null) continue;

                bool isVisible = false;

                for (int j = 0; j < viewPoints.Length; j++)
                {
                    isVisible = CheckVisibility(viewPoints[j].position, allVehicleDimentions[i]);

                    if (isVisible) break;
                }

                if (Vector3.Distance(vehicle.transform.position, allVehicleDimentions[i].transform.position) <= xrayDistance)
                {
                    isVisible = true;
                }

                if (isVisible && visibleVehicles.Contains(allVehicleDimentions[i].Vehicle.netIdentity) == false)
                {
                    visibleVehicles.Add(allVehicleDimentions[i].Vehicle.netIdentity);
                    remainingTime.Add(-1);
                }

                if (isVisible && visibleVehicles.Contains(allVehicleDimentions[i].Vehicle.netIdentity))
                {
                    remainingTime[visibleVehicles.IndexOf(allVehicleDimentions[i].Vehicle.netIdentity)] = -1;
                }

                if (isVisible == false && visibleVehicles.Contains(allVehicleDimentions[i].Vehicle.netIdentity))
                {
                    if (remainingTime[visibleVehicles.IndexOf(allVehicleDimentions[i].Vehicle.netIdentity)] == -1)
                    {
                        remainingTime[visibleVehicles.IndexOf(allVehicleDimentions[i].Vehicle.netIdentity)] = exitTimeFromDiscovery;
                    }
                }
            }

            for (int i = 0; i < remainingTime.Count; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= Time.deltaTime;

                    if (remainingTime[i] <= 0)
                    {
                        remainingTime[i] = 0;
                    }
                }

                if (remainingTime[i] == 0)
                {
                    remainingTime.RemoveAt(i);
                    visibleVehicles.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// При старте сервера
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();

            vehicle = GetComponent<Vehicle>();

            NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
        }

        /// <summary>
        /// При остановке сервера
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopServer();

            NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnSvMatchStart()
        {
            color = Random.ColorHSV();

            Vehicle[] allVehicles = FindObjectsOfType<Vehicle>();

            for (int i = 0; i < allVehicles.Length; i++)
            {
                if (vehicle == allVehicles[i]) continue;

                VehicleDimentions vehicleDimentions = allVehicles[i].GetComponent<VehicleDimentions>();

                if (vehicleDimentions == null) continue;

                if (vehicle.TeamID != allVehicles[i].TeamID)
                {
                    allVehicleDimentions.Add(vehicleDimentions);
                }
                else
                {
                    visibleVehicles.Add(allVehicles[i].netIdentity);
                    remainingTime.Add(-1);
                }
            }
        }

        /// <summary>
        /// Проверка видимости
        /// </summary>
        /// <param name="viewPoint">Точка смотра</param>
        /// <param name="vehicleDimentions">Габаритная точка</param>
        /// <returns>Видно ли</returns>
        private bool CheckVisibility(Vector3 viewPoint, VehicleDimentions vehicleDimentions)
        {
            float distance = Vector3.Distance(transform.position, vehicleDimentions.transform.position);

            if (distance > viewDistance)
            {
                return false;
            }

            return vehicleDimentions.IsVisibleFromPoint(transform.root, viewPoint, color);
        }

        /// <summary>
        /// Видно ли танк по ид?
        /// </summary>
        /// <param name="identity">Ид танка</param>
        /// <returns>Видно ли танк по ид?</returns>
        public bool IsVisible(NetworkIdentity identity)
        {
            return visibleVehicles.Contains(identity);
        }
    }
}