using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение метки игрока в списке
    /// </summary>
    public class UIPlayerLable : MonoBehaviour
    {
        /// <summary>
        /// Текст с фрагами
        /// </summary>
        [SerializeField] private Text fragText;
        /// <summary>
        /// Текст с ником
        /// </summary>
        [SerializeField] private Text nicknameText;
        /// <summary>
        /// Цвет текста убитого
        /// </summary>
        [SerializeField] private Color deadNicknameTextColor;

        /// <summary>
        /// Фоновое изображение
        /// </summary>
        [SerializeField] private Image backgroundImage;

        /// <summary>
        /// Цвет своего изображения
        /// </summary>
        [SerializeField] private Color selfColor;

        /// <summary>
        /// Сетевой ID
        /// </summary>
        private int netID;
        public int NetID => netID;


        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="netId">Сетевой ID</param>
        /// <param name="nickname">Ник</param>
        public void Init(int netId, string nickname)
        {
            netID = netId;
            nicknameText.text = nickname;

            if (netId == Player.Local.netId)
            {
                backgroundImage.color = selfColor;
            }
        }

        /// <summary>
        /// Обновить количество фрагов
        /// </summary>
        /// <param name="frag">Количество фрагов</param>
        public void UpdateFrag(int frag)
        {
            fragText.text = frag.ToString();
        }

        /// <summary>
        /// Обновить ник
        /// </summary>
        public void UpdateNickname()
        {
            nicknameText.color = deadNicknameTextColor;
        }
    }
}