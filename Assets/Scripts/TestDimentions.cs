using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Тест габаритов
    /// </summary>
    public class TestDimentions : MonoBehaviour
    {
        /// <summary>
        /// Габариты транспорта
        /// </summary>
        [SerializeField] private VehicleDimentions vehicleDimentions;


        private void Update()
        {
            Debug.Log(vehicleDimentions.IsVisibleFromPoint(transform.root, transform.position, Color.red));
        }
    }
}