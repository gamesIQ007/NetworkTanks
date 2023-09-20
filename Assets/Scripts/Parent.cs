using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Задать родителя
    /// </summary>
    public class Parent : MonoBehaviour
    {
        /// <summary>
        /// Родитель
        /// </summary>
        [SerializeField] private Transform parent;


        private void Awake()
        {
            transform.SetParent(parent);
        }
    }
}