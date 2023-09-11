using System.Collections.Generic;
using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Панель аммуниции
    /// </summary>
    public class UIAmmunitionPanel : MonoBehaviour
    {
        /// <summary>
        /// Панель аммуниции
        /// </summary>
        [SerializeField] private Transform ammunitionPanel;
        /// <summary>
        /// Префаб элемента аммуниции
        /// </summary>
        [SerializeField] private UIAmmunitionElement ammunitionElementPrefab;

        /// <summary>
        /// Список всех элементов аммуниции
        /// </summary>
        private List<UIAmmunitionElement> allAmmunitionElements = new List<UIAmmunitionElement>();
        /// <summary>
        /// Список всей аммуниции
        /// </summary>
        private List<Ammunition> allAmunition = new List<Ammunition>();

        /// <summary>
        /// Турель
        /// </summary>
        private Turret turret;
        /// <summary>
        /// Последний выбранный индекс аммуниции
        /// </summary>
        private int lastSelectedAmmunitionIndex;


        #region Unity Events

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
            }

            if (turret != null)
            {
                turret.UpdateSelectedAmmunition -= OnTurretUpdateSelectedAmmunition;
            }

            for (int i = 0; i < turret.Ammunition.Length; i++)
            {
                turret.Ammunition[i].AmmoCountChanged -= OnAmmoCountChanged;
            }
        }

        #endregion


        /// <summary>
        /// При спавне транспорта
        /// </summary>
        /// <param name="vehicle">Транспорт</param>
        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            turret = vehicle.Turret;
            turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;

            for (int i = 0; i < turret.Ammunition.Length; i++)
            {
                UIAmmunitionElement ammunitionElement = Instantiate(ammunitionElementPrefab);
                ammunitionElement.transform.SetParent(ammunitionPanel);
                ammunitionElement.transform.localScale = Vector3.one;
                ammunitionElement.SetAmmunition(turret.Ammunition[i]);

                turret.Ammunition[i].AmmoCountChanged += OnAmmoCountChanged;

                allAmmunitionElements.Add(ammunitionElement);
                allAmunition.Add(turret.Ammunition[i]);

                if (i == 0)
                {
                    ammunitionElement.Select();
                }
            }
        }

        /// <summary>
        /// При обновлении выбранной аммуниции
        /// </summary>
        /// <param name="index"></param>
        private void OnTurretUpdateSelectedAmmunition(int index)
        {
            allAmmunitionElements[lastSelectedAmmunitionIndex].UnSelect();
            allAmmunitionElements[index].Select();

            lastSelectedAmmunitionIndex = index;
        }

        /// <summary>
        /// При изменении количества
        /// </summary>
        /// <param name="count">Количество</param>
        private void OnAmmoCountChanged(int count)
        {
            allAmmunitionElements[turret.SelectedAmmunitionIndex].UpdateAmmoCount(count);
        }
    }
}