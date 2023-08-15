using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Управление транспортом
    /// </summary>
    public class VehicleInputControl : MonoBehaviour
    {
        /// <summary>
        /// Транспорт
        /// </summary>
        [SerializeField] private Vehicle vehicle;


        protected virtual void Update()
        {
            vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
        }
    }
}