﻿using System.Collections.Generic;
using Adept_AIO.SDK.Delegates;
using Adept_AIO.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

namespace Adept_AIO.Champions.Tristana.Core
{
    internal class MenuConfig
    {
        public Menu Combo,
                    Harass,
                    LaneClear,
                    JungleClear,
                    Killsteal,
                    Drawings;

        public MenuConfig()
        {
            var mainMenu = new Menu(string.Empty, "Adept AIO", true);
            mainMenu.Attach();

            Global.Orbwalker.Attach(mainMenu);

            Gapcloser.Attach(mainMenu, "Anti Gapcloser");

            Combo = new Menu("Combo", "Combo")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("W", "Smart W Usage", false).SetToolTip("Will Try To Chain Combo with W"),
                new MenuBool("E", "Use E"),
                new MenuSeperator("Whitelist", "Whitelist")
            };

            foreach (var target in GameObjects.EnemyHeroes)
            {
                Combo.Add(new MenuBool(target.ChampionName, "Use E On: " + target.ChampionName));
            }

            Harass = new Menu("Harass", "Harass")
            {
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E"),
                new MenuSeperator("Whitelist", "Whitelist")
            };

            foreach (var target in GameObjects.EnemyHeroes)
            {
                Harass.Add(new MenuBool(target.ChampionName, "Use E On: " + target.ChampionName));
            }

            LaneClear = new Menu("LaneClear", "LaneClear")
            {
                new MenuBool("Check", "Safe Clear").SetToolTip("Wont clear when enemies are nearby"),
                new MenuBool("Q", "Use Q"),
                new MenuSliderBool("E", "Use E If X Hit (AoE)", true, 3, 1, 7),
                new MenuBool("Turret", "Use E At Turret")
            };

            JungleClear = new Menu("JungleClear", "JungleClear")
            {
                new MenuBool("Avoid", "Don't Use Anything At Lvl 1"),
                new MenuBool("Q", "Use Q"),
                new MenuBool("E", "Use E")
            };

            Killsteal = new Menu("Killsteal", "Killsteal")
            {
                new MenuBool("W", "Use W", false),
                new MenuBool("E", "Use E"),
                new MenuBool("R", "Use R")
            };

            Drawings = new Menu("Drawings", "Drawings")
            {
                new MenuSlider("Segments", "Segments", 200, 50, 300).SetToolTip("Smoothness of the circles. Less equals more FPS."),
                new MenuBool("Dmg", "Damage"),
                new MenuBool("W", "W Range")
            };

            foreach (var menu in new List<Menu>
            {
                Combo,
                Harass,
                LaneClear,
                JungleClear,
                Killsteal,
                Drawings,
                MenuShortcut.Credits
            }) mainMenu.Add(menu);
        }
    }
}
