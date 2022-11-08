using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Items.Scripts
{
    [CreateAssetMenu(menuName = "Create Items/Inventory Item", order = 1)]
    public abstract class ItemFunction : ScriptableObject
    {
        [Header("Basic Item Configuration")]
        [FormerlySerializedAs("useDelay")] public float useCooldown;

        public bool isChargeItem;
        [Range(0.0f, float.PositiveInfinity)]
        public float chargeTime;
        public bool autoUse;
        public float damage;
        public AudioSource useSound;

        public async void Use(KeyCode key, LivingEntity parent, Rigidbody2D parentRigidBody, bool singlePress)
        {
            if (!parent.CanUseItem() || parent.IsUsingItem() || parent.IsCharging() || (!autoUse && !singlePress))
                return;

            switch (key)
            {
                case KeyCode.Mouse0:
                    parent.SetUsingItem(true);
                    PlayUseSound(parent.gameObject);
                    await LeftUse(parent, parentRigidBody);
                    ResetItemUseInSeconds(parent, useCooldown);
                    parent.SetUsingItem(false);
                    break;
                case KeyCode.Mouse1:
                    parent.SetUsingItem(true);
                    await RightUse(parent, parentRigidBody);
                    ResetItemUseInSeconds(parent, useCooldown);
                    parent.SetUsingItem(false);
                    break;
            }
            
        }
        
        private static async void ResetItemUseInSeconds(LivingEntity parent, float seconds)
        {
            parent.SetCanUseItem(false);
            await Task.Delay((int) (seconds * 1000));
            parent.SetCanUseItem(true);
        }

        private void PlayUseSound(GameObject parent)
        {
            if (useSound)
            {
                var sfx = Instantiate(useSound);
                sfx.transform.position = parent.transform.position;
                sfx.PlayOneShot(useSound.clip, 1);
            }
        }

        protected abstract Task LeftUse(LivingEntity parent, Rigidbody2D parentRigidBody);

        protected abstract Task RightUse(LivingEntity parent, Rigidbody2D parentRigidBody);
    }

    public static class ItemExtensions
    {
        public static void SetCanUseItem(this LivingEntity entity, bool value)
        {
            entity.SetItemUseData(ItemUseData.CanUseItem, value);
        }
        
        public static bool CanUseItem(this LivingEntity entity)
        {
            var output = entity.GetItemUseDate(ItemUseData.CanUseItem);
            if (output is bool b)
                return b;

            entity.SetCanUseItem(true);
            return true;
        }
        
        public static void SetUsingItem(this LivingEntity entity, bool value)
        {
            entity.SetItemUseData(ItemUseData.IsUsingItem, value);
        }
        
        public static bool IsUsingItem(this LivingEntity entity)
        {
            var output = entity.GetItemUseDate(ItemUseData.IsUsingItem);
            if (output is bool b)
                return b;
            
            entity.SetUsingItem(false);
            return false;
        }
        
        public static void SetCharging(this LivingEntity entity, bool value)
        {
            entity.SetItemUseData(ItemUseData.IsCharging, value);
        }

        public static bool IsCharging(this LivingEntity entity)
        {
            var output = entity.GetItemUseDate(ItemUseData.IsCharging);
            if (output is bool b)
                return b;
            
            entity.SetCharging(false);
            return false;
        }
    }

    public enum ItemUseData
    {
        CanUseItem,
        IsUsingItem,
        IsCharging,
        ItemChargeAmount,
    }
}
