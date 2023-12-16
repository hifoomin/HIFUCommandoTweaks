using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine.AddressableAssets;

namespace HIFUCommandoTweaks.Skills
{
    public class SuppressiveFire : TweakBase
    {
        public static float Damage;
        public static float FireRate;
        public static float Cooldown;
        public static int ShotCount;
        public static bool Ignite;

        public override string Name => ": Special : Suppressive Fire";

        public override string SkillToken => "special";

        public override string DescText => "<style=cIsDamage>Stunning</style>. " + (Ignite ? "<style=cIsDamage>Ignite</style>." : "") + " Fire repeatedly for <style=cIsDamage>" + d(Damage) + " damage</style> per bullet. The number of shots increases with attack speed.";

        public override void Init()
        {
            Damage = ConfigOption(1.5f, "Damage", "Decimal. Vanilla is 1");
            FireRate = ConfigOption(0.1f, "Fire Rate", "Vanilla is 0.15");
            Cooldown = ConfigOption(7f, "Cooldown", "Vanilla is 9");
            ShotCount = ConfigOption(8, "Shot Count", "Vanilla is 6");
            Ignite = ConfigOption(true, "Ignite?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Commando.CommandoWeapon.FireBarrage.OnEnter += FireBarrage_OnEnter;
            if (Ignite)
                IL.EntityStates.Commando.CommandoWeapon.FireBarrage.FireBullet += FireBarrage_FireBullet;
        }

        private void FireBarrage_FireBullet(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(32)))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 160; // 32 + 128, stun + ignite
                });
            }
            else
            {
                Main.HCTLogger.LogError("Failed to apply Suppressive Fire Ignite hook");
            }
        }

        private void FireBarrage_OnEnter(On.EntityStates.Commando.CommandoWeapon.FireBarrage.orig_OnEnter orig, EntityStates.Commando.CommandoWeapon.FireBarrage self)
        {
            EntityStates.Commando.CommandoWeapon.FireBarrage.baseBulletCount = ShotCount;
            EntityStates.Commando.CommandoWeapon.FireBarrage.baseDurationBetweenShots = FireRate;
            EntityStates.Commando.CommandoWeapon.FireBarrage.damageCoefficient = Damage;
            orig(self);
        }

        public static void Changes()
        {
            var suppressive = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyBarrage.asset").WaitForCompletion();
            suppressive.baseRechargeInterval = Cooldown;
            if (Ignite)
                suppressive.keywordTokens = new string[] { "KEYWORD_STUNNING", "KEYWORD_IGNITE" };
        }
    }
}