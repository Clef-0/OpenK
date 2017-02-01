using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    public partial class KProject : Game
    {
        private void RailGameplayUpdate(GameTime gameTime)
        {
            bool mouseInView = true;
            // mouse stuff
            if ((Mouse.GetState().X >= 0 && Mouse.GetState().X < GraphicsDevice.Viewport.Width) && (Mouse.GetState().Y >= 0 && Mouse.GetState().Y < GraphicsDevice.Viewport.Height))
            {
                mouseX = Mouse.GetState().X;
                mouseY = Mouse.GetState().Y;
            }
            else
            {
                mouseInView = false;
            }

            pEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // location to draw cursor sprite
            cursorPosition = new Vector2(mouseX - 37, mouseY - 37);

            MouseState newMouseState = Mouse.GetState();

            if (beatsElapsed != acknowledgedBeatsElapsed && beatsElapsed % 4 == 0 && beatsElapsed >= 16)
            {
                enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -80)));
                enemies.Add(new Drone(new Vector3(8 * flipMultiplier, 5, -80)));
                enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -80)));
                enemies.Add(new Drone(new Vector3(14 * flipMultiplier, 5, -80)));

                //switch (Rnd.Next(1, 2))
                //{
                //    case 1:
                //        enemies.Add(new Drone(new Vector3(5 * offset, 5, -80)));
                //        break;
                //    case 2:
                //        enemies.Add(new Sentinel(new Vector3(Rnd.Next(-10, 10), 5, Rnd.Next(-90, -80))));
                //        break;
                //    case 3:
                //        enemies.Add(new Colonel(new Vector3(Rnd.Next(-10, 10), 5, Rnd.Next(-90, -80))));
                //        break;
                //}
            }

            for (int i = 0; i < enemies.Count(); i++)
            {
                if (enemies[i] != null)
                {
                    ((Enemy)enemies[i]).FlyTowardsCamera();
                    ((Enemy)enemies[i]).WorldMatrix = Matrix.CreateTranslation(((Enemy)enemies[i]).Position);
                    if (mouseInView && newMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (Intersects(new Vector2(mouseX, mouseY), // mouse position
                            ((Enemy)enemies[i]).Model, // enemy model
                            ((Enemy)enemies[i]).WorldMatrix, // enemy position
                            view,
                            projection,
                            this.GraphicsDevice.Viewport) && ((Enemy)enemies[i]).Health > 0 && lockedEnemies < 8)
                        {
                            logEntry = ((Enemy)enemies[i]).GetType().Name;
                            newEntry = true;
                            ((Enemy)enemies[i]).Injure(1);
                            lockedEnemies += 1;
                        }
                    }
                    if (enemies.ElementAtOrDefault(i) != null)
                    {
                        if (((Enemy)enemies[i]).Position.Z > 10)
                        {
                            if (((Enemy)enemies[i]).Health == 0)
                            {
                                lockedEnemies -= 1;
                            }
                            enemies.RemoveAt(i);
                            logEntry = i + " offscreen and culled";
                            newEntry = true;
                        }
                    }
                }
            }

            offsetX = (mouseX - ((float)GraphicsDevice.Viewport.Width / 2)) / 1000;
            offsetY = (mouseY - ((float)GraphicsDevice.Viewport.Height / 2)) / 500;

            //tilt camera with mouse move
            view = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(offsetX, 3, offsetY), Vector3.UnitY);

            oldMouseState = newMouseState;

            ticksElapsedSinceBeat++;

            if (acknowledgedBeatsElapsed != beatsElapsed)
            {
                acknowledgedBeatsElapsed = beatsElapsed;
                logEntry = Convert.ToString(beatsElapsed);
                newEntry = true;
                ticksElapsedSinceBeat = 0;

                // destroy enemies on beat

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    if (((Enemy)enemies[i]).Health == 0 && newMouseState.LeftButton == ButtonState.Released)
                    {
                        Vector3 pos = GraphicsDevice.Viewport.Project(((Enemy)enemies[i]).Position, projection, view, world);
                        pEffect.Trigger(new Vector2(pos.X, pos.Y));
                        enemies.RemoveAt(i);
                        lockedEnemies = 0;
                    }
                }

                if (beatsElapsed % 4 == 0)
                {
                    if (flipMultiplier == 1)
                    {
                        flipMultiplier = -1;
                    }
                    else
                    {
                        flipMultiplier = 1;
                    }
                }
            }

            // beat bar stuff
            float currentTime = DateTime.Now.Ticks - startingTime;
            beatsElapsed = (int)Math.Truncate((Decimal)currentTime / (Decimal)ticksPerBeat);
        }

        private void RailGameplayDraw(GameTime gameTime)
        {
            int R = 0;
            int G = 128;
            int B = 128;
            GraphicsDevice.Clear(Color.FromNonPremultiplied(R, G, B, 255));

            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: pCamera.GetViewMatrix());

            spriteBatch.Draw(pEffect);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(vignetteTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            if (newEntry)
            {
                newEntry = false;
                log[0] = log[1];
                log[1] = log[2];
                log[2] = log[3];
                log[3] = log[4];
                log[4] = logEntry;
            }

            spriteBatch.DrawString(Arial12, log[0], new Vector2(5, 120), Color.White);
            spriteBatch.DrawString(Arial12, log[1], new Vector2(5, 140), Color.White);
            spriteBatch.DrawString(Arial12, log[2], new Vector2(5, 160), Color.White);
            spriteBatch.DrawString(Arial12, log[3], new Vector2(5, 180), Color.White);
            spriteBatch.DrawString(Arial12, log[4], new Vector2(5, 200), Color.White);


            // CAMERA SCALE

            Dictionary<int, float> tickScale = new Dictionary<int, float>
            {
                {0, 1f},
                {1, 1.01f},
                {2, 1.02f},
                {3, 1.03f},
                {4, 1.04f},
                {5, 1.05f},
                {6, 1.03f},
                {7, 1.01f},
                {8, 0.99f}
            };
            

            if (tickScale.ContainsKey(ticksElapsedSinceBeat))
            {
                world.Scale = new Vector3(tickScale[ticksElapsedSinceBeat]);

                if (beatsElapsed % 4 == 0)
                {
                    projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45) / ((tickScale[ticksElapsedSinceBeat] - 1) * 2 + 1), 800f / 480f, 0.1f, 100f);
                }
            }

            if ((acknowledgedBeatsElapsed != beatsElapsed && beatsElapsed % 16 == 0))
            {
                drumLoop.Play();
                world.Scale = new Vector3(0.96f, 0.96f, 0.96f);
            }

            // 3D DRAWING

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawModel(playerModel, world * Matrix.CreateRotationY(-offsetX / 4), view, projection); // draw player

            world.Scale = new Vector3(1);

            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    DrawModel(enemy.Model, enemy.WorldMatrix, view, projection);
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (((Enemy)enemies[i]).Health == 0 && ((Enemy)enemies[i]).Position.Z < 10)
                {
                    Vector3 pos = GraphicsDevice.Viewport.Project(((Enemy)enemies[i]).Position, projection, view, world);
                    spriteBatch.Draw(lockTexture, new Vector2(pos.X - (lockTexture.Width / 2), pos.Y - (lockTexture.Height / 2)), Color.White);
                }
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            DrawModel(playerModel, world * Matrix.CreateRotationY(-offsetX / 4), view, projection); // draw player

            spriteBatch.Draw(cursorTexture, cursorPosition, Color.White);

            spriteBatch.End();
        }
    }
}
