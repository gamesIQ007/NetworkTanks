using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Метка танка на миникарте
    /// </summary>
    public class UITankMark : MonoBehaviour
    {
        /// <summary>
        /// Изображение значка
        /// </summary>
        [SerializeField] private Image image;

        /// <summary>
        /// Цвет своей команды
        /// </summary>
        [SerializeField] private Color localTeamColor;
        /// <summary>
        /// Цвет вражеской команды
        /// </summary>
        [SerializeField] private Color otherTeamColor;


        /// <summary>
        /// Задать цвет своей команды
        /// </summary>
        public void SetLocalColor()
        {
            image.color = localTeamColor;
        }

        /// <summary>
        /// Задать цвет вражеской команды
        /// </summary>
        public void SetOtherColor()
        {
            image.color = otherTeamColor;
        }
    }
}