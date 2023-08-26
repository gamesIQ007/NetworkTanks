using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Точка синхронизации колеса
    /// </summary>
    [System.Serializable]
    public class WheelSyncPoint
    {
        /// <summary>
        /// Кость
        /// </summary>
        public Transform bone;
        /// <summary>
        /// Меш колеса
        /// </summary>
        public Transform mesh;
        /// <summary>
        /// Смещение
        /// </summary>
        [HideInInspector] public Vector3 offset;
    }

    /// <summary>
    /// Синхронизация траков с колёсами
    /// </summary>
    public class TrackSyncByWheel : MonoBehaviour
    {
        /// <summary>
        /// Точки синхронизации колёс
        /// </summary>
        [SerializeField] private WheelSyncPoint[] syncPoints;


        private void Start()
        {
            for (int i = 0; i < syncPoints.Length; i++)
            {
                syncPoints[i].offset = syncPoints[i].bone.localPosition - syncPoints[i].mesh.localPosition;
            }
        }

        private void Update()
        {
            for (int i = 0; i < syncPoints.Length; i++)
            {
                syncPoints[i].bone.localPosition = syncPoints[i].mesh.localPosition + syncPoints[i].offset;
            }
        }
    }
}