﻿using System.Linq;
using Adept_AIO.Champions.LeeSin.Core.Spells;
using Adept_AIO.SDK.Extensions;
using Aimtec;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Extensions;

namespace Adept_AIO.Champions.LeeSin.Update.OrbwalkingEvents.LaneClear
{
    internal class LaneClear : ILaneClear
    {
        public bool Q1Enabled { get; set; }
        public bool WEnabled { get; set; }
        public bool EEnabled { get; set; }
        public bool CheckEnabled { get; set; }

        private readonly ISpellConfig _spellConfig;

        public LaneClear(ISpellConfig spellConfig)
        {
            _spellConfig = spellConfig;
        }

        public void OnPostAttack()
        {
            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.Distance(Global.Player) < Global.Player.AttackRange + x.BoundingRadius &&
                                                                      x.Health > Global.Player.GetAutoAttackDamage(x));

            if (minion == null || !CheckEnabled && Global.Player.CountEnemyHeroesInRange(2000) >= 1)
            {
                return;
            }

            if (_spellConfig.E.Ready && EEnabled && minion.Health < Global.Player.GetSpellDamage(minion, SpellSlot.E))
            {
                _spellConfig.E.Cast(minion);
            }
            else if (_spellConfig.W.Ready && WEnabled)
            {
                _spellConfig.W.CastOnUnit(Global.Player);
            }
        }

        public void OnUpdate()
        {
            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.Distance(Global.Player) < _spellConfig.Q.Range + x.BoundingRadius);

            if (minion == null || !CheckEnabled && Global.Player.CountEnemyHeroesInRange(2000) >= 1)
            {
                return;
            }

            if (_spellConfig.Q.Ready && Q1Enabled)
            {
                _spellConfig.Q.Cast(minion);
            }
        }
    }
}
