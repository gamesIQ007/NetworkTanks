using System.Collections.Generic;
using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение транспорта камерой
    /// </summary>
    public class VehicleVisibilityInCamera : MonoBehaviour
    {
        /// <summary>
        /// Список всех транспортов
        /// </summary>
        private List<Vehicle> vehicles = new List<Vehicle>();


        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            }
        }

        private void Update()
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(vehicles[i].netIdentity);

                vehicles[i].SetVisible(isVisible);
            }
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnMatchStart()
        {
            vehicles.Clear();

            Vehicle[] allVehicles = FindObjectsOfType<Vehicle>();

            for (int i = 0; i < allVehicles.Length; i++)
            {
                if (allVehicles[i] == Player.Local.ActiveVehicle) continue;

                vehicles.Add(allVehicles[i]);
            }
        }
    }
}