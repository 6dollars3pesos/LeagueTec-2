﻿using System.Linq;
using Adept_AIO.Champions.Riven.Core;
using Adept_AIO.Champions.Riven.Update.Miscellaneous;
using Adept_AIO.SDK.Extensions;
using Adept_AIO.SDK.Usables;
using Aimtec;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Extensions;

namespace Adept_AIO.Champions.Riven.Update.OrbwalkingEvents
{
    internal class Combo
    {
        private static bool CanCastR1(Obj_AI_Base target)
        {
            return MenuConfig.Combo["R"].Value != 0 && SpellConfig.R.Ready && !(MenuConfig.Combo["Check"].Enabled && target.HealthPercent() < MenuConfig.Combo["Check"].Value) &&
                   Extensions.UltimateMode == UltimateMode.First &&
                   !(MenuConfig.Combo["R"].Value == 2 && Dmg.Damage(target) <
                     target.Health);
        }

        public static void OnPostAttack(Obj_AI_Base target)
        {
            if (AutoBeforeR2(target) && (SpeedItUp(target) || Extensions.CurrentQCount == 1 && !SpellConfig.Q.Ready) &&
                target.Health < Global.Player.GetAutoAttackDamage(target) + Global.Player.GetSpellDamage(target, SpellSlot.R))
            {
                SpellManager.CastR2(target);
            }

            if (SpellConfig.Q.Ready)
            {
                if (SpellManager.InsideKiBurst(target) && SpellConfig.W.Ready && !CanCastR1(target))
                {
                    SpellConfig.W.Cast();
                }
                SpellManager.CastQ(target);
            }
        }

        public static void OnUpdate()
        {
            var target = Global.TargetSelector.GetTarget(SpellConfig.R2.Range);
            if (target != null && Extensions.UltimateMode == UltimateMode.Second && target.Distance(Global.Player) >= SpellConfig.R2.Range - 100 && target.HealthPercent() <= 40)
            {
                SpellManager.CastR2(target);
            }

            if (target != null && MenuConfig.Combo["Chase"].Value != 0 && target.Distance(Global.Player) > Global.Player.AttackRange)
            {
                switch (MenuConfig.Combo["Chase"].Value)
                {
                    case 1:
                        if (target.Distance(Global.Player) <= Global.Player.AttackRange + SpellConfig.Q.Range && Extensions.CurrentQCount == 1)
                        {
                            SpellManager.CastQ(target); // Bug: Will only cast Q when inside of Q range. CBA fix now
                                                        // Todo: Add Q casting towards a Vector3
                        }
                        break;
                    case 2:
                        if (target.Distance(Global.Player) <= Global.Player.AttackRange + SpellConfig.E.Range)
                        {
                            SpellConfig.E.CastOnUnit(target);
                        }
                        break;
                    case 3:
                        if (target.Distance(Global.Player) <= Global.Player.AttackRange + SpellConfig.Q.Range + SpellConfig.E.Range)
                        {
                            SpellConfig.E.CastOnUnit(target);
                            SpellManager.CastQ(target);
                        }
                        break;
                }
            }

            ExecuteCombo();
            Flash();
        }

        private static void ExecuteCombo()
        {
            var target = Global.TargetSelector.GetTarget(Extensions.EngageRange + 50);
            if (target == null)
            {
                return;
            }

            if (SpellConfig.Q.Ready || SpellConfig.W.Ready && target.Distance(Global.Player) <= Global.Player.AttackRange && Global.Orbwalker.CanAttack())
            {
                Global.Orbwalker.Attack(target); 
            }

            if (SpellConfig.E.Ready)
            {
                SpellConfig.E.Cast(target.ServerPosition);

                if (CanCastR1(target))
                {
                    SpellConfig.R.Cast();
                }
            }
            else if (SpellManager.InsideKiBurst(target) && SpellConfig.W.Ready && !CanCastR1(target))
            {
                if (SpellConfig.Q.Ready && Extensions.CurrentQCount == 1)
                {
                    return;
                }
                SpellManager.CastW(target);
            }
        }

        private static void Flash()
        {
            var target = Global.TargetSelector.GetTarget(1200);
            if (target == null || target.IsUnderEnemyTurret())
            {
                return;
            }

            Extensions.AllIn = MenuConfig.Combo["Flash"].Enabled &&
                               SummonerSpells.Flash.Ready &&
                               SpeedItUp(target) &&
                               target.Distance(Global.Player) > 500 &&
                               target.Distance(Global.Player) < 720;

            if (!Extensions.AllIn)
            {
                return;
            }

            SummonerSpells.Flash.Cast(target);

            if (SpellManager.InsideKiBurst(target))
            {
                SpellManager.CastW(target);
            }
        }

        private static bool AutoBeforeR2(GameObject target)
        {
            return target.Distance(Global.Player) <= Global.Player.AttackRange + 40
                   && SpellConfig.R2.Ready
                   && Extensions.UltimateMode == UltimateMode.Second
                   && MenuConfig.Combo["R2"].Enabled;
        }

        private static bool SpeedItUp(Obj_AI_Base target)
        {
            return target.Health < Dmg.Damage(target) * .3 && Global.Player.HealthPercent() >= 65 ||
                   target.Health < Global.Player.GetAutoAttackDamage(target) && GameObjects.AllyHeroes.FirstOrDefault(x => x.Distance(target) < 300) == null ||
                   target.Health < Dmg.Damage(target) * .75f && target.HealthPercent() <= 25;
        }
    }
}