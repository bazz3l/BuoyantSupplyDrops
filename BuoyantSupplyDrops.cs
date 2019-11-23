using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Buoyant Supply Drops", "Bazz3l", "1.0.0")]
    [Description("Allows supply drops to float on water")]
    class BuoyantSupplyDrops : RustPlugin
    {
        void OnEntitySpawned(SupplyDrop supply)
        {
            if (supply == null) return;
            BaseEntity entity    = supply as BaseEntity;
            MakeBuoyant sfloat   = entity.gameObject.AddComponent<MakeBuoyant>();
            sfloat.entity        = entity;
            sfloat.buoyancyScale = 1f;
            sfloat.detectionRate = 1;
        }

        class MakeBuoyant : MonoBehaviour
        {
            public BaseEntity entity;
            public float buoyancyScale;
            public int detectionRate;

            void FixedUpdate()
            {
                if (UnityEngine.Time.frameCount % detectionRate == 0 && WaterLevel.Factor(entity.WorldSpaceBounds().ToBounds()) > 0.65f)
                {
                    SupplyDrop supply = GetComponent<SupplyDrop>();
                    supply.RemoveParachute();
                    supply.MakeLootable();
                    BuoyancyComponent();
                    Destroy(this);
                }
            }

            void BuoyancyComponent()
            {
                Buoyancy buoyancy                  = entity.gameObject.AddComponent<Buoyancy>();
                buoyancy.buoyancyScale             = buoyancyScale;
                buoyancy.rigidBody                 = entity.gameObject.GetComponent<Rigidbody>();
                buoyancy.rigidBody.velocity        = Vector3.zero;
                buoyancy.rigidBody.angularVelocity = Vector3.zero;
                buoyancy.rigidBody.constraints     = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;

            }
        }
    }
}
