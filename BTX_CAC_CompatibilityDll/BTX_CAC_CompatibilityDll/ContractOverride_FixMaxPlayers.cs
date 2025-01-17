﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using BattleTech;
using BattleTech.Framework;
using System.Reflection.Emit;

namespace BTX_CAC_CompatibilityDll
{
    [HarmonyPatch(typeof(ContractOverride), "FromJSONFull")]
    class ContractOverride_FromJSONFull
    {
        public static void Postfix(ContractOverride __instance)
        {
            PatchContract(__instance);
        }

        internal static void PatchContract(ContractOverride __instance)
        {
            if (__instance.maxNumberOfPlayerUnits == 4 && !Main.Sett.Use4LimitOnContractIds.Contains(__instance.ID))
            {
                __instance.maxNumberOfPlayerUnits = 8;
            }
        }
    }


    [HarmonyPatch(typeof(ContractOverride), "FullRehydrate")]
    class ContractOverride_FullRehydrate
    {
        public static void Postfix(ContractOverride __instance)
        {
            ContractOverride_FromJSONFull.PatchContract(__instance);
        }
    }
}
