﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;

namespace CrowdCORE
{
    public class UISplashScreen : UIBaseScreen
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            int playerCount = ReInput.players.playerCount;
            for (int i = 0; i < playerCount; i++)
            {
                Player p = ReInput.players.GetPlayer(i);
                p.controllers.maps.SetMapsEnabled(true, Category.Menu);

            }

            base.Start();
        }

        public override void Update()
        {
            if (!UIManager.Instance.IsInputLocked)
            {
                int playerCount = ReInput.players.playerCount;
                for (int i = 0; i < playerCount; i++)
                {
                    Player p = ReInput.players.GetPlayer(i);
                    if (p.GetAnyButtonDown())
                    {
                        UIManager.Instance.TransitionToScreen(UIScreenId.Warnings);
                    }
                }
            }
            base.Update();
        }
    }
}