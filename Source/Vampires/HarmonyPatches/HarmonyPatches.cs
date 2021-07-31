using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;
using UnityEngine;
using RimWorld.Planet;
using Verse.AI.Group;
using AbilityUser;
using DubsBadHygiene;

namespace Vampire
{
    [StaticConstructorOnStartup]
    static partial class HarmonyPatches
    {
        public static bool VampireGenInProgress = false;

        static HarmonyPatches()
        {
            Log.Message("working1");
            var harmony = new Harmony("rimworld.jecrell.vampire");

            Log.Message("working2");
            HarmonyPatches_Needs(harmony);

            Log.Message("working3");
            HarmonyPatches_Ingestion(harmony);

            Log.Message("working4");
            HarmonyPatches_Pathing(harmony);

            Log.Message("working5");
            HarmonyPatches_Caravan(harmony);

            Log.Message("working6");
            HarmonyPatches_Givers(harmony);

            Log.Message("working7");
            HarmonyPatches_Beds(harmony);

            Log.Message("working8");
            HarmonyPatches_Lovin(harmony);

            Log.Message("working9");
            HarmonyPatches_Graphics(harmony);

            Log.Message("working10");
            HarmonyPatches_UI(harmony);

            Log.Message("working11");
            HarmonyPatches_Age(harmony);

            Log.Message("working12");
            HarmonyPatches_AI(harmony);

            Log.Message("working13");
            HarmonyPatches_Menu(harmony);

            Log.Message("working14");
            HarmonyPatches_Thoughts(harmony);

            Log.Message("working15");
            HarmonyPatches_Powers(harmony);

            Log.Message("working16");
            HarmonyPatches_Graves(harmony);

            Log.Message("working17");
            HarmonyPatches_Misc(harmony);

            Log.Message("working18");
            HarmonyPatches_Mods(harmony);

            Log.Message("working19");
            HarmonyPatches_Royalty(harmony);

            Log.Message("working20");
            HarmonyPatches_Fixes(harmony);
            Log.Message("working21");
        }    




    }
}