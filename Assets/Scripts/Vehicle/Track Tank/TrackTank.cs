using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    [System.Serializable]
    /// <summary>
    /// Колёсная ось
    /// </summary>
    public class TrackWheelRow
    {
        /// <summary>
        /// Коллайдеры колёс
        /// </summary>
        [SerializeField] private WheelCollider[] colliders;

        /// <summary>
        /// Меши колёс
        /// </summary>
        [SerializeField] private Transform[] meshs;

        /// <summary>
        /// Минимальное вращение колёсного ряда
        /// </summary>
        public float MinRpm;

        
        #region Public

        /// <summary>
        /// Задать крутящий момент
        /// </summary>
        /// <param name="motorTorque">Крутящий момент</param>
        public void SetTorque(float motorTorque)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].motorTorque = motorTorque;
            }
        }

        /// <summary>
        /// Торможение
        /// </summary>
        /// <param name="breakTorque">Сила торможения</param>
        public void Brake(float brakeTorque)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].brakeTorque = brakeTorque;
            }
        }

        /// <summary>
        /// Сброс
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].motorTorque = 0;
                colliders[i].brakeTorque = 0;
            }
        }

        /// <summary>
        /// Задать боковое скольжение
        /// </summary>
        /// <param name="stiffness">Величина скольжения</param>
        public void SetSidewaysStiffness(float stiffness)
        {
            WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

            for (int i = 0; i < colliders.Length; i++)
            {
                wheelFrictionCurve = colliders[i].sidewaysFriction;
                wheelFrictionCurve.stiffness = stiffness;

                colliders[i].sidewaysFriction = wheelFrictionCurve;
            }
        }

        /// <summary>
        /// Синхронизация поворота колеса и его меша
        /// </summary>
        public void UpdateMeshTransform()
        {
            // Поиск минимального вращения колёсного ряда
            List<float> allRpm = new List<float>();

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].isGrounded)
                {
                    allRpm.Add(colliders[i].rpm);
                }
            }

            if (allRpm.Count > 0)
            {
                MinRpm = Mathf.Abs(allRpm[0]);
                for (int i = 0; i < allRpm.Count; i++)
                {
                    if (MinRpm > Mathf.Abs(allRpm[i]))
                    {
                        MinRpm = Mathf.Abs(allRpm[i]);
                    }
                }
                MinRpm *= Mathf.Sign(allRpm[0]);
            }

            float angle = MinRpm * 360.0f / 60.0f * Time.fixedDeltaTime;

            for (int i = 0; i < meshs.Length; i++)
            {
                Vector3 position;
                Quaternion rotation;

                colliders[i].GetWorldPose(out position, out rotation);

                meshs[i].position = position;
                meshs[i].Rotate(angle, 0, 0);
            }
        }

        /// <summary>
        /// Обновить поворот мешей в зависимости от вращения
        /// </summary>
        /// <param name="rpm">Вращение</param>
        public void UpdateMeshRotationByRpm(float rpm)
        {
            float angle = rpm * 360.0f / 60.0f * Time.fixedDeltaTime;

            for (int i = 0; i < meshs.Length; i++)
            {
                Vector3 position;
                Quaternion rotation;

                colliders[i].GetWorldPose(out position, out rotation);

                meshs[i].position = position;
                meshs[i].Rotate(angle, 0, 0);
            }
        }

        #endregion
    }


    [RequireComponent(typeof(Rigidbody))]

    /// <summary>
    /// Гусеничный танк
    /// </summary>
    public class TrackTank : Vehicle
    {
        /// <summary>
        /// Линейная скорость
        /// </summary>
        public override float LinearVelocity => rigidbody.velocity.magnitude;


        /// <summary>
        /// Визуальная модель
        /// </summary>
        [SerializeField] private GameObject visualModel;
        /// <summary>
                                                            /// Префаб уничтоженного танка
                                                            /// </summary>
        [SerializeField] private GameObject destroyedPrefab;

        /// <summary>
        /// Центр масс
        /// </summary>
        [SerializeField] private Transform centerOfMass;

        [Header("Tracks")]
        /// <summary>
        /// Левый ряд колёс
        /// </summary>
        [SerializeField] private TrackWheelRow leftWheelRow;
        /// <summary>
        /// Правый ряд колёс
        /// </summary>
        [SerializeField] private TrackWheelRow rightWheelRow;
        
        [Header("Movement")]
        /// <summary>
        /// Кривая крутящего момента вперёд
        /// </summary>
        [SerializeField] private ParameterCurve forwardTorqueCurve;
        /// <summary>
        /// Максимальный крутящий момент вперёд
        /// </summary>
        [SerializeField] private float maxForwardTorque;
        /// <summary>
        /// Кривая крутящего момента назад
        /// </summary>
        [SerializeField] private ParameterCurve backwardTorqueCurve;
        /// <summary>
        /// Максимальный крутящий момент назад
        /// </summary>
        [SerializeField] private float maxBackwardTorque;
        /// <summary>
        /// Торможение
        /// </summary>
        [SerializeField] private float brakeTorque;
        /// <summary>
        /// Сопротивление качению
        /// </summary>
        [SerializeField] private float rollingResistance;
        
        [Header("Rotation")]
        /// <summary>
        /// Вращение на месте
        /// </summary>
        [SerializeField] private float rotateTorqueInPlace;
        /// <summary>
        /// Торможение на месте
        /// </summary>
        [SerializeField] private float rotateBrakeInPlace;
        [Space(2)]
        /// <summary>
        /// Вращение в движении
        /// </summary>
        [SerializeField] private float rotateTorqueInMotion;
        /// <summary>
        /// Торможение в движении
        /// </summary>
        [SerializeField] private float rotateBrakeInMotion;

        [Header("Stiffness")]
        [SerializeField] private float minSidewayStiffnessInPlace;
        [SerializeField] private float minSidewayStiffnessInMotion;

        private new Rigidbody rigidbody;

        /// <summary>
        /// Текущий крутящий момент
        /// </summary>
        [SerializeField] private float currentMotorTorque;

        /// <summary>
        /// Минимальное вращение левого колёсного ряда
        /// </summary>
        public float LeftWheelRpm => leftWheelRow.MinRpm;
        /// <summary>
        /// Минимальное вращение правого колёсного ряда
        /// </summary>
        public float RightWheelRpm => rightWheelRow.MinRpm;


        #region Unity Events

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.centerOfMass = centerOfMass.localPosition;
            Destroyed += OnTrackTankDestroyed;
        }

        private void OnDestroy()
        {
            Destroyed -= OnTrackTankDestroyed;
        }

        private void FixedUpdate()
        {
            if (isOwned)
            {
                UpdateMotorTorque();

                CmdUpdateWheelRpm(LeftWheelRpm, RightWheelRpm);
            }
        }

        #endregion


        /// <summary>
        /// При уничтожении
        /// </summary>
        /// <param name="arg0">Дестрактибл</param>
        private void OnTrackTankDestroyed(Destructible arg0)
        {
            GameObject ruinedVisualModel = Instantiate(destroyedPrefab);
            ruinedVisualModel.transform.position = visualModel.transform.position;
            ruinedVisualModel.transform.rotation = visualModel.transform.rotation;
        }


        /// <summary>
        /// Обновить вращение колёс
        /// </summary>
        /// <param name="leftRpm">Левый ряд</param>
        /// <param name="rightRpm">Правый ряд</param>
        [Command]
        private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            SvUpdateWheelRpm(leftRpm, rightRpm);
        }
        [Server]
        private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            RpcUpdateWheelRpm(leftRpm, rightRpm);
        }
        [ClientRpc(includeOwner = false)]
        private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            leftWheelRow.MinRpm = leftRpm;
            rightWheelRow.MinRpm = rightRpm;

            leftWheelRow.UpdateMeshRotationByRpm(leftRpm);
            rightWheelRow.UpdateMeshRotationByRpm(rightRpm);
        }


        private void UpdateMotorTorque()
        {
            float targetMotorTorque = targetInputControl.z > 0 ? maxForwardTorque * Mathf.RoundToInt(targetInputControl.z) : maxBackwardTorque * Mathf.RoundToInt(targetInputControl.z);
            float brakeTorque = this.brakeTorque * targetInputControl.y;
            float steering = targetInputControl.x;

            // Обновление крутящего момента
            if (targetMotorTorque > 0)
            {
                currentMotorTorque = forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
            }
            if (targetMotorTorque < 0)
            {
                currentMotorTorque = backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
            }
            if (targetMotorTorque == 0)
            {
                currentMotorTorque = forwardTorqueCurve.Reset();
                currentMotorTorque = backwardTorqueCurve.Reset();
            }

            // Тормозное усилие
            leftWheelRow.Brake(brakeTorque);
            rightWheelRow.Brake(brakeTorque);

            // Качение
            if (targetMotorTorque == 0 && steering == 0)
            {
                leftWheelRow.Brake(rollingResistance);
                rightWheelRow.Brake(rollingResistance);
            }
            else
            {
                leftWheelRow.Reset();
                rightWheelRow.Reset();
            }

            // Поворот на месте
            if (targetMotorTorque == 0 && steering != 0)
            {
                if (Mathf.Abs(leftWheelRow.MinRpm) < 1 || Mathf.Abs(rightWheelRow.MinRpm) < 1)
                {
                    leftWheelRow.SetTorque(rotateTorqueInPlace);
                    rightWheelRow.SetTorque(rotateTorqueInPlace);
                }
                else
                {
                    if (steering < 0)
                    {
                        leftWheelRow.Brake(rotateBrakeInPlace);
                        rightWheelRow.SetTorque(rotateTorqueInPlace);
                    }

                    if (steering > 0)
                    {
                        leftWheelRow.SetTorque(rotateTorqueInPlace);
                        rightWheelRow.Brake(rotateBrakeInPlace);
                    }
                }

                leftWheelRow.SetSidewaysStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
                rightWheelRow.SetSidewaysStiffness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
            }

            // Движение
            if (targetMotorTorque != 0)
            {
                if (steering == 0)
                {
                    if (LinearVelocity < maxLinearVelocity)
                    {
                        leftWheelRow.SetTorque(currentMotorTorque);
                        rightWheelRow.SetTorque(currentMotorTorque);
                    }
                }

                if (steering != 0 && (Mathf.Abs(leftWheelRow.MinRpm) < 1 || Mathf.Abs(rightWheelRow.MinRpm) < 1))
                {
                    leftWheelRow.SetTorque(rotateTorqueInMotion);
                    rightWheelRow.SetTorque(rotateTorqueInMotion);
                }
                else
                {
                    if (steering < 0)
                    {
                        leftWheelRow.Brake(rotateBrakeInMotion);
                        rightWheelRow.SetTorque(rotateTorqueInMotion);
                    }

                    if (steering > 0)
                    {
                        leftWheelRow.SetTorque(rotateTorqueInMotion);
                        rightWheelRow.Brake(rotateBrakeInMotion);
                    }
                }

                leftWheelRow.SetSidewaysStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
                rightWheelRow.SetSidewaysStiffness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
            }

            leftWheelRow.UpdateMeshTransform();
            rightWheelRow.UpdateMeshTransform();
        }
    }
}