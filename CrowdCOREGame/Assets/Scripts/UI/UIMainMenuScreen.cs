using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;

namespace CrowdCORE
{
    public class UIMainMenuScreen : UIBaseScreen
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Initialize()
        {
            int playerCount = ReInput.players.playerCount;
            for (int i = 0; i < playerCount; i++)
            {
                Player p = ReInput.players.GetPlayer(i);
                p.controllers.maps.SetMapsEnabled(true, Category.Menu);

            }

            base.Initialize();
        }

        public override void SetPrompts()
        {
            List<InputButton> lButtons = new List<InputButton>();
            lButtons.Add(InputButton.A);
            UIManager.Instance.Prompts.SetLeftPrompts(lButtons);

            List<InputButton> rButtons = new List<InputButton>();
            rButtons.Add(InputButton.B);
            UIManager.Instance.Prompts.SetRightPrompts(rButtons);

            base.SetPrompts();
        }

        public override void Shutdown()
        {
            UIManager.Instance.Prompts.ClearPrompts();
            base.Shutdown();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}