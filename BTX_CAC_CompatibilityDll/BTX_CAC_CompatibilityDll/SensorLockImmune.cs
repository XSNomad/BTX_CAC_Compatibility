﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace BTX_CAC_CompatibilityDll
{
    [HarmonyPatch(typeof(ActiveProbeSequence), "FireWave")]
    class ActiveProbeSequence_FireWave
    {
        public static bool Prefix(ActiveProbeSequence __instance, AbstractActor Target, ref int ___numWavesFired, ref float ___timeSinceLastWave)
        {
            if (Target.StatCollection.GetValue<float>("SensorLockDefense") > 0) {
                __instance.SetCamera(CameraControl.Instance.ShowSensorLockCam(Target, 2f), __instance.MessageIndex);
                CameraControl.Instance.ClearTargets();
                CombatGameState Combat = Traverse.Create(__instance).Property("Combat").GetValue<CombatGameState>();
                Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.owningActor.GUID, Target.GUID, "Sensor Lock blocked by ECM", FloatieMessage.MessageNature.Buff));
                ___numWavesFired++;
                ___timeSinceLastWave = 0;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SensorLockSequence), "FireWave")]
    class SensorLockSequence_FireWave
    {
        public static bool Prefix(SensorLockSequence __instance, ref int ___numWavesFired, ref float ___timeSinceLastWave)
        {
            if (__instance.Target.StatCollection.GetValue<float>("SensorLockDefense") > 0)
            {
                CombatGameState Combat = Traverse.Create(__instance).Property("Combat").GetValue<CombatGameState>();
                Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.owningActor.GUID, __instance.Target.GUID, "Sensor Lock blocked by ECM", FloatieMessage.MessageNature.Buff));
                ___numWavesFired++;
                ___timeSinceLastWave = 0;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Mech), "InitStats")]
    class AbstractActor_InitStats
    {
        public static void Prefix(AbstractActor __instance)
        {
            if (!__instance.Combat.IsLoadingFromSave)
            {
                __instance.StatCollection.AddStatistic("SensorLockDefense", 0f);
            }
        }
    }
}