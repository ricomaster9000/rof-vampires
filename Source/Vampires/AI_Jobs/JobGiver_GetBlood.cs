﻿using RimWorld;
using Verse;
using Verse.AI;

namespace Vampire
{
    public class JobGiver_GetBlood : ThinkNode_JobGiver
    {
        //private HungerCategory minCategory;

        //public override ThinkNode DeepCopy(bool resolve = true)
        //{
        //    JobGiver_GetBlood jobGiver_GetBlood = (JobGiver_GetBlood)base.DeepCopy(resolve);
        //    jobGiver_GetBlood.minCategory = this.minCategory;
        //    return jobGiver_GetBlood;
        //}

        public override float GetPriority(Pawn pawn)
        {
            //Log.Message("0");
            if (pawn.VampComp() == null)
            {
                //Log.Message("0a");
                return 0f;

            }
            if (!pawn.VampComp().IsVampire)
            {
                return 0f;
            }
            
            if (pawn.Drafted)
            {
                return 0f;
            }

            Need_Blood blood = pawn.needs.TryGetNeed<Need_Blood>();
            if (blood == null)
            {
                //Log.Message("0c");
                return 0f;

            }
            if (pawn.Faction == Faction.OfPlayer && blood.preferredFeedMode == PreferredFeedMode.None)
            {
                //Log.Message("0d");
                return 0f;
            }
            if (Find.World.GetComponent<WorldComponent_VampireTracker>().recentVampires?.ContainsKey(pawn) ?? false)
            {
                return 0f;
            }
            if (blood.CurLevelPercentage < blood.ShouldFeedPerc)
            {
                return 9.5f;
            }
            return 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            return FeedJob(pawn);
        }

        public static Job FeedJob(Pawn pawn)
        {
            Need_Blood blood = pawn.needs.TryGetNeed<Need_Blood>();
            if (blood == null)
            {
                return null;
            }
            bool desperate = blood.CurCategory == HungerCategory.Starving;
            bool isHuntHuman = blood.preferredFeedMode == PreferredFeedMode.HumanoidLethal || blood.preferredFeedMode == PreferredFeedMode.HumanoidNonLethal;
            bool isHuntLethal = blood.preferredFeedMode == PreferredFeedMode.HumanoidLethal || blood.preferredFeedMode == PreferredFeedMode.AnimalLethal;
            Thing thing;
            ThingDef def;
            if (!BloodUtility.TryFindBestBloodSourceFor(pawn, pawn, desperate, out thing, out def))
            {
                return null;
            }
            if (thing != null && thing != pawn && thing.Spawned)
            {
                Pawn pawn2 = thing as Pawn;
                if (pawn2 != null)
                {
                    return new Job(VampDefOf.ROMV_Feed, pawn2)
                    {
                        killIncappedTarget = blood.preferredFeedMode == PreferredFeedMode.AnimalLethal ? isHuntLethal && pawn2.RaceProps?.petness <= 0 : isHuntLethal
                    };
                }
                return new Job(VampDefOf.ROMV_ConsumeBlood, thing)
                {
                    count = BloodUtility.WillConsumeStackCountOf(pawn, def)
                };
            }
            return null;
        }


    }
}
