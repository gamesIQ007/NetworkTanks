using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Панель с результатами попадания
    /// </summary>
    public class UIHitResultPanel : MonoBehaviour
    {
        /// <summary>
        /// Объект, на котором будут спавниться всплывашки
        /// </summary>
        [SerializeField] private Transform spawnPanel;

        /// <summary>
        /// Префаб всплывающего сообщения
        /// </summary>
        [SerializeField] private UIHitResultPopup hitResultPopup;

        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnStartMatch;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnStartMatch;
            }
            if (Player.Local != null)
            {
                Player.Local.ProjectileHit -= OnProjectileHit;
            }
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnStartMatch()
        {
            Player.Local.ProjectileHit += OnProjectileHit;
        }

        /// <summary>
        /// При попадении снаряда
        /// </summary>
        /// <param name="hitResult">Результат попадания</param>
        private void OnProjectileHit(ProjectileHitResult hitResult)
        {
            if (hitResult.Type == ProjectileHitType.Environment || hitResult.Type == ProjectileHitType.ModulePenetration || hitResult.Type == ProjectileHitType.ModuleNoPenetration) return;

            UIHitResultPopup hitPopup = Instantiate(hitResultPopup);
            hitPopup.transform.SetParent(spawnPanel);
            hitPopup.transform.localScale = Vector3.one;
            hitPopup.transform.position = Camera.main.WorldToScreenPoint(hitResult.Point);

            if (hitResult.Type == ProjectileHitType.Penetration)
            {
                hitPopup.SetTypeResult("Пробил");
            }
            if (hitResult.Type == ProjectileHitType.Nopenetration)
            {
                hitPopup.SetTypeResult("Не пробил");
            }
            if (hitResult.Type == ProjectileHitType.Ricochet)
            {
                hitPopup.SetTypeResult("Рикошет");
            }
            hitPopup.SetDamageResult(hitResult.Damage);
        }
    }
}