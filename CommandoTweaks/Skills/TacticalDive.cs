using RoR2;
using RoR2.Skills;

namespace HIFUCommandoTweaks.Skills
{
    public class TacticalDive : TweakBase
    {
        public static float Cooldown;
        public static float InitialSpeedCoefficient;
        public static float EndSpeedCoefficient;
        public static int Charges;
        public static int ChargesToRecharge;
        public static bool ArmorBuff;
        public static float ArmorBuffDuration;

        public override string Name => ": Utility : Tactical Dive";

        public override string SkillToken => "utility";

        public override string DescText => "<style=cIsUtility>Roll</style> a" +
                                           (InitialSpeedCoefficient >= 7f || EndSpeedCoefficient >= 7f ? " long" : (InitialSpeedCoefficient >= 4f || EndSpeedCoefficient >= 4f ? " medium" : " short")) +
                                           " distance." +
                                           (Charges > 1 ? " Can store up to <style=cIsUtility>" + Charges + "</style> charges." : "");

        public override void Init()
        {
            Charges = ConfigOption(1, "Charges", "Vanilla is 1");
            ChargesToRecharge = ConfigOption(1, "Charges to Recharge", "Vanilla is 1");
            Cooldown = ConfigOption(4f, "Cooldown", "Vanilla is 4");
            InitialSpeedCoefficient = ConfigOption(5f, "Initial Speed Coefficient", "Vanilla is 5");
            EndSpeedCoefficient = ConfigOption(2.65f, "End Speed Coefficient", "Vanilla is 2.5");
            ArmorBuff = ConfigOption(true, "Enable Armor Buff?", "Vanilla is false");
            ArmorBuffDuration = ConfigOption(0.5f, "Armor Buff Dur", "Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Commando.DodgeState.OnEnter += DodgeState_OnEnter;
            On.EntityStates.Commando.DodgeState.FixedUpdate += DodgeState_FixedUpdate;
            On.EntityStates.Commando.DodgeState.OnExit += DodgeState_OnExit;
        }

        private void DodgeState_OnExit(On.EntityStates.Commando.DodgeState.orig_OnExit orig, EntityStates.Commando.DodgeState self)
        {
            self.characterBody.isSprinting = true;
            orig(self);
        }

        private void DodgeState_FixedUpdate(On.EntityStates.Commando.DodgeState.orig_FixedUpdate orig, EntityStates.Commando.DodgeState self)
        {
            self.characterBody.isSprinting = true;
            orig(self);
        }

        private void DodgeState_OnEnter(On.EntityStates.Commando.DodgeState.orig_OnEnter orig, EntityStates.Commando.DodgeState self)
        {
            self.characterBody.isSprinting = true;
            orig(self);
        }

        public static void Changes()
        {
            On.EntityStates.Commando.DodgeState.OnEnter += (orig, self) =>
            {
                self.initialSpeedCoefficient = InitialSpeedCoefficient;
                self.finalSpeedCoefficient = EndSpeedCoefficient;
                if (ArmorBuff)
                {
                    self.characterBody.AddTimedBuffAuthority(RoR2Content.Buffs.ArmorBoost.buffIndex, ArmorBuffDuration);
                }
                orig(self);
            };
            var Roll = LegacyResourcesAPI.Load<SkillDef>("SkillDefs/CommandoBody/CommandoBodyRoll");
            Roll.baseMaxStock = Charges;
            Roll.rechargeStock = ChargesToRecharge;
            Roll.baseRechargeInterval = Cooldown;
            Roll.cancelSprintingOnActivation = false;
        }
    }
}