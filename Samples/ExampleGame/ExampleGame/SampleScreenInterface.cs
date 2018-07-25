using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wobble;
using Wobble.Graphics;
using Wobble.Graphics.Shaders;
using Wobble.Graphics.Sprites;
using Wobble.Input;
using Wobble.Screens;
using Wobble.Window;

namespace ExampleGame
{
    public class SampleScreenInterface : ScreenInterface
    {
        /// <summary>
        ///     Test sprite
        /// </summary>
        public Sprite SpriteWithShader { get; }

        public AnimatableSprite AnimatedSprite { get; }

        public SpriteText SampleText { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="screen"></param>
        public SampleScreenInterface(SampleScreen screen) : base(screen)
        {
            // Grab the game instance.
            var game = (ExampleGame) GameBase.Game;

            // Create new sprite to be drawn.
            SpriteWithShader = new Sprite
            {
                Image = game.Spongebob,
                Size = new ScalableVector2(400, 400),
                Alignment = Alignment.MidCenter,
                Tint = Color.Blue,
                Parent = Container,
                SpriteBatchOptions = new SpriteBatchOptions()
                {
                    Shader = new Shader(ResourceStore.semi_transparent, new Dictionary<string, object>
                    {
                        {"p_dimensions", new Vector2(400, 400)},
                        {"p_position", new Vector2(0, 0)},
                        {"p_rectangle", new Vector2(200, 400)},
                        {"p_alpha", 0f}
                    })
                }
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

            new Sprite()
            {
                Alignment = Alignment.TopRight,
                Parent = Container,
                Image = game.Spongebob,
                Size = new ScalableVector2(150, 80)
            };

            SampleText = new SpriteText
            {
                Font = game.Aller,
                Text = "hey sample text",
                Parent = AnimatedSprite,
                TextColor = Color.White,
                Alignment = Alignment.MidCenter,
                X = 0,
                TextScale = 4.5f
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
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