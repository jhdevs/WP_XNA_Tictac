using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using GameLogic;

namespace ScreenManagement
{
    class ScreenManager : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;

        TicTacLogic gameLogic = new TicTacLogic();

        Texture2D title;
        Texture2D background;
        Texture2D background2;
        Texture2D temp_texture;
        Texture2D pause;
        SpriteFont font;

        List<MenuItem> menu = new List<MenuItem>();

        const int MaxInputs = 4;
        GamePadState[] CurrentGamePadStates;
        GamePadState[] LastGamePadStates;

        public enum ScreenState
        {
            MainMenu,
            Game,
            Pause,
            Result
        }
        ScreenState state;
        
        public ScreenManager(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Debug.WriteLine("Initialize()");
            MenuItem start = new MenuItem("START");
            start.Selected += StartGame;
            MenuItem quit = new MenuItem("QUIT");
            quit.shade = Color.Red;
            quit.Selected += QuitGame;
            menu.Add(start);
            menu.Add(quit);
            CurrentGamePadStates = new GamePadState[MaxInputs]; 
            LastGamePadStates = new GamePadState[MaxInputs];
            state = ScreenState.MainMenu;

            gameLogic.Initialize();
            
            base.Initialize();
        }

        void StartGame(object sender, EventArgs e)
        {
            Debug.WriteLine("StartGame()");
            state = ScreenState.Game;
            gameLogic.Begin();
        }

        void QuitGame(object sender, EventArgs e)
        {
            Debug.WriteLine("QuitGame()");
            Game.Exit();
        }

        protected override void LoadContent()
        {
            Debug.WriteLine("LoadContent()");
            // Load content.
            content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            title = content.Load<Texture2D>("title");
            background = content.Load<Texture2D>("background");
            background2 = content.Load<Texture2D>("background2");
            temp_texture = content.Load<Texture2D>("temp_texture");
            font = content.Load<SpriteFont>("menufont");

            gameLogic.o = content.Load<Texture2D>("o");
            gameLogic.x = content.Load<Texture2D>("x");
            gameLogic.ResultO = content.Load<Texture2D>("ResultO");
            gameLogic.ResultX = content.Load<Texture2D>("ResultX");
            gameLogic.ResultNo = content.Load<Texture2D>("ResultNo");
            pause = content.Load<Texture2D>("pause");
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            Debug.WriteLine("UnloadContent()");
        }
        
        public bool IsNewButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (CurrentGamePadStates[i].IsButtonDown(button) && LastGamePadStates[i].IsButtonUp(button))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            //Debug.WriteLine("Update()");
            for (int i = 0; i < MaxInputs; i++)
            {
                LastGamePadStates[i] = CurrentGamePadStates[i];
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.IndependentAxes);
            }

            if (IsNewButtonPress(Buttons.Start)) {
                QuitGame(this, EventArgs.Empty);
            }
            if (IsNewButtonPress(Buttons.Back))
            {
                switch (state)
                {
                    case ScreenState.MainMenu:
                        QuitGame(this, EventArgs.Empty);
                        break;
                    case ScreenState.Game:
                        state = ScreenState.Pause;
                        break;
                    case  ScreenState.Pause:
                        state = ScreenState.MainMenu;
                        break;
                    case  ScreenState.Result:
                        state = ScreenState.MainMenu;
                        break;

                }
            }

            TouchCollection touchState = TouchPanel.GetState();
            bool touchDetected = false;
            Vector2 touchPosition = new Vector2();
            //interpert touch screen presses
            foreach (TouchLocation location in touchState)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                        touchDetected = true;
                        touchPosition = location.Position;
                        break;
                    case TouchLocationState.Moved:
                        break;
                    case TouchLocationState.Released:
                        break;
                }
            }

            if (touchDetected)
            {
                Rectangle touchRect = new Rectangle((int)touchPosition.X - 5, (int)touchPosition.Y - 5,
                                                                10, 10);
                switch (state)
                {
                    case ScreenState.MainMenu:
                        foreach (MenuItem mitem in menu)
                        {
                            if (mitem.rect.Intersects(touchRect))
                                mitem.OnSelectEntry();
                        }
                        break;
                    case ScreenState.Game:
                        gameLogic.ClickRect(touchRect);
                        if (gameLogic.finished) state = ScreenState.Result;
                        break;
                    case ScreenState.Pause:
                        state = ScreenState.Game;
                        break;
                    case ScreenState.Result:
                        StartGame(this, EventArgs.Empty);
                        break;
                }
            }
        }

        class MenuItem
        {
            public String text;
            public Rectangle rect;
            public Color shade = Color.Transparent;
            public MenuItem(String txt) { text = txt; }
            public event EventHandler<EventArgs> Selected;
            protected internal virtual void OnSelectEntry()
            {
                if (Selected != null)
                    Selected(this, EventArgs.Empty);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (state == ScreenState.MainMenu)
            {
                spriteBatch.Begin();

                // Draw Background
                spriteBatch.Draw(background, new Vector2(0, 0),
                     new Color(192, 192, 192));

                // Draw Title
                spriteBatch.Draw(title, new Vector2(60, 55),
                     new Color(192, 192, 192));

                Vector2 position = new Vector2(60, 250);
                float scale = 2.5f;

                foreach (MenuItem mitem in menu)
                {
                    spriteBatch.DrawString(font, mitem.text, position, mitem.shade, 0,
                                          new Vector2(0, 0), scale, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, mitem.text, position, Color.Black, 0,
                                         new Vector2(1, 1), scale, SpriteEffects.None, 0);
                    Vector2 actualSize = font.MeasureString(mitem.text);
                    mitem.rect = new Rectangle((int)position.X, (int)position.Y,
                                                  (int)(actualSize.X * scale), (int)(actualSize.Y * scale));

                    position = new Vector2(position.X, position.Y + (int)(actualSize.Y * scale * 1.5f));
                }
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();

                // Draw Background
                spriteBatch.Draw(background2, new Vector2(0, 0), new Color(192, 192, 192));

                // Draw Title
                spriteBatch.Draw(title, new Vector2(20,20), new Color(192, 192, 192));

                //spriteBatch.Draw(temp_texture, new Rectangle(10, 155, 135, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(10, 320, 135, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(10, 480, 135, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(170, 155, 135, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(170, 320, 135, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(170, 480, 135, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(325, 155, 145, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(325, 320, 145, 140), new Color(192, 192, 192));
                //spriteBatch.Draw(temp_texture, new Rectangle(325, 480, 145, 140), new Color(192, 192, 192));

                gameLogic.Draw(spriteBatch);

                if (state==ScreenState.Pause) 
                {
                    spriteBatch.Draw(pause, new Vector2(40, 600), new Color(192, 192, 192));
                }
                else if (state == ScreenState.Result) 
                {
                    spriteBatch.Draw(gameLogic.Result, new Vector2(40, 600), new Color(192, 192, 192));
                }

                spriteBatch.End();
            }
        }

    }
}
