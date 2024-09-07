using RoR2.Projectile;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using System.Collections;
using R2API.Utils;

namespace HIFUCommandoTweaks.Skills
{
    public class FragGrenade : TweakBase
    {
        public static float Damage;
        public static float AoE;
        public static bool RemoveFalloff;
        public static bool Ignite;
        public static bool NewFeedback;

        public override string Name => ": Special :: Frag Grenade";

        public override string SkillToken => "special_alt1";

        public override string DescText => (Ignite ? "<style=cIsDamage>Ignite</style>. " : "") + "Throw a grenade that explodes for <style=cIsDamage>" + d(Damage) + " damage</style>. Can hold up to 2.";

        public override void Init()
        {
            Damage = ConfigOption(6f, "Damage", "Decimal. Vanilla is 7");
            AoE = ConfigOption(11f, "Area of Effect", "Vanilla is 11");
            RemoveFalloff = ConfigOption(true, "Remove Damage Falloff?", "Vanilla is false");
            Ignite = ConfigOption(true, "Ignite?", "Vanilla is false");
            NewFeedback = ConfigOption(true, "Use new SFX and VFX?", "Vanilla is false");
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

            if (NewFeedback)
            {
                var effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
                effect.AddComponent<GrenadeSFXSexerController>();

                var trans = effect.transform;
                var scaledHitSparks1 = trans.GetChild(0);
                scaledHitSparks1.localScale = Vector3.one * (AoE / 4.4f);

                var scaledHitSparks1Em = scaledHitSparks1.GetComponent<ParticleSystem>().emission;
                var iLoveUnity = scaledHitSparks1Em.GetBurst(0);
                var unityIsSoGood = iLoveUnity.count;
                unityIsSoGood.constant = 6;

                var novaSphere1 = trans.Find("Nova Sphere");
                novaSphere1.localScale = Vector3.one * (AoE / 4f);

                var novaSphere1PS = novaSphere1.GetComponent<ParticleSystem>();
                var unityyy = novaSphere1PS.main;
                var fuckingUnity = unityyy.startLifetime;
                fuckingUnity.constant = 0.15f;
                /*
                var dudeUnityIsdfipjgoijsdgf = novaSphere1PS.colorOverLifetime;
                var fuckingaaaa = dudeUnityIsdfipjgoijsdgf.color;
                var PAIBODFIOFSDIFO = fuckingaaaa.colorMax;
                PAIBODFIOFSDIFO.a = 1f;
                */
                var imGonnaBreakMyMonitor = novaSphere1PS.sizeOverLifetime;
                imGonnaBreakMyMonitor.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.66f, 1f), new Keyframe(1f, 0f)));

                var novaSphere1PSR = novaSphere1.GetComponent<ParticleSystemRenderer>();

                var newMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matCryoCanisterSphere.mat").WaitForCompletion());
                newMat.SetColor("_TintColor", new Color32(255, 88, 0, 255));
                newMat.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/VoidSurvivor/texRampVoidSurvivorCorrupted3.png").WaitForCompletion());
                newMat.SetFloat("_InvFade", 0.06039937f);
                newMat.SetFloat("_SoftPower", 0.2511877f);
                newMat.SetFloat("_Boost", 0.3f);
                newMat.SetFloat("_RimPower", 2.720645f);
                newMat.SetFloat("_RimStrength", 2.668566f);

                novaSphere1PSR.material = newMat;

                var novaSphere2 = trans.Find("Nova Sphere (1)");
                novaSphere2.localScale = Vector3.one * (AoE / 3f);
                novaSphere2.GetComponent<ParticleSystemRenderer>().material = newMat;
                novaSphere2.gameObject.SetActive(false);

                var pointLight = trans.Find("Point Light").GetComponent<Light>();
                pointLight.color = new Color32(255, 153, 0, 255);
                pointLight.range = AoE * 3f / 4f;
                pointLight.GetComponent<LightIntensityCurve>().timeMax = 0.5f;
                pointLight.intensity = 150f;

                var dashBright = trans.Find("Dash, Bright");
                dashBright.localScale = Vector3.one * (AoE / 3.67f);

                var unscaledFlames = trans.Find("Unscaled Flames").GetComponent<ParticleSystem>().main;
                var startSize = unscaledFlames.startSize;
                startSize.constantMin = AoE;
                startSize.constantMax = AoE * 1.3f;

                var ghost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoGrenadeGhost.prefab").WaitForCompletion();
                var transform = ghost.transform;
                var nadePointLight = transform.Find("Point Light").GetComponent<Light>();
                nadePointLight.range = AoE;
                nadePointLight.color = new Color32(53, 82, 146, 255);

                var model = transform.Find("mdlCommandoGrenade").GetComponent<ObjectScaleCurve>();
                model.baseScale = Vector3.one * 3f;

                var pulse = transform.Find("Pulse");
                pulse.localScale = Vector3.one * 1.5f;

                var trail = transform.Find("Helix").GetChild(1).GetComponent<TrailRenderer>();
                trail.widthMultiplier = 0.2f;
                trail.time = 0.5f;

                var newTrailMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressArrowTrail.mat").WaitForCompletion());
                newTrailMat.SetFloat("_Boost", 5.6f);

                trail.material = newTrailMat;
            }
        }
    }

    public class GrenadeSFXSexerController : MonoBehaviour
    {
        public void OnEnable()
        {
            // Main.HCTLogger.LogError("playing sound");
            Util.PlaySound("Play_bleedOnCritAndExplode_explode", gameObject);
            Util.PlaySound("Play_bleedOnCritAndExplode_explode", gameObject);
            Util.PlaySound("Play_clayboss_M1_explo", gameObject);
            Util.PlaySound("Play_captain_m1_hit", gameObject);
            StartCoroutine(FireAftershock());
        }

        public IEnumerator FireAftershock()
        {
            if (!FragGrenade.Ignite)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
            Util.PlaySound("Play_lemurianBruiser_m2_loop", gameObject);
            yield return new WaitForSeconds(0.3f);
            Util.PlaySound("Stop_lemurianBruiser_m2_loop", gameObject);
        }
    }
}