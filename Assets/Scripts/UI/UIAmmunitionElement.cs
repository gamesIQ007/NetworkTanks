using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Элемент панели аммуниции
    /// </summary>
    public class UIAmmunitionElement : MonoBehaviour
    {
        /// <summary>
        /// Текст с количеством патронов
        /// </summary>
        [SerializeField] private Text ammoCountText;
        /// <summary>
        /// Иконка снаряда
        /// </summary>
        [SerializeField] private Image projectileIco;
        /// <summary>
        /// Рамка выбора
        /// </summary>
        [SerializeField] private GameObject selectionBorder;


        /// <summary>
        /// Задать аммуницию
        /// </summary>
        /// <param name="ammunition">Аммуниция</param>
        public void SetAmmunition(Ammunition ammunition)
        {
            projectileIco.sprite = ammunition.Properties.Icon;

            UpdateAmmoCount(ammunition.AmmoCount);
        }

        /// <summary>
        /// Обновить количество патронов
        /// </summary>
        /// <param name="count"></param>
        public void UpdateAmmoCount(int count)
        {
            ammoCountText.text = count.ToString();
        }

        /// <summary>
        /// Выбрать
        /// </summary>
        public void Select()
        {
            selectionBorder.SetActive(true);
        }

        /// <summary>
        /// Снять выбор
        /// </summary>
        public void UnSelect()
        {
            selectionBorder.SetActive(false);
        }
    }
}