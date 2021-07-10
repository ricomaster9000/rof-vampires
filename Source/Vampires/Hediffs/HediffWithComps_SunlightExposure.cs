using RimWorld;
using Verse;
using Verse.Sound;
using Verse.AI;

namespace Vampire
{
    public class HediffWithComps_SunlightExposure : HediffWithComps, ISizeReporter
    {
        private int curSunDamage = 2;
        private int checkRate = 110;
        public int ticksRemaining = GenDate.TicksPerHour;
        private Sustainer sustainer = null;
        private Effecter sunBurningEffect = null;

        public override void Tick()
        {
            base.Tick();

            if (pawn == null && pawn?.Corpse?.InnerPawn == null)
            {
                return;
            }


            if (sustainer != null && !sustainer.Ended)
            {
                sustainer.Maintain();
            }
            else
            {
                SoundDef def = SoundDef.Named("FireBurning");
                SoundInfo info = SoundInfo.InMap(new TargetInfo(pawn.Position, pawn.Map), MaintenanceType.PerTick);
                info.volumeFactor *= 2;
                sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, def, info);
            }

            if (sunBurningEffect != null)
            {
                sunBurningEffect.EffectTick(pawn, pawn);
                if (Find.TickManager.TicksGame % 20 == 0)
                {
                    if (CurStageIndex > 1 && Rand.Value > 0.5f) MoteMaker.ThrowSmoke(pawn.DrawPos, pawn.Map, 1f);
                    if (CurStageIndex > 1 && Rand.Value < CurStageIndex * 0.31f)
                        MoteMaker.ThrowFireGlow(pawn.PositionHeld, pawn.Map, 1f);
                }
            }
            if (Find.TickManager.TicksGame % checkRate == 0)
            {
                if (pawn?.PositionHeld is IntVec3 pos && pos != default(IntVec3) && pos.Roofed(pawn?.MapHeld ?? Find.CurrentMap) == false &&
                    VampireUtility.IsDaylight(pawn))
                {
                    if (sunBurningEffect == null)
                    {
                        EffecterDef effecterDef = EffecterDefOf.RoofWork;
                        if (effecterDef != null)
                        {
                            sunBurningEffect = effecterDef.Spawn();
                        }
                    }

                    if (CurStageIndex > 1)
                    {
                        Burn();
                    }

                    if(pawn.VampComp().Generation == VampireUtility.GETThirdGenerationLevelForVampires)
                        Severity += 0.001f;
                    else if(pawn.VampComp().Generation <= VampireUtility.GETHighestGenerationForLegendaryVampires)
                        Severity += 0.002f;
                    else if(pawn.VampComp().Generation <= VampireUtility.GETHighestGenerationForGreaterVampires)
                        Severity += 0.006f;
                    else if(pawn.VampComp().Generation <= VampireUtility.GETHighestGenerationForAverageVampires)
                        Severity += 0.010f;
                    else if(pawn.VampComp().Generation <= VampireUtility.GETHighestGenerationForAverageVampires)
                        Severity += 0.015f;
                    else if(pawn.VampComp().Generation <= VampireUtility.GETHighestGenerationForLesserVampires)
                        Severity += 0.020f;
                }
                else
                {
                    curSunDamage = 5;
                    Severity -= 0.2f;
                    if (sunBurningEffect != null) sunBurningEffect = null;
                    if (pawn?.MentalStateDef == VampDefOf.ROMV_Rotschreck) { pawn.MentalState.RecoverFromState();  }
                    if (pawn?.CurJob?.def == VampDefOf.ROMV_DigAndHide) pawn.jobs.StopAll();
                }
            }
        }

        public void Burn()
        {
            if (pawn != null)
            {                
                if (!pawn.Dead)
                {
                    int dmgRange = curSunDamage;
                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Burn, Rand.Range(1, curSunDamage));
                    dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                    if (CurStageIndex > 2)
                        curSunDamage += Rand.Range(1, 2);

                    ApplyBurnDamage(dinfo);
                    if (pawn.Dead)
                    {
                        RotCorpseAway();
                    }
                }
                else
                {
                    RotCorpseAway();
                }

            }
        }

        public void ApplyBurnDamage(DamageInfo dinfo)
        {
            pawn.TakeDamage(dinfo);
            if (pawn?.mindState?.mentalStateHandler is MentalStateHandler h)
            {
                if (pawn.InMentalState && pawn.MentalStateDef != VampDefOf.ROMV_Rotschreck)
                {
                    h.CurState.RecoverFromState();
                }
                if (CurStageIndex > 1 && Rand.Value < Severity && !pawn.InMentalState)
                {
                    h.TryStartMentalState(VampDefOf.ROMV_Rotschreck);
                }
            }
        }

        public void RotCorpseAway()
        {

            if (pawn?.Corpse?.GetComp<CompRottable>() is CompRottable r)
            {
                pawn.Corpse.GetComp<CompRottable>().RotProgress = 999999999f;
            }
        }

        public float CurrentSize()
        {
            switch (CurStageIndex)
            {
                case 1:
                    return 8;
                case 2:
                    return 14;
                case 3:
                    return 20;
                case 4:
                    return 30;
                default:
                    return 0;
            }
        }
    }
}
