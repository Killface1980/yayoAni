using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoAni;

[HotSwappable]
[HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming")]
internal class patch_DrawEquipmentAiming
{
    [HarmonyPriority(9999)]
    [HarmonyPrefix]
    static bool Prefix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
    {
        if (!core.val_combat)
        {
            return true;
        }
        Pawn pawn = __instance.pawn;

        float num = aimAngle - 90f;
        Mesh mesh;



        bool isMeleeAtk = false;
        bool flip = false;

            
        Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;

        bool flag = true;
        if (pawn.CurJob != null && pawn.CurJob.def.neverShowWeapon) flag = false;

        if (flag && stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid)
        {
            if (pawn.Rotation == Rot4.West)
            {
                flip = true;
            }

            if (!pawn.equipment.Primary.def.IsRangedWeapon || stance_Busy.verb.IsMeleeAttack)
            {
                // 근접공격
                isMeleeAtk = true;
            }
        }


                


        if (isMeleeAtk)
        {
            if (flip)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
        }
        else
        {
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            //else if ((aimAngle > 200f && aimAngle < 340f) || ignore)
            else if ((aimAngle > 200f && aimAngle < 340f) || flip)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
        }



        num %= 360f;

        CompEquippable compEquippable = eq.TryGetComp<CompEquippable>();
        if (compEquippable != null)
        {
            EquipmentUtility.Recoil(eq.def, EquipmentUtility.GetRecoilVerb(compEquippable.AllVerbs), out var drawOffset, out var angleOffset, aimAngle);
            drawLoc += drawOffset;
            num += angleOffset;
        }

        Material matSingle;
        //if (graphic_StackCount != null)
        //{
        //    matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
        //}
        //else
        //{
        //    matSingle = eq.Graphic.MatSingle;
        //}
        //Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
        Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
        Graphics.DrawMesh(material: (graphic_StackCount == null) ? eq.Graphic.MatSingleFor(eq) : graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingleFor(eq), mesh: mesh, position: drawLoc, rotation: Quaternion.AngleAxis(num, Vector3.up), layer: 0);



        return false;

    }








        





}