using RoR2;
using RoR2.Skills;

namespace HCT.Skills
{
    public class SuppressiveFire : TweakBase
    {
        public static float Damage;
        public static float FireRate;
        public static float Cooldown;
        public static int ShotCount;

        public override string Name => ": Special : Suppressive Fire";

        public override string SkillToken => "special";

        public override string DescText => "<style=cIsDamage>Stunning</style>. Fire repeatedly for <style=cIsDamage>" + d(Damage) + " damage</style> per bullet. The number of shots increases with attack speed.";

        public override void Init()
        {
            Damage = ConfigOption(1.5f, "Damage", "Decimal. Vanilla is 1");
            FireRate = ConfigOption(0.1f, "Fire Rate", "Vanilla is 0.15");
            Cooldown = ConfigOption(7f, "Cooldown", "Vanilla is 9");
            ShotCount = ConfigOption(8, "Shot Count", "Vanilla is 6");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            On.EntityStates.Commando.CommandoWeapon.FireBarrage.OnEnter += (orig, self) =>
            {
                EntityStates.Commando.CommandoWeapon.FireBarrage.baseBulletCount = ShotCount;
                EntityStates.Commando.CommandoWeapon.FireBarrage.baseDurationBetweenShots = FireRate;
                EntityStates.Commando.CommandoWeapon.FireBarrage.damageCoefficient = Damage;
                orig(self);
            };
            var Supp = LegacyResourcesAPI.Load<SkillDef>("SkillDefs/CommandoBody/CommandoBodyBarrage");
            Supp.baseRechargeInterval = Cooldown;
        }
    }
}