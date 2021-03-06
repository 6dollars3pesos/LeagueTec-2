﻿using System.Linq;
using Adept_AIO.Champions.Riven.Core;
using Adept_AIO.SDK.Extensions;
using Adept_AIO.SDK.Usables;
using Aimtec;
using Aimtec.SDK.Extensions;

namespace Adept_AIO.Champions.Riven.Update.Miscellaneous
{
    internal class SpellManager
    {
        private static bool _canUseQ;
        private static bool _canUseW;
        private static Obj_AI_Base _unit;

        public static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
        
            if (args.SpellData.DisplayName.Contains("BasicAttack"))
            {
                Extensions.DidJustAuto = true;
            }

            switch (args.SpellData.Name)
            {
                case "RivenTriCleave":
                    Extensions.LastQCastAttempt = Game.TickCount;
                    _canUseQ = false;
                    Animation.Reset();
                    break;
                case "RivenMartyr":
                    _canUseW = false;
                    Global.Orbwalker.ResetAutoAttackTimer();
                    break;
                case "RivenFengShuiEngine":
                    Extensions.UltimateMode = UltimateMode.Second;
                    break;
                case "RivenIzunaBlade":
                    Extensions.UltimateMode = UltimateMode.First;
                    break;
            }
        }

        public static void OnUpdate()
        {
            if (_unit == null)
            {
                return;
            }

            if (_canUseQ && Extensions.DidJustAuto)
            {
                Global.Player.SpellBook.CastSpell(SpellSlot.Q, _unit);
                Extensions.DidJustAuto = false;
            }

            if (!_canUseW)
            {
                return;
            }
          
            Items.CastTiamat();

            _canUseW = false;

            SpellConfig.W.Cast();
        }

        public static void CastQ(Obj_AI_Base target)
        {
            if (target.HasBuff("FioraW") || target.HasBuff("PoppyW"))
            {
                return;
            }

            _unit = target;
            _canUseQ = true;
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (target.HasBuff("FioraW"))
            {
                return;
            }

            _canUseW = SpellConfig.W.Ready && InsideKiBurst(target);
            _unit = target;  
        }

        public static void CastR2(Obj_AI_Base target)
        {
            if (target.ValidActiveBuffs()
                .Where(buff => Extensions.InvulnerableList.Contains(buff.Name))
                .Any(buff => buff.Remaining > Time(target)))
            {
                return;
            }

            SpellConfig.R2.Cast(target);

            if (target.Distance(Global.Player) <= Global.Player.AttackRange)
            {
                Items.CastTiamat();
            }
        }

        private static int Time(GameObject target)
        {
            return (int)(Global.Player.Distance(target) / (SpellConfig.R2.Speed * 1000 + SpellConfig.R2.Delay));
        }

        public static bool InsideKiBurst(GameObject target)
        {
            return Global.Player.HasBuff("RivenFengShuiEngine")
                 ? Global.Player.Distance(target) <= 265 + target.BoundingRadius
                 : Global.Player.Distance(target) <= 195 + target.BoundingRadius;
        }

        public static bool InsideKiBurst(Vector3 position)
        {
            return Global.Player.HasBuff("RivenFengShuiEngine")
                ? Global.Player.Distance(position) <= 265
                : Global.Player.Distance(position) <= 195;
        }
    }
}
