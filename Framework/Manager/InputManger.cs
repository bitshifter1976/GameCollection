using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace Framework
{
    public partial class InputManager : GameComponent
    {
        private readonly Mouse mouse;
        private readonly Keyboard keyboard;
        private readonly Dictionary<Keys, float> holdingKeys = new();
        private readonly SortedList<int,GamePad> gamePads = new();
        private SortedList<int,Dictionary<Buttons, float>> gamePadHoldingKeys = new();
        private float holdingKeyTime = 0.3f; 
        private float gamePadHoldingKeyTime = 0.3f;
        private float timeElapsed;

        public InputManager(Game game) : base(game)
        {
            keyboard = new Keyboard();
            mouse = new Mouse();
            foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
                gamePads.Add((int)playerIndex, new GamePad());
            game.Components.Add(this);
            game.Services.AddService(typeof(InputManager), this);
        }

        public override void Initialize()
        {
            keyboard.Initialize();
            mouse.Initialize();
            foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
                gamePads[(int)playerIndex].Initialize(playerIndex);
            base.Initialize();
        }

        public void CreateHoldingKeys(float holdTime, params Keys[] keys)
        {
            holdingKeyTime = holdTime;
            holdingKeys.Clear();
            foreach (var key in keys)
                holdingKeys.Add(key, 0f);
        }

        public void CreateHoldingButtons(float holdTime, params Buttons[] buttons)
        {
            gamePadHoldingKeyTime = holdTime;
            gamePadHoldingKeys = new SortedList<int, Dictionary<Buttons, float>>();
            foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
                gamePadHoldingKeys.Add((int)playerIndex, new Dictionary<Buttons, float>());
            foreach (var button in buttons)
            {
                foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
                    gamePadHoldingKeys[(int)playerIndex].Add(button, 0f);
            }
        }

        public bool MouseButtonPressed(MouseButton button)
        {
            return mouse.IsButtonPressed(button);
        }

        public Point MousePosition()
        {
            return mouse.Position;
        }

        public bool KeyPressed(Keys key)
        {
            return keyboard.IsKeyPressed(key);
        }

        public bool KeyPressed(params Keys[] keys)
        {
            return keys.Any(KeyPressed);
        }

        public bool KeyUp(Keys key)
        {
            return keyboard.IsKeyUp(key);
        }

        public bool KeyUp(params Keys[] keys)
        {
            return keys.Any(KeyUp);
        }

        public bool KeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
        }

        public bool KeyDown(params Keys[] keys)
        {
            return keys.Any(KeyDown);
        }

        public bool HoldingKey(Keys key)
        {
            return holdingKeys[key] > holdingKeyTime;
        }

        public bool HoldingKey(params Keys[] keys)
        {
            return keys.Any(HoldingKey);
        }

        public bool GamePadHoldingKey(PlayerIndex playerIdx, Buttons button)
        {
            return gamePadHoldingKeys[(int)playerIdx].First(b => b.Key == button).Value > gamePadHoldingKeyTime;
        }

        public bool GamePadHoldingKey(PlayerIndex playerIdx, params Buttons[] buttons)
        {
            return buttons.Any(button => GamePadHoldingKey(playerIdx, button));
        }

        public bool GamePadKeyPressed(PlayerIndex playerIdx, Buttons button)
        {
            return gamePads[(int)playerIdx].IsButtonPressed(button);
        }

        public bool GamePadKeyPressed(PlayerIndex playerIdx, params Buttons[] buttons)
        {
            return buttons.Any(button => GamePadKeyPressed(playerIdx, button));
        }

        public bool GamePadKeyDown(PlayerIndex playerIdx, Buttons button)
        {
            return gamePads[(int)playerIdx].IsButtonDown(button);
        }

        public bool GamePadKeyDown(PlayerIndex playerIdx, params Buttons[] buttons)
        {
            return buttons.Any(button => GamePadKeyDown(playerIdx, button));
        }

        public bool GamePadKeyUp(PlayerIndex playerIdx, Buttons button)
        {
            return gamePads[(int)playerIdx].IsButtonUp(button);
        }

        public bool GamePadKeyUp(PlayerIndex playerIdx, params Buttons[] buttons)
        {
            return buttons.Any(button => GamePadKeyUp(playerIdx, button));
        }

        public void Reset()
        {
            keyboard.Reset(); 
            mouse.Update();
            gamePads.Values.ToList().ForEach(p => p.Reset());
            holdingKeys.Clear();
            gamePadHoldingKeys.Clear();
            timeElapsed = 0;
        }

        public override void Update(GameTime gameTime)
        {
            keyboard.Update();
            mouse.Update();
            gamePads.Values.ToList().ForEach(p => p.Update());
            UpdateHoldingKeys(gameTime);
            base.Update(gameTime);
        }

        private void UpdateHoldingKeys(GameTime gameTime)
        {
            timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var key in holdingKeys.Keys)
            {
                if (KeyDown(key))
                    holdingKeys[key] += timeElapsed;
                else if (KeyUp(key))
                    holdingKeys[key] = 0f;
            }

            if (gamePadHoldingKeys.Count > 0)
            {
                foreach (var playerIndex in (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex)))
                {
                    foreach (var key in gamePadHoldingKeys[(int)playerIndex].Keys)
                    {
                        if (GamePadKeyDown(playerIndex, key))
                            gamePadHoldingKeys[(int)playerIndex][key] += timeElapsed;
                        else if (GamePadKeyUp(playerIndex, key))
                            gamePadHoldingKeys[(int)playerIndex][key] = 0f;
                    }
                }
            }
        }
    }
}
