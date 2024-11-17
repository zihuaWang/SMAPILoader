using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace MyGame;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;
    private Hook _myHook;

    public Game1()
    {
        new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;


        var src = typeof(Game1).GetMethod(nameof(Draw), BindingFlags.Instance | BindingFlags.NonPublic);
        var detour = typeof(Game1).GetMethod(nameof(PrefixDraw), BindingFlags.Static | BindingFlags.Public);
        _myHook = new(src, detour);
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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    public static void PrefixDraw(Game1 game, GameTime gameTime)
    {
        Console.WriteLine("on prefix draw");
    }
}