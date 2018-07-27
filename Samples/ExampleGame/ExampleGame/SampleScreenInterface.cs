using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wobble;
using Wobble.Assets;
using Wobble.Graphics;
using Wobble.Graphics.Shaders;
using Wobble.Graphics.Sprites;
using Wobble.Graphics.UI;
using Wobble.Graphics.UI.Buttons;
using Wobble.Graphics.UI.Debugging;
using Wobble.Input;
using Wobble.Screens;
using Wobble.Window;

namespace ExampleGame
{
    public class SampleScreenInterface : ScreenInterface
    {
        public BackgroundImage Wallpaper { get; }

        /// <summary>
        ///     Test sprite
        /// </summary>
        public Sprite SpriteWithShader { get; }

        public AnimatableSprite AnimatedSprite { get; }

        public SpriteText SampleText { get; }

        public ImageButton ImageButton { get; }

        public ProgressBar SongTimeProgressBar{ get; }

        /// <inheritdoc />
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="screen"></param>
        public SampleScreenInterface(SampleScreen screen) : base(screen)
        {
            // Grab the game instance.
            var game = (ExampleGame) GameBase.Game;

            Wallpaper = new BackgroundImage(game.Wallpaper, 60) { Parent = Container };

            // Create new sprite to be drawn.
            SpriteWithShader = new Sprite
            {
                Image = game.Spongebob,
                Size = new ScalableVector2(400, 400),
                Alignment = Alignment.MidCenter,
                Parent = Container,
                SpriteBatchOptions = new SpriteBatchOptions()
                {
                    SortMode = SpriteSortMode.Deferred,
                    BlendState = BlendState.NonPremultiplied,
                    SamplerState = SamplerState.PointClamp,
                    DepthStencilState = DepthStencilState.Default,
                    RasterizerState = RasterizerState.CullNone,
                    Shader = new Shader(ResourceStore.semi_transparent, new Dictionary<string, object>
                    {
                        {"p_dimensions", new Vector2(400, 400)},
                        {"p_position", new Vector2(0, 0)},
                        {"p_rectangle", new Vector2(200, 400)},
                        {"p_alpha", 0f}
                    })
                }
            };

            // ReSharper disable once ObjectCreationAsStatement
            var fpsCounter = new FpsCounter(game.Aller, 1.0f)
            {
                Alignment = Alignment.BotRight,
                Parent = Container,
                Size = new ScalableVector2(100, 40),
                TextFps =
                {
                    TextColor = Color.LimeGreen,
                    TextScale = 1.1f
                },
            };

            fpsCounter.X -= fpsCounter.TextFps.MeasureString().X;
            fpsCounter.Y = -20;

            SampleText = new SpriteText(game.Aller, "Sample Text")
            {
                Parent = SpriteWithShader,
                TextColor = Color.White,
                Alignment = Alignment.BotCenter,
                X = 0,
                TextScale = 1.25f,
            };

            AnimatedSprite = new AnimatableSprite(game.TestSpritesheet)
            {
                Alignment = Alignment.MidLeft,
                Size = new ScalableVector2(300, 300),
                Parent = Container,
                X = 150,
                SpriteBatchOptions = new SpriteBatchOptions()
                {
                    BlendState = BlendState.Additive
                }
            };

            ImageButton = new ImageButton(WobbleAssets.WhiteBox, (sender, args) => Console.WriteLine("Blue button clicked."))
            {
                Parent = Container,
                Size = new ScalableVector2(100, 100),
                Alignment = Alignment.TopRight,
                X = -300,
                Y = 100,
                Tint = Color.Blue
            };

            new DraggableButton(WobbleAssets.WhiteBox, (sender, args) => Console.WriteLine("red Button Clicked"))
            {
                Parent = Container,
                Size = new ScalableVector2(100, 50),
                Alignment = Alignment.TopRight,
                X = -275,
                Y = 100,
                Tint = Color.Red
            };

            new TextButton(WobbleAssets.WhiteBox, game.Aller, "Click me!", 1f,
                (sender, args) => game.Song.Rate += 0.1f)
            {
                Parent = Container,
                Size = new ScalableVector2(250, 100),
                Alignment = Alignment.BotCenter,
                X = -300,
                Tint = Color.Brown
            };

            new ProgressBar(new Vector2(WindowManager.VirtualScreen.X, 30), game.AudioTime, Color.AliceBlue, Color.Red, false)
            {
                Parent = Container,
                Alignment = Alignment.TopLeft,
                Y = 200
            };

            SongTimeProgressBar = new ProgressBar(new Vector2(WindowManager.VirtualScreen.X / 2f, 10), 0, game.Song.Length,
                game.Song.Position, Color.Gold, Color.LimeGreen)
            {
                Parent = Container,
                Alignment = Alignment.TopLeft,
                Y = 100
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            var game = (ExampleGame) GameBase.Game;
            SongTimeProgressBar.Bindable.Value = game.Song.Time;

            HandleInput();
            Container.Update(gameTime);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            GameBase.Game.GraphicsDevice.Clear(Color.CornflowerBlue);
;
            Container.Draw(gameTime);
            GameBase.Game.SpriteBatch.End();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void Destroy() => Container.Destroy();

        /// <summary>
        ///     In this example, it when the user presses down a key, it'll change the shader's parameters.
        /// </summary>
        private void HandleInput()
        {
            // Make shader transparency rect smaller.
            if (KeyboardManager.CurrentState.IsKeyDown(Keys.Left))
            {
                // Grab the current shader parameter.
                var currentRect = (Vector2) SpriteWithShader.SpriteBatchOptions.Shader.Parameters["p_rectangle"];
                ChangeShaderRectWidth(MathHelper.Clamp(currentRect.X - 20, 0, SpriteWithShader.Width));
            }

            // Make shader transparency rect larger.
            if (KeyboardManager.CurrentState.IsKeyDown(Keys.Right))
            {
                var currentRect = (Vector2) SpriteWithShader.SpriteBatchOptions.Shader.Parameters["p_rectangle"];
                ChangeShaderRectWidth(MathHelper.Clamp(currentRect.X + 20, 0, SpriteWithShader.Width));
            }

            if (KeyboardManager.IsUniqueKeyPress(Keys.Space))
            {
                if (AnimatedSprite.IsLooping)
                    AnimatedSprite.StopLoop();
                else
                    AnimatedSprite.StartLoop(Direction.Forward, 240);
            }

            if (KeyboardManager.IsUniqueKeyPress(Keys.F))
            {
                SampleText.Alignment = Alignment.TopLeft;
                SampleText.TextScale = 1f;
                SampleText.TextColor = Color.White;
            }

            if (KeyboardManager.IsUniqueKeyPress(Keys.Up))
                AnimatedSprite.Size = new ScalableVector2(AnimatedSprite.Width, AnimatedSprite.Height / 1.5f);

        }

        /// <summary>
        ///     Changes the rectangle parameter of the shader and applies it.
        /// </summary>
        /// <param name="width"></param>
        private void ChangeShaderRectWidth(float width)
        {
            SpriteWithShader.SpriteBatchOptions.Shader.SetParameter("p_rectangle", new Vector2(width, SpriteWithShader.Height), true);
        }
    }
}