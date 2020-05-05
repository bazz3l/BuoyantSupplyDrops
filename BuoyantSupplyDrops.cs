using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Buoyant Supply Drops", "Bazz3l", "1.0.3")]
    [Description("Allows supply drops to float on water.")]
    class BuoyantSupplyDrops : RustPlugin
    {
        #region Fields
        PluginConfig _config;
        #endregion

        #region Config
        PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                DetectionRate = 5,
            };
        }

        class PluginConfig
        {
            public int DetectionRate;
        }
        #endregion

        #region Oxide
        protected override void LoadDefaultConfig() => Config.WriteObject(GetDefaultConfig(), true);

        void Init()
        {
            _config = Config.ReadObject<PluginConfig>();
        }

        void OnEntitySpawned(SupplyDrop supply)
        {
            BuoyantComponent sfloat = supply.gameObject.AddComponent<BuoyantComponent>();
            sfloat.BuoyancyScale = 1f;
            sfloat.DetectionRate = _config.DetectionRate;
        }
        #endregion

        #region Classes
        class BuoyantComponent : MonoBehaviour
        {
            public float BuoyancyScale;
            public int DetectionRate;
            SupplyDrop _drop;

            void Awake()
            {
                _drop = GetComponent<SupplyDrop>();
                if (_drop == null)
                {
                    Destroy(this);
                    return;
                }
            }

            void FixedUpdate()
            {
                if (_drop != null && UnityEngine.Time.frameCount % DetectionRate == 0 && WaterLevel.Factor(_drop.WorldSpaceBounds().ToBounds()) > 0.65f)
                {
                    _drop.RemoveParachute();
                    _drop.MakeLootable();

                    BuoyancyComponent();

                    Destroy(this);
                }
            }

            void BuoyancyComponent()
            {
                Buoyancy buoyancy = gameObject.AddComponent<Buoyancy>();
                buoyancy.buoyancyScale = BuoyancyScale;
                buoyancy.rigidBody = gameObject.GetComponent<Rigidbody>();
                buoyancy.rigidBody.velocity = Vector3.zero;
                buoyancy.rigidBody.angularVelocity = Vector3.zero;
                buoyancy.rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;
            }
        }
        #endregion
    }
}
