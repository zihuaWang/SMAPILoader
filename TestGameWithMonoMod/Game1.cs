using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TestGameWithMonoMod
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            GameAssetTool.SetupLoadAssetPath();
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            YoooProtected(); //bug here if protected method
            YoooInternal();
            YoooPrivate();

            //base.Draw(gameTime); //bug here
        }

        internal void YoooInternal() { }
        private void YoooPrivate() { }
        protected void YoooProtected() { }

    }
    [HarmonyPatch]
    public static class GameAssetTool
    {
        static Harmony harmony;
        public static void SetupLoadAssetPath()
        {
            harmony = new(nameof(GameAssetTool));
            Harmony.DEBUG = true;
            harmony.PatchAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Game1), "Draw")]
        public static void Draw(Game1 __instance, GameTime gameTime)
        {
            Console.WriteLine("prefix draw: ");
        }
    }

}
