namespace HIFUCommandoTweaks.Misc
{
    public class BloomAndSpread : MiscBase
    {
        public override string Name => ":: Misc : Bloom and Spread";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            On.EntityStates.Commando.CommandoWeapon.FirePistol2.OnEnter += FirePistol2_OnEnter;
        }

        private static void FirePistol2_OnEnter(On.EntityStates.Commando.CommandoWeapon.FirePistol2.orig_OnEnter orig, EntityStates.Commando.CommandoWeapon.FirePistol2 self)
        {
            EntityStates.Commando.CommandoWeapon.FirePistol2.spreadBloomValue = 0.275f;
            EntityStates.Commando.CommandoWeapon.FirePistol2.recoilAmplitude = 1.45f;
            orig(self);
        }
    }
}