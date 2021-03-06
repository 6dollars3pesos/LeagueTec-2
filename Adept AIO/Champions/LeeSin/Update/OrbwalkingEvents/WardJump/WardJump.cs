﻿using Adept_AIO.Champions.LeeSin.Core.Spells;
using Adept_AIO.Champions.LeeSin.Update.Ward_Manager;
using Aimtec;

namespace Adept_AIO.Champions.LeeSin.Update.OrbwalkingEvents.WardJump
{
    internal class WardJump : IWardJump
    {
        public bool Enabled { get; set; }
        public int Range { get; set; }

        private readonly IWardTracker _wardTracker;

        private readonly IWardManager _wardManager;

        private readonly ISpellConfig _spellConfig;

        public WardJump(IWardTracker wardTracker, IWardManager wardManager, ISpellConfig spellConfig)
        {
            _wardTracker = wardTracker;
            _wardManager = wardManager;
            _spellConfig = spellConfig;
        }

        public void OnKeyPressed()
        {
            if (!Enabled)
            {
                return;
            }

            if (_spellConfig.W.Ready && _spellConfig.IsFirst(_spellConfig.W) && _wardTracker.IsWardReady)
            {
                _wardManager.WardJump(Game.CursorPos, Range);
            }
        }
    }
}
