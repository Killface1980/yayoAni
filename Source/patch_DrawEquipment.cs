using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using Verse;
using System.Linq;




namespace yayoAni
{

    [HotSwappable]
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    internal class patch_DrawEquipment
    {
        [HarmonyPriority(0)]
        [HarmonyPrefix]
        static bool Prefix(PawnRenderer __instance, Vector3 rootLoc)
        {
            if (!core.val_combat)
            {
                return true;
            }
            Pawn pawn = __instance.pawn;

            if (pawn.Dead || !pawn.Spawned)
            {
                return false;
            }
            if (pawn.equipment == null || pawn.equipment.Primary == null)
            {
                return false;
            }
            if (pawn.CurJob != null && pawn.CurJob.def.neverShowWeapon)
            {
                return false;
            }
            


            // duelWeld
            ThingWithComps offHandEquip = null;
            if (core.using_dualWeld)
            {
                if (pawn.equipment.TryGetOffHandEquipment(out ThingWithComps result))
                {
                    offHandEquip = result;
                }
            }





            // 주무기
            Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
            bool drawVanilla = !PawnRenderer_override.animateEquip(__instance, pawn, rootLoc, pawn.equipment.Primary, stance_Busy, new Vector3(0f, 0f, 0.0005f));

            // 보조무기
            if (offHandEquip != null)
            {
                Stance_Busy offHandStance = null;
                if (pawn.GetStancesOffHand() != null)
                {
                    offHandStance = pawn.GetStancesOffHand().curStance as Stance_Busy;
                }
                drawVanilla = !PawnRenderer_override.animateEquip(__instance, pawn, rootLoc, offHandEquip, offHandStance, new Vector3(0.1f, 0.1f, 0f), true) && drawVanilla;
            }

            return drawVanilla;
        }
    }
}

















