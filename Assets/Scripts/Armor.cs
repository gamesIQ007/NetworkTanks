using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Тип брони
    /// </summary>
    public enum ArmorType
    {
        /// <summary>
        /// Транспорт
        /// </summary>
        Vehicle,
        /// <summary>
        /// Модули
        /// </summary>
        Module
    }

    /// <summary>
    /// Броня
    /// </summary>
    public class Armor : MonoBehaviour
    {
        /// <summary>
        /// Тип брони
        /// </summary>
        [SerializeField] private ArmorType type;
        public ArmorType Type => type;

        /// <summary>
        /// Защищаемый дистрактибл
        /// </summary>
        [SerializeField] private Destructible destructible;
        public Destructible Destructible => destructible;

        /// <summary>
        /// Толщина
        /// </summary>
        [SerializeField] private int thickness;
        public int Thickness => thickness;


        /// <summary>
        /// Задать дестрактибл
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        public void SetDestrucible(Destructible destructible)
        {
            this.destructible = destructible;
        }
    }
}