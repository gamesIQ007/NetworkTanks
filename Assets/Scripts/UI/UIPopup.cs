using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Всплывающее сообщение
    /// </summary>
    public class UIPopup : MonoBehaviour
    {
        /// <summary>
        /// Направление движения
        /// </summary>
        [SerializeField] private Vector2 movementDirection;

        /// <summary>
        /// Скорость движения
        /// </summary>
        [SerializeField] private float movementSpeed;

        /// <summary>
        /// Время жизни
        /// </summary>
        [SerializeField] private float lifeTime;


        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
        }
    }
}