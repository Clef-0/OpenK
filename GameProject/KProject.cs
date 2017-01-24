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
        enum GameState {Menu, Rail}
        GameState currentState = GameState.Menu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // render resolution
        private int resolutionX;
        private int resolutionY;

        // time signature
        private int bpm = 130;

        private float ticksPerBeat;
        private int beatsElapsed = 0;
        private int acknowledgedBeatsElapsed = 0;
        private int ticksElapsedSinceBeat = 0;
        private long startingTime = DateTime.Now.Ticks;

        // cursor variables
        private MouseState oldMouseState;
        private int mouseX;
        private int mouseY;
        private int lockedEnemies = 0;

        public static readonly Random Rnd = new Random((int)DateTime.Now.Ticks);

        // particles
        ParticleEffect pEffect;

        // Audio
        private SoundEffect drumLoop;

        // 3D Models
        private static Model playerModel;
        public static Model droneModel { get; set; }
        public static Model sentinelModel { get; set; }
        public static Model colonelModel { get; set; }

        // Textures
        private Texture2D enemyParticleTexture;
        private Texture2D lockTexture;
        private Texture2D frameTexture;
        private Texture2D buttonPlayTexture;
        private Texture2D buttonResetTexture;
        private Texture2D buttonOptionsTexture;
        private Texture2D buttonExitTexture;
        private Texture2D cursorTexture;
        private Texture2D cursorRail;
        private Texture2D cursorMenu;
        private Texture2D vignetteTexture;
        private Vector2 cursorPosition;
        private SpriteFont Arial12;

        private string[] log = {"", "", "", "", ""};
        private string logEntry;
        private bool newEntry = false;

        private Matrix playerPos = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix camera = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(0, 3, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        Camera2D pCamera;

        private float offsetX;
        private float offsetY;

        private int flipMultiplier = -1;

        Node rootNode = new Node {
            Type = Node.NodeType.Cabal,
            Company = "TestCompany",
            Country = "TestCountry",
            Address = "000.000.000.000",
            Parent = null,
            Children =
            {
                new Node
                {
                    Type = Node.NodeType.Cabal,
                    Company = "TestCompany2",
                    Country = "TestCountry2",
                    Address = "000.000.000.000"
                },
                new Node
                {
                    Type = Node.NodeType.Cabal,
                    Company = "TestCompany3",
                    Country = "TestCountry3",
                    Address = "000.000.000.000"
                },
                new Node
                {
                    Type = Node.NodeType.Cabal,
                    Company = "TestCompany4",
                    Country = "TestCountry4",
                    Address = "000.000.000.000"
                }
            }
        };
        
        private List<object> enemies = new List<object>();

        

        public KProject()
        {
            graphics = new GraphicsDeviceManager(this);
            resolutionX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            resolutionY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = resolutionX;
            graphics.PreferredBackBufferHeight = resolutionY;
            graphics.IsFullScreen = true;
            Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), (float)resolutionX / (float)resolutionY, 0.1f, 200f);

            Content.RootDirectory = "Content";

            ticksPerBeat = ((float)60 / (float)bpm) * (float)10000000;

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
            drumLoop = Content.Load<SoundEffect>(@"Audio\dubloop");

            // Fonts
            Arial12 = Content.Load<SpriteFont>(@"Fonts\Arial12");

            // 3D Models
            playerModel = this.Content.Load<Model>(@"3D Models\Player\player");
            droneModel = this.Content.Load<Model>(@"3D Models\Test\Cube");
            sentinelModel = this.Content.Load<Model>(@"3D Models\Test\Cube");
            colonelModel = this.Content.Load<Model>(@"3D Models\Test\Cube");

            // Textures
            enemyParticleTexture = Content.Load<Texture2D>(@"Textures\EnemyParticle");
            frameTexture = Content.Load<Texture2D>(@"Textures\WindowFrame");
            buttonPlayTexture = Content.Load<Texture2D>(@"Textures\WindowButtonPlay");
            buttonResetTexture = Content.Load<Texture2D>(@"Textures\WindowButtonReset");
            buttonOptionsTexture = Content.Load<Texture2D>(@"Textures\WindowButtonOptions");
            buttonExitTexture = Content.Load<Texture2D>(@"Textures\WindowButtonExit");
            cursorTexture = Content.Load<Texture2D>(@"Textures\Cursor_Menu");
            cursorRail = Content.Load<Texture2D>(@"Textures\Cursor");
            cursorMenu = Content.Load<Texture2D>(@"Textures\Cursor_Menu");
            lockTexture = Content.Load<Texture2D>(@"Textures\Lock");
            vignetteTexture = Content.Load<Texture2D>(@"Textures\Vignette");

            ParticleInit(new TextureRegion2D(enemyParticleTexture)); 
        }

        protected override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    MenuUpdate(gameTime);
                    break;
                case GameState.Rail:
                    RailGameplayUpdate(gameTime);
                    break;
            }

            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    MenuDraw(gameTime);
                    break;
                case GameState.Rail:
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
                }
                mesh.Draw();
            }
        }
    }
}
