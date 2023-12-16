using RoR2.Projectile;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace HIFUCommandoTweaks.Skills
{
    public class FragGrenade : TweakBase
    {
        public static float Damage;
        public static float AoE;
        public static bool RemoveFalloff;
        public static bool Ignite;

        public override string Name => ": Special :: Frag Grenade";

        public override string SkillToken => "special_alt1";

        public override string DescText => (Ignite ? "<style=cIsDamage>Ignite</style>. " : "") + "Throw a grenade that explodes for <style=cIsDamage>" + d(Damage) + " damage</style>. Can hold up to 2.";

        public override void Init()
        {
            Damage = ConfigOption(6f, "Damage", "Decimal. Vanilla is 7");
            AoE = ConfigOption(11f, "Area of Effect", "Vanilla is 11");
            RemoveFalloff = ConfigOption(true, "Remove Damage Falloff?", "Vanilla is false");
            Ignite = ConfigOption(true, "Ignite?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.GenericProjectileBaseState.OnEnter += GenericProjectileBaseState_OnEnter;
        }

        private void GenericProjectileBaseState_OnEnter(On.EntityStates.GenericProjectileBaseState.orig_OnEnter orig, EntityStates.GenericProjectileBaseState self)
        {
            if (self is EntityStates.Commando.CommandoWeapon.ThrowGrenade)
            {
                self.damageCoefficient = Damage;
            }
            orig(self);
        }

        public static void Changes()
        {
            var grenade = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoGrenadeProjectile.prefab").WaitForCompletion();
            var projectileImpactExplosion = grenade.GetComponent<ProjectileImpactExplosion>();
            if (RemoveFalloff)
                projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.blastRadius = AoE;

            if (Ignite)
            {
                var projectileDamage = grenade.GetComponent<ProjectileDamage>();
                projectileDamage.damageType = DamageType.IgniteOnHit;

                var skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/ThrowGrenade.asset").WaitForCompletion();
                skillDef.keywordTokens = new string[] { "KEYWORD_IGNITE" };
            }
        }
    }
}