using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace HIFUCommandoTweaks.Skills
{
    public class FragGrenade : TweakBase
    {
        public static float Damage;
        public static float AoE;

        public override string Name => ": Special :: Frag Grenade";

        public override string SkillToken => "special_alt1";

        public override string DescText => "Throw a grenade that explodes for <style=cIsDamage>" + d(Damage) + " damage</style>. Can hold up to 2.";

        public override void Init()
        {
            Damage = ConfigOption(7f, "Damage", "Decimal. Vanilla is 7");
            AoE = ConfigOption(14f, "Area of Effect", "Vanilla is 11");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Grenade = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/CommandoGrenadeProjectile");
            Grenade.GetComponent<ProjectileImpactExplosion>().blastRadius = AoE;
            On.EntityStates.GenericProjectileBaseState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.Commando.CommandoWeapon.ThrowGrenade)
                {
                    self.damageCoefficient = Damage;
                }
                orig(self);
            };
        }
    }
}