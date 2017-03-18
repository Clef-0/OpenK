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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    public partial class KProject : Game
    {
        enum GameState { Menu, Map, Rail, Marathon };
        GameState currentState = GameState.Menu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        PrecisionTimer timer = new PrecisionTimer();

        // render resolution
        private int resolutionX;
        private int resolutionY;

        // timing variables
        private int bpm = 90;
        private float ticksPerBeat;
        private int beatsElapsed = 0;
        private int acknowledgedBeatsElapsed = 0;
        private int ticksElapsedSinceBeat = 0;
        private long startingTime = 0;

        // cursor variables
        private MouseState oldMouseState;
        private int mouseX;
        private int mouseY;

        private int lockedEnemies = 0;

        public static readonly Random Rnd = new Random((int)DateTime.Now.Ticks);

        // particles
        ParticleEffect pEffectExplosion;
        ParticleEffect pEffectLock;

        // Audio
        private SoundEffect fearDrumLoop;
        private SoundEffect fearBassLoop;
        private SoundEffect fearPadsLoop;
        private SoundEffect fearSoulLoop;
        private SoundEffect fearJourneyLoop;
        private SoundEffect fearBreak1Loop;

        private SoundEffect boomDrum;
        private SoundEffect snareDrum;

        private SoundEffect drum;
        private SoundEffect bass;
        private SoundEffect pads;
        private SoundEffect soul;
        private SoundEffect journey;
        private SoundEffect break1;

        // 3D Models
        private static Model PlayerModel;
        public static Model DroneModel { get; set; }
        public static Model SentinelModel { get; set; }
        public static Model ColonelModel { get; set; }
        private static Model TerrainModel;

        // Textures
        private Texture2D enemyParticleTexture;
        private Texture2D lockParticleTexture;
        private Texture2D lockTexture;
        private Texture2D frameTexture;
        private Texture2D linePixel;
        private Texture2D buttonPlayTexture;
        private Texture2D buttonResetTexture;
        private Texture2D buttonOptionsTexture;
        private Texture2D buttonExitTexture;
        private Texture2D cursorTexture;
        private Texture2D cursorRail;
        private Texture2D cursorMenu;
        private Texture2D vignetteTexture;
        private Texture2D nodeCircle;
        private Vector2 cursorPosition;
        private SpriteFont Arial12;
        private SpriteFont scoreFont;
        private SpriteFont menuFont;

        private string[] log = {"", "", "", "", ""};
        private string logEntry;
        private bool newEntry = false;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(0, 3, 0), Vector3.UnitY);
        private Matrix projection;
        private float zoomFactor = 1f;

        Camera2D pCamera;

        private float offsetX;
        private float offsetY;

        private int flipMultiplier = -1;

        Node rootNode;

        private const int railNodesToMake = 30;
        private const int cabalNodesToMake = 10;
        private const int miniNodesToMake = 10;
        private const int totalNodesToMake = railNodesToMake + cabalNodesToMake + miniNodesToMake;
        private int railNodesMade = 1;
        
        private List<object> enemies = new List<object>();
   
        private Color currentNodeColor = Color.FromNonPremultiplied(160, 64, 0, 255);
        private NodeMusic currentNodeMusic = NodeMusic.Fear;
        private int currentNodeScore = 0;
        private int currentNodeSpawned = 0;
        private int currentNodeShot = 0;
        private Point currentNodeLocation;
        private bool currentNodeCompleted = false;

        private Decimal marathonHue = 0;

        public KProject()
        {
            graphics = new GraphicsDeviceManager(this);
            resolutionX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            resolutionY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = resolutionX;
            graphics.PreferredBackBufferHeight = resolutionY;
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = true;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), resolutionX / resolutionY, 0.1f, 3000f);

            Content.RootDirectory = "Content";

            ticksPerBeat = ((float)60 / (float)bpm) * (float)10000000;

            if (File.Exists("save.dat"))
            {
                LoadGame();
            }
            else
            {
                rootNode = new Node
                {
                    Type = NodeType.Rail,
                    Company = NameGenerate(1),
                    Country = NameGenerate(2),
                    Address = NameGenerate(3)
                };

                CreateChildren(rootNode, 3, 4, 0);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            mouseX = GraphicsDevice.Viewport.Width / 2;
            mouseY = GraphicsDevice.Viewport.Height / 2;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, resolutionX, resolutionY); 
            pCamera = new Camera2D(viewportAdapter);

            // Audio
            fearDrumLoop = Content.Load<SoundEffect>(@"Audio\fear\clean beat");
            fearBassLoop = Content.Load<SoundEffect>(@"Audio\fear\gross bass");
            fearPadsLoop = Content.Load<SoundEffect>(@"Audio\fear\intro pads");
            fearSoulLoop = Content.Load<SoundEffect>(@"Audio\fear\california soul");
            fearJourneyLoop = Content.Load<SoundEffect>(@"Audio\fear\journey pad");
            fearBreak1Loop = Content.Load<SoundEffect>(@"Audio\fear\break 1");
            boomDrum = Content.Load<SoundEffect>(@"Audio\fear\boom");
            snareDrum = Content.Load<SoundEffect>(@"Audio\snare");

            // Fonts
            Arial12 = Content.Load<SpriteFont>(@"Fonts\Arial12");
            scoreFont = Content.Load<SpriteFont>(@"Fonts\ScoreFont");
            menuFont = Content.Load<SpriteFont>(@"Fonts\Bender");

            // 3D Models
            PlayerModel = this.Content.Load<Model>(@"3D Models\Player\player");
            DroneModel = this.Content.Load<Model>(@"3D Models\Test\Ship");
            SentinelModel = this.Content.Load<Model>(@"3D Models\Test\Sentinel");
            ColonelModel = this.Content.Load<Model>(@"3D Models\Test\Cube");
            TerrainModel = this.Content.Load<Model>(@"3D Models\Env\Terrain");

            // Textures
            enemyParticleTexture = Content.Load<Texture2D>(@"Textures\Slice");
            lockParticleTexture = Content.Load<Texture2D>(@"Textures\SliceLock");
            frameTexture = Content.Load<Texture2D>(@"Textures\WindowFrame");
            buttonPlayTexture = Content.Load<Texture2D>(@"Textures\WindowButtonPlay");
            buttonResetTexture = Content.Load<Texture2D>(@"Textures\WindowButtonReset");
            buttonOptionsTexture = Content.Load<Texture2D>(@"Textures\WindowButtonOptions");
            buttonExitTexture = Content.Load<Texture2D>(@"Textures\WindowButtonExit");
            cursorTexture = Content.Load<Texture2D>(@"Textures\Cursor_Menu");
            cursorRail = Content.Load<Texture2D>(@"Textures\Cursor");
            cursorMenu = Content.Load<Texture2D>(@"Textures\Cursor_Menu");
            linePixel = Content.Load<Texture2D>(@"Textures\Line");
            lockTexture = Content.Load<Texture2D>(@"Textures\Lock");
            vignetteTexture = Content.Load<Texture2D>(@"Textures\Vignette");
            nodeCircle = Content.Load<Texture2D>(@"Textures\NodeCircle");

            ParticleInitialise(new TextureRegion2D(enemyParticleTexture), new TextureRegion2D(lockParticleTexture));

            bass = fearBassLoop;
            drum = fearDrumLoop;
            pads = fearPadsLoop;
            soul = fearSoulLoop;
            journey = fearJourneyLoop;
            break1 = fearBreak1Loop;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (currentState)
            {
                case GameState.Menu:
                case GameState.Map:
                    MenuUpdate(gameTime);
                    break;
                case GameState.Rail:
                case GameState.Marathon:
                    RailGameplayUpdate(gameTime);
                    break;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                currentState = GameState.Menu;
                cursorTexture = cursorMenu;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    MenuDraw(gameTime);
                    break;
                case GameState.Map:
                    MenuDraw(gameTime);
                    break;
                case GameState.Rail:
                    RailGameplayDraw(gameTime);
                    break;
                case GameState.Marathon:
                    RailGameplayDraw(gameTime);
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.FogEnabled = true;
                    effect.FogColor = new HslColor(currentNodeColor.ToHsl().H, currentNodeColor.ToHsl().S, 0).ToRgb().ToVector3();
                    effect.FogStart = 50f;
                    effect.FogEnd = 180f;
                }
                mesh.Draw();
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            SaveGame();
        }
    }
}
