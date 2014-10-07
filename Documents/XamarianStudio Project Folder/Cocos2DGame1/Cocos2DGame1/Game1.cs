using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace MindRain
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
		public static InputHelper Input;
		public CCSize screenSize = CCDirector.SharedDirector.WinSize;
		CollisionDetection collisionlistener = new CollisionDetection();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //#if MACOS
            //            Content.RootDirectory = "AngryNinjas/Content";
            //#else

			//Window Title
			this.Window.Title = "MindRain";

            Content.RootDirectory = "Content";
            //#endif
            //
            //#if XBOX || OUYA
            //            graphics.IsFullScreen = true;
            //#else
            graphics.IsFullScreen = false;
            //#endif

            // Frame rate is 30 fps by default for Windows Phone.
			TargetElapsedTime = TimeSpan.FromTicks (333333 / 2);

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            CCApplication application = new AppDelegate(this, graphics);
            Components.Add(application);
            //#if XBOX || OUYA
            //            CCDirector.SharedDirector.GamePadEnabled = true;
            //            application.GamePadButtonUpdate += new CCGamePadButtonDelegate(application_GamePadButtonUpdate);
            //#endif

        }

        //#if XBOX || OUYA
        //        private void application_GamePadButtonUpdate(CCGamePadButtonStatus backButton, CCGamePadButtonStatus startButton, CCGamePadButtonStatus systemButton, CCGamePadButtonStatus aButton, CCGamePadButtonStatus bButton, CCGamePadButtonStatus xButton, CCGamePadButtonStatus yButton, CCGamePadButtonStatus leftShoulder, CCGamePadButtonStatus rightShoulder, PlayerIndex player)
        //        {
        //            if (backButton == CCGamePadButtonStatus.Pressed)
        //            {
        //                ProcessBackClick();
        //            }
        //        }
        //#endif

        private void ProcessBackClick()
        {
            if (CCDirector.SharedDirector.CanPopScene)
            {
                CCDirector.SharedDirector.PopScene();
            }
            else
            {
                Exit();
            }
        }

        protected override void Update(GameTime gameTime)
        {
			//Note: Updates the state of the input helper
			Input.Update();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ProcessBackClick();
            }

			//Note: This section of code gets the currently running scene and use it to find the world and update all b2bodys that are contained in that world
			var runningScene = CCDirector.SharedDirector.RunningScene;
			GameLayer gameLayer = null;
			float timeStep = 1/60f;

			if (runningScene != null) 
			{
				gameLayer = (GameLayer)runningScene.GetChildByTag (GameScene.gameLayerTag);
				gameLayer.Update ();

				//Note: the step function does the physics for the world with the bodies in the currently used world
				//Note: the first input in the function is used to help represent seconds, the second is velocity iterations and lastly position iterations
				gameLayer.world.Step (timeStep, 8, 3);
			}


            base.Update(gameTime);
        }
    }
}