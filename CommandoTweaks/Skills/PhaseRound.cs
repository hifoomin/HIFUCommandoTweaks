using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUCommandoTweaks.Skills
{
    public class PhaseRound : TweakBase
    {
        public static float Damage;
        public static float RampingDamage;
        public static float Size;

        public override string Name => ": Secondary : Phase Round";

        public override string SkillToken => "secondary";

        public override string DescText => "Fire a <style=cIsDamage>piercing</style> bullet for <style=cIsDamage>" + d(Damage) + " damage</style>. Deals <style=cIsDamage>40%</style> more total damage for each enemy pierced.";

        public override void Init()
        {
            Damage = ConfigOption(4.5f, "Damage", "Decimal. Vanilla is 3");
            Size = ConfigOption(2f, "Size Multiplier", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Round1 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRamping.prefab").WaitForCompletion();
            var Round2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRampingGhost.prefab").WaitForCompletion();

            Round1.transform.localScale = new Vector3(Size, Size, Size);
            Round2.transform.localScale = new Vector3(Size, Size, Size);

            var Proj = Round1.GetComponent<ProjectileSimple>();
            Proj.desiredForwardSpeed = 200f;

            On.EntityStates.GenericProjectileBaseState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.Commando.CommandoWeapon.FireFMJ)
                {
                    self.damageCoefficient = Damage;
                    self.baseDuration = 0.25f;
                    self.force = 3000f;
                }
                orig(self);
            };
            Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFireFMJ.asset").WaitForCompletion().mustKeyPress = true;
        }
    }
}