using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
using System.Threading;
using System.Threading.Tasks;

namespace GameProject
{
    public partial class KProject : Game
    {
        private string[] log = { "", "", "", "", "" };
        private string logEntry;
        private bool newEntry = false;

        private List<object> enemies = new List<object>();

        private Color currentNodeColor = Color.FromNonPremultiplied(160, 64, 0, 255);
        private decimal marathonHue = 0;
        private NodeMusic currentNodeMusic = NodeMusic.Fear;
        private int currentNodeScore = 0;
        private int currentNodeSpawned = 0;
        private int currentNodeShot = 0;
        private decimal currentNodePercentage;
        private Point currentNodeLocation;
        private bool currentNodeCompleted = false;

        private int flipMultiplier = -1;

        // selected music
        private SoundEffect drum;
        private SoundEffect bass;
        private SoundEffect pads;
        private SoundEffect soul;
        private SoundEffect journey;
        private SoundEffect break1;

        private void RailGameplayUpdate(GameTime gameTime)
        {
            if (acknowledgedBeatsElapsed != beatsElapsed && currentNodeMusic == NodeMusic.Fear)
            {
                SoundEffectInstance drumInst = drum.CreateInstance();
                SoundEffectInstance padsInst = pads.CreateInstance();
                SoundEffectInstance journeyInst = journey.CreateInstance();
                SoundEffectInstance bassInst = bass.CreateInstance();
                SoundEffectInstance break1Inst = break1.CreateInstance();
                SoundEffectInstance soulInst = soul.CreateInstance();

                if (beatsElapsed >= 16 && beatsElapsed % 4 == 0 && beatsElapsed != 80 && beatsElapsed != 32)
                {
                    drumInst.Volume /= 3f;
                    drumInst.Play();
                }

                if (beatsElapsed >= 0 && beatsElapsed % 16 == 0 && beatsElapsed < 32)
                {
                    padsInst.Volume /= 3f;
                    padsInst.Play();
                }

                // activate music if reached score

                if (beatsElapsed == 32 && currentNodeScore > 100)
                {
                    journeyInst.Volume /= 3f;
                    journeyInst.Play();
                }

                if (beatsElapsed >= 36 && (beatsElapsed - 4) % 32 == 0 && currentNodeScore > 100)
                {
                    bassInst.Volume /= 3f;
                    bassInst.Play();
                }

                // activate music if reached score

                if (beatsElapsed == 80 && currentNodeScore > 200)
                {
                    break1Inst.Volume /= 3f;
                    break1Inst.Play();
                }

                if (beatsElapsed == 84 && (beatsElapsed + 12) % 32 == 0 && currentNodeScore > 200)
                {
                    soulInst.Volume /= 3f;
                    soulInst.Play();
                }
                if ((beatsElapsed + 12) % 32 == 0 && currentNodeScore >= 3000)
                {
                    if (currentState == GameState.Marathon)
                    {
                        soulInst.Volume /= 3f;
                        soulInst.Play();
                    }
                    else
                    {
                        currentNodePercentage = decimal.Divide(currentNodeShot, currentNodeSpawned) * 100; // calculate shot down percentage
                        currentNodeCompleted = true; // set node to complete
                        currentState = GameState.Map;
                        cursorTexture = cursorMenu;
                    }
                }
            }

            if (beatsElapsed == 35)
            {
                zoomFactor = 1f + (ticksElapsedSinceBeat / 1000f);
            }
            else
            {
                zoomFactor = 1f;
            }

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

            pEffectExplosion.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            pEffectLock.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // location to draw cursor sprite
            cursorPosition = new Vector2(mouseX - 37, mouseY - 37);

            MouseState newMouseState = Mouse.GetState();


            // enemy spawners
            if (beatsElapsed != acknowledgedBeatsElapsed && beatsElapsed % 1 == 0 && beatsElapsed > 0)
            {
                if (currentNodeScore < 400 && beatsElapsed % 4 == 0)
                {
                    enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                    enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                    enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                    enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                    currentNodeSpawned += 4;
                }
                if (currentNodeScore >= 400 && beatsElapsed % 4 == 0 && currentNodeScore < 2000)
                {
                    enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                    enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                    currentNodeSpawned += 4;
                }
                if (currentNodeScore >= 1000 && beatsElapsed % 4 == 0 && currentNodeScore < 2000)
                {
                    enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                    enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                    enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    currentNodeSpawned += 4;
                }
                if (currentNodeScore >= 2000 && beatsElapsed % 4 == 0)
                {
                    if (currentState == GameState.Rail && currentNodeScore < 2500)
                    {
                        enemies.Add(new Sentinel(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0, 0, 0), EnemyFlightMode.CurveOut));
                        enemies.Add(new Sentinel(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0, 0, 0), EnemyFlightMode.CurveOut));
                        enemies.Add(new Sentinel(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0, 0, 0), EnemyFlightMode.CurveOut));
                        currentNodeSpawned += 3;
                    }
                    else if (currentState == GameState.Marathon || currentNodeScore >= 2500)
                    {
                        int selector = Rnd.Next(1,9);
                        switch (selector)
                        {
                            case 1:
                            enemies.Add(new Sentinel(new Vector3(-11, 5, -150), new Vector3(0, 0, 0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Sentinel(new Vector3(11, 5, -150), new Vector3(0, 0, 0), EnemyFlightMode.CurveOut));
                            currentNodeSpawned += 2;
                            break;
                            case 2:
                            enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            currentNodeSpawned += 4;
                            break;
                            case 3:
                            enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            currentNodeSpawned += 4;
                            break;
                            case 4:
                            enemies.Add(new Drone(new Vector3(-5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                            enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            currentNodeSpawned += 3;
                            break;
                            case 5:
                            enemies.Add(new Sentinel(new Vector3(-5 * flipMultiplier, 5, -150), new Vector3(0, 0, 0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(5 * flipMultiplier, -2, -150), new Vector3(0), EnemyFlightMode.Straight));
                            enemies.Add(new Drone(new Vector3(10 * flipMultiplier, 3, -150), new Vector3(0), EnemyFlightMode.Straight));
                            enemies.Add(new Drone(new Vector3(15 * flipMultiplier, 8, -150), new Vector3(0), EnemyFlightMode.Straight));
                            currentNodeSpawned += 4;
                            break;
                            case 6:
                            enemies.Add(new Drone(new Vector3(5, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                            enemies.Add(new Drone(new Vector3(5, 0, -150), new Vector3(0), EnemyFlightMode.Straight));
                            enemies.Add(new Drone(new Vector3(-5, 5, -150), new Vector3(0), EnemyFlightMode.Straight));
                            enemies.Add(new Drone(new Vector3(-5, 0, -150), new Vector3(0), EnemyFlightMode.Straight));
                            currentNodeSpawned += 4;
                            break;
                            case 7:
                            enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(-5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(-11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(-17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(-23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            currentNodeSpawned += 8;
                            break;
                            case 8:
                            enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                            enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 5, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    enemies.Add(new Drone(new Vector3(5 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    enemies.Add(new Drone(new Vector3(11 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                    enemies.Add(new Drone(new Vector3(17 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveOut));
                    enemies.Add(new Drone(new Vector3(23 * flipMultiplier, 0, -150), new Vector3(0), EnemyFlightMode.CurveDown));
                            currentNodeSpawned += 8;
                            break;
                        }
                    }
                }
            }

            List<int> indicesToCull = new List<int>();
            bool moreThanOneEnemyLocked = false;
            
            for (int i = enemies.Count() - 1; i >= 0; i--)
            {
                if (enemies[i] != null)
                {
                    ((Enemy)enemies[i]).Update();
                    ((Enemy)enemies[i]).WorldMatrix = Matrix.CreateRotationX(((Enemy)enemies[i]).Rotation.X)
                        * Matrix.CreateRotationY(((Enemy)enemies[i]).Rotation.Y)
                        * Matrix.CreateRotationZ(((Enemy)enemies[i]).Rotation.Z)
                        * Matrix.CreateTranslation(((Enemy)enemies[i]).Position);

                    if (mouseInView && newMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (CheckRay(new Vector2(mouseX, mouseY), // mouse position
                            ((Enemy)enemies[i]).Model, // enemy model
                            ((Enemy)enemies[i]).WorldMatrix, // enemy matrix for accurate 3D position
                            view,
                            projection,
                            this.GraphicsDevice.Viewport) && ((Enemy)enemies[i]).Health > 0 && lockedEnemies < 8)
                        {
                            logEntry = ((Enemy)enemies[i]).GetType().Name.ToLower();
                            newEntry = true;
                            ((Enemy)enemies[i]).Injure(1);
                            lockedEnemies += 1;
                            moreThanOneEnemyLocked = true;
                            Vector3 pos = GraphicsDevice.Viewport.Project(((Enemy)enemies[i]).WorldMatrix.Translation, projection, view, world);
                            pEffectLock.Trigger(new Vector2(pos.X, pos.Y));
                        }
                    }
                    if (enemies.ElementAtOrDefault(i) != null)
                    {
                        if (((Enemy)enemies[i]).Position.Z > 10)
                        {
                            if (((Enemy)enemies[i]).Health == 0)
                            {
                                if (((Enemy)enemies[i]).GetType().Name.ToLower() == "drone")
                                {
                                    lockedEnemies -= 1;
                                }
                                else
                                {
                                    lockedEnemies -= 2;
                                }
                                currentNodeScore += ((Enemy)enemies[i]).Points;
                                currentNodeShot += 1;
                            }
                            indicesToCull.Add(i);
                            

                            // end marathon mode if enemy missed
                            if (currentState == GameState.Marathon)
                            {
                                currentState = GameState.Menu;
                                cursorTexture = cursorMenu;
                                System.Windows.Forms.MessageBox.Show("Your score was " + currentNodeScore + ".", "OpenK");
                            }
                        }
                    }
                }
            }
            if (moreThanOneEnemyLocked == true)
            {
                SoundEffectInstance inst = snareDrum.CreateInstance();
                inst.Volume /= 10f;
                inst.Pitch *= 10f;
                inst.Play();
            }

            offsetX = (mouseX - ((float)GraphicsDevice.Viewport.Width / 2)) / 1000;
            offsetY = (mouseY - ((float)GraphicsDevice.Viewport.Height / 2)) / 500;

            //tilt camera with mouse move
            view = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(offsetX, 3, offsetY), Vector3.UnitY);

            oldMouseState = newMouseState;

            ticksElapsedSinceBeat++;
            
            // events to occur only once per beat
            if (acknowledgedBeatsElapsed != beatsElapsed)
            {
                //acknowledgedBeatsElapsed = beatsElapsed;
                ticksElapsedSinceBeat = 0;

                // destroy enemies on beat

                int enemiesDestroyed = 0;
                int enemiesDestroyedScore = 0;
                for (int i = enemies.Count() - 1; i >= 0; i--)
                {
                    if (((Enemy)enemies[i]).Health == 0 && newMouseState.LeftButton == ButtonState.Released)
                    {
                        Vector3 pos = GraphicsDevice.Viewport.Project(((Enemy)enemies[i]).WorldMatrix.Translation, projection, view, world);
                        pEffectExplosion.Trigger(new Vector2(pos.X, pos.Y));
                        enemiesDestroyedScore += ((Enemy)enemies[i]).Points;
                        indicesToCull.Add(i);
                        lockedEnemies = 0;
                        currentNodeShot += 1;
                        enemiesDestroyed++;
                    }
                }
                if (enemiesDestroyed > 0)
                {
                    SoundEffectInstance boomInst = boomDrum.CreateInstance();
                    boomInst.Volume /= 3f;
                    boomInst.Play();
                    enemiesDestroyedScore += (enemiesDestroyed - 1) * 5;
                    currentNodeScore += enemiesDestroyedScore;
                }

                if (beatsElapsed % 8 == 0)
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

            foreach (int index in indicesToCull)
            {
                if (enemies[index] != null)
                {
                    enemies.RemoveAt(index);
                }
            }


            // beat bar stuff
            float currentTime = timer.Now.Ticks - startingTime;
            beatsElapsed = (int)Math.Truncate((decimal)currentTime / (decimal)ticksPerBeat) - 1;
        }

        private void RailGameplayDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(currentNodeColor);

            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: pCamera.GetViewMatrix());

            spriteBatch.Draw(pEffectExplosion);
            spriteBatch.Draw(pEffectLock);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(vignetteTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black);

            if (newEntry)
            {
                newEntry = false;
                log[0] = log[1];
                log[1] = log[2];
                log[2] = log[3];
                log[3] = log[4];
                log[4] = logEntry;
            }

            spriteBatch.DrawString(menuFont, log[0], new Vector2(5, 120), Color.FromNonPremultiplied(255, 255, 255, 105));
            spriteBatch.DrawString(menuFont, log[1], new Vector2(5, 140), Color.FromNonPremultiplied(255, 255, 255, 125));
            spriteBatch.DrawString(menuFont, log[2], new Vector2(5, 160), Color.FromNonPremultiplied(255, 255, 255, 145));
            spriteBatch.DrawString(menuFont, log[3], new Vector2(5, 180), Color.FromNonPremultiplied(255, 255, 255, 165));
            spriteBatch.DrawString(menuFont, log[4], new Vector2(5, 200), Color.FromNonPremultiplied(255, 255, 255, 255));


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
                    projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f / zoomFactor) / ((tickScale[ticksElapsedSinceBeat] - 1) * 1.1f + 1), resolutionX / resolutionY, 0.1f, 3000f);
                }
            }

            if ((acknowledgedBeatsElapsed != beatsElapsed && beatsElapsed % 16 == 0))
            {
                world.Scale = new Vector3(0.96f, 0.96f, 0.96f);
            }

            // 3D DRAWING

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawModel(PlayerModel, world * Matrix.CreateScale(0.5f, 1f, 1f) * Matrix.CreateRotationY(-offsetX / 4), view, projection); // draw player additive echo
            
            world.Scale = new Vector3(1);

            DrawModel(TerrainModel, world * Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateScale(1f, .1f, 1f) * Matrix.CreateTranslation(0f, -80f, -70f + (ticksElapsedSinceBeat) / 2f), view, projection);

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
                    Vector3 pos = GraphicsDevice.Viewport.Project(((Enemy)enemies[i]).WorldMatrix.Translation, projection, view, world);
                    spriteBatch.Draw(lockTexture, new Vector2(pos.X - (lockTexture.Width / 2), pos.Y - (lockTexture.Height / 2)), Color.White);
                }
            }

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            
            DrawModel(PlayerModel, world * Matrix.CreateScale(new Vector3(0.5f, 1f, 1f)) * Matrix.CreateRotationY(-offsetX / 4), view, projection); // draw player overlay

            spriteBatch.Draw(cursorTexture, cursorPosition, Color.White);
            spriteBatch.DrawString(scoreFont, currentNodeScore.ToString(), new Vector2(GraphicsDevice.Viewport.Width - scoreFont.MeasureString(currentNodeScore.ToString()).X - 5, GraphicsDevice.Viewport.Height - scoreFont.MeasureString(currentNodeScore.ToString()).Y + 15), Color.White);

            if (currentState == GameState.Marathon)
            {
                if (marathonHue <= 1)
                {
                    marathonHue = 370M;
                }
                else
                {
                    marathonHue -= 1M;
                }
                currentNodeColor = new HslColor((float)marathonHue, 0.1f, currentNodeColor.ToHsl().L).ToRgb();
            }

            spriteBatch.End();
        }
    }
}
