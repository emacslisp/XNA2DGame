using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PlatformerReading
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class Game1 : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private SpriteFont hudFont;

    private Texture2D pong;
    private Vector2 pongPosition;
    private Vector2 pongSpeed;
    private int levelIndex = -1;
    private Level level;
    private Texture2D pad;
    private Texture2D help;
    private Texture2D toppad;
    //    private Texture2D background;
    private List<Texture2D> backgrounds = new List<Texture2D>();
    private Texture2D gamestart;
    private Texture2D gameStartBackgroud;
    private Random rnd = new Random((int)DateTime.Now.Ticks);    

    private List<Dollar> dollarList = new List<Dollar>();
    private List<int> scoreList = new List<int>();    
    public int numOfBallLives = 4;
    public int score = 0;
    public string resultPath;
    bool pause = false;         // is the "pause" button is pressed.
    bool playBackGround = false; 
    bool exitOrNot = false;     // is ball reach hole.
    bool popup = false;         // is "you_win" or "you_die" popup
    bool reachLastHole = false; // is the ball reach last hole.
    public Dictionary<string,Keys> dict = new Dictionary<string,Keys>();

    float timef =0;
    int currentItem = 0;

    
    AudioEngine audioEngine;
    WaveBank waveBank;
    SoundBank soundBank;
    

    static readonly Rectangle
      ExtermePongLogoExterme = new Rectangle(98,65,267,60),
      ExtermePongLogoPong = new Rectangle(269,144,170,60),
      ExtermePongLogoSmall = new Rectangle(395,208,100,63),
      gameMenuStart = new Rectangle(184,278,206,43),
      gameMenuHelp = new Rectangle(226,346,124,43);

    enum GameMode               // for different gameMode
    {
      Menu,
      Game,
      GameOver,
      Help,
      iniFileError,             // If the input error in ini files
    }

    GameMode gameMode = GameMode.Menu;
    
    public Game1()
      {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
      }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // TODO: Add your initialization logic here
      graphics.PreferredBackBufferWidth = 640;
      graphics.PreferredBackBufferHeight = 480;
      graphics.ApplyChanges();
      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected void InitDictionary()
    {
      ConfigFile cf = new ConfigFile("main.ini");
      dict = cf.LoadFromFile();
      if(!checkDict())
        gameMode = GameMode.iniFileError;
    }

    protected bool checkDict()
    {
      if(dict.ContainsKey("up")&&dict.ContainsKey("down")&&dict.ContainsKey("left")&&dict.ContainsKey("right")&&dict.ContainsKey("pause")&&dict.ContainsKey("quit"))
        return true;
      return false;
      
    }
    
    
    protected void InitSound()
    {
      audioEngine = new AudioEngine("Content\\Sound\\sound.xgs");
      waveBank = new WaveBank(audioEngine,"Content\\Sound\\Wave Bank.xwb");
      if(waveBank != null)
        {
          soundBank = new SoundBank(audioEngine,"Content\\Sound\\Sound Bank.xsb");
        }
    }

    
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      InitSound();
      InitDictionary();
      spriteBatch = new SpriteBatch(GraphicsDevice);
      //      background = Content.Load<Texture2D>("background");
      for(int i = 1;i <= 6;i++)
        {
          backgrounds.Add(Content.Load<Texture2D>("Backgroud\\b" + i.ToString()));
        }
      gamestart = Content.Load<Texture2D>("GameMenu\\gamestart");
      help = Content.Load<Texture2D>("GameMenu\\help");
      gameStartBackgroud = Content.Load<Texture2D>("GameMenu\\gameStartBackgroud");
      //      pongSpeed = new Vector2(4,5);
      hudFont = Content.Load<SpriteFont>("Font\\Hud");
      LoadScore();
      //      pad = Content.Load<Texture2D>("pad");
      //      toppad = Content.Load<Texture2D>("toppad");


      // TODO: use this.Content to load your game content here
      //      LoadNextLevel();

    }

    private void LoadNextLevel()
    {
      string levelPath;

      while(true)
        {
          levelPath = String.Format("Level\\{0}.txt",++levelIndex);
          levelPath = Path.Combine(StorageContainer.TitleLocation,"Content/" + levelPath);
          if(File.Exists(levelPath))
            break;
          if(levelIndex>5)
            {
              reachLastHole = true;
              return;
            }

          if(levelIndex == 0)
            throw new Exception("No levels found.");
          levelIndex =  -1;
        }

      if(level != null)
        level.Dispose();

      level = new Level(Services,levelPath);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    KeyboardState last;
    KeyboardState keybs;
    bool wasSpacePressed = false;
    protected override void Update(GameTime gameTime)
    {
      KeyboardState keyboard = Keyboard.GetState();
      if(numOfBallLives <= 0||reachLastHole)
        {
          gameMode = GameMode.GameOver;
        }

      if(gameMode == GameMode.Game)
        {
          if(!playBackGround)
            {
              soundBank.PlayCue("background");
              playBackGround = !playBackGround;
            }
          if (last.IsKeyDown(dict["pause"])&&keyboard.IsKeyUp(dict["pause"]))
            {
              pause = !pause;
            }

      
          if(!pause)
            {
              if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
              //            MoveLeftPad();
              if(!popup)
                level.Update(gameTime);
      
              handleCollisionWithTarget();

              if(level.isBallDie)
                {
                  popup = true;
                  bool isSpaceButton = keyboard.IsKeyDown(Keys.Space);
                      
                  if(!wasSpacePressed&&isSpaceButton)
                    {
                      --levelIndex;
                      --numOfBallLives;
                      score = level.score;
                      LoadNextLevel();
                      level.score += score;
                      popup = false;
                    }
                  wasSpacePressed = isSpaceButton;
                }
            }
        }
      else if(gameMode == GameMode.GameOver)
        {
          if(level != null)
            level.Dispose();
          if (last.IsKeyDown(Keys.B)&&keyboard.IsKeyUp(Keys.B))
            {
              InitSound();
              gameMode = GameMode.Menu;
              numOfBallLives = 4;
              levelIndex = -1;
              reachLastHole = false;
            }
          if(level.isInsert)
            {
              for(int i = 0;i<9;i++)
                {
                  if(level.score>scoreList[i])
                    {
                      scoreList.Insert(i,level.score);
                      scoreList.RemoveAt(9);
                      break;
                    }
                }
              StreamWriter writer = new StreamWriter(resultPath);
              foreach(int j in scoreList)
                {
                  writer.Write(j.ToString() + "\r\n");
                }
              writer.Flush();
              writer.Close();
              level.isInsert = !level.isInsert;
              
            }
        }
      last = keyboard;                
      base.Update(gameTime);      
    }


    private void DrawGameString()
    {
      Rectangle titleSafeArea = new Rectangle(20,20,40,20);
      Vector2 hudLocation = new Vector2(titleSafeArea.X,titleSafeArea.Y);
      Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width/2.0f,titleSafeArea.Y + titleSafeArea.Height/2.0f);
      Color lifeColor;

      if(numOfBallLives>1)
        {
          lifeColor = Color.Yellow;
        }
      else
        {
          lifeColor = Color.Red;
        }

      
      DrawShadowedString(hudFont,"Live: " + numOfBallLives.ToString(),hudLocation,lifeColor);
      DrawShadowedString(hudFont,"SCORE: " + level.score.ToString(),hudLocation + new Vector2(80,0),Color.Yellow);

      Texture2D status = null;
      if(exitOrNot&&popup)
        {
          spriteBatch.Draw(Content.Load<Texture2D>("GameMenu\\you_win"),new Vector2(200,160),Color.White);
          level.pongSpeed = Vector2.Zero;
        }
      else if(level.isBallDie&&popup)
        {
          spriteBatch.Draw(Content.Load<Texture2D>("GameMenu\\you_die"),new Vector2(200,160),Color.White);
          level.pongSpeed = Vector2.Zero;
        }
    }

    private void DrawShadowedString(SpriteFont font,string value,Vector2 position,Color color)
    {
      spriteBatch.DrawString(font,value,position + new Vector2(2.0f,2.0f),Color.Black);
      spriteBatch.DrawString(font,value,position,color);
    }
    
    protected void handleCollisionWithTarget()
    {
      exitOrNot = level.dealWithCollisionWithTarget();
      if(exitOrNot)
        {
          if(level.goToNextLevel)
            soundBank.PlayCue("applause");

          KeyboardState keyboard = Keyboard.GetState();                
          bool isSpaceButton = keyboard.IsKeyDown(Keys.Space);
          popup = true;
          level.goToNextLevel = false;
          if(!wasSpacePressed&&isSpaceButton)
            {
              score = level.score;
              LoadNextLevel();
              level.score = score;
              popup = false;
            }
          wasSpacePressed = isSpaceButton;
        }
    }

    protected void LoadScore()
    {
      resultPath = String.Format("Level\\result.txt");
      resultPath = Path.Combine(StorageContainer.TitleLocation,"Content/" + resultPath);
      if(File.Exists(resultPath))
        {
          List<string> lines = new List<string>();
          using(StreamReader reader = new StreamReader(resultPath))
            {
              string line = reader.ReadLine();
              while(line != null)
                {
                  scoreList.Add(Convert.ToInt32(line));
                  line = reader.ReadLine();
                }
              reader.Close();
            }
        }
      else 
        {
          for(int i = 0;i<9;i++)
            {
              scoreList.Add(0);
            }
        }
    }
    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      if(gameMode == GameMode.Game)
        {
          if(levelIndex > 5)
            return;
          spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
          
          spriteBatch.Draw(backgrounds[levelIndex],new Vector2(0,0),Color.White);

          spriteBatch.End();
      
          spriteBatch.Begin();
          level.Draw(gameTime,spriteBatch);
          DrawGameString();            
          spriteBatch.End();
        }
      else if(gameMode == GameMode.Menu)
        {
          spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
          spriteBatch.Draw(gameStartBackgroud,Vector2.Zero,Color.White);
          spriteBatch.Draw(gamestart,ExtermePongLogoExterme,ExtermePongLogoExterme,Color.White);
          spriteBatch.Draw(gamestart,ExtermePongLogoPong,ExtermePongLogoPong,Color.White);

        
          timef += gameTime.ElapsedGameTime.Milliseconds;
          if(2000>=timef&&timef >= 1000)
            {
              spriteBatch.Draw(gamestart,ExtermePongLogoSmall,ExtermePongLogoSmall,Color.White);
            }
          else if(timef>2000)
            {
              timef = 0f;
            }
          last = Keyboard.GetState();
          //          if (last.IsKeyDown(Keys.Down)&&keybs.IsKeyUp(Keys.Down))
          if (last.IsKeyDown(dict["down"])&&keybs.IsKeyUp(dict["down"]))          
            {
              currentItem = (currentItem + 1)%2;
              soundBank.PlayCue("plop");
            }
          //          if(last.IsKeyDown(Keys.Up)&&keybs.IsKeyUp(Keys.Up))
          if(last.IsKeyDown(dict["up"])&&keybs.IsKeyUp(dict["up"]))          
            {
              currentItem = (currentItem + 1)%2;
              soundBank.PlayCue("plop");
            }

          if(last.IsKeyDown(Keys.Enter)&&keybs.IsKeyUp(Keys.Enter))
            {
              if(currentItem == 0)
                {
                  gameMode = GameMode.Game;
                  LoadNextLevel();
                }
              else if(currentItem == 1)
                {
                  gameMode = GameMode.Help;
                }
            }
          keybs = last;

          spriteBatch.Draw(gamestart,gameMenuStart,gameMenuStart,currentItem == 0?Color.Orange:Color.White);
          spriteBatch.Draw(gamestart,gameMenuHelp,gameMenuHelp,currentItem == 1?Color.Orange:Color.White);
          spriteBatch.End();
        }
      else if(gameMode == GameMode.GameOver)
        {
          soundBank.Dispose();
          playBackGround = false;
          spriteBatch.Begin();
          spriteBatch.Draw(gameStartBackgroud,Vector2.Zero,Color.White);
          //          DrawShadowedString(hudFont,"Game Over",new Vector2(200,40),Color.Red);
          spriteBatch.Draw(Content.Load<Texture2D>("GameMenu\\gameover"),new Vector2(200,50),Color.White);
          for(int i = 0;i<9;i++)
            {
              DrawShadowedString(hudFont,(i + 1).ToString() + "         " + scoreList[i].ToString(),new Vector2(270,120 + 20*i),Color.Orange);
            }
          DrawShadowedString(hudFont,"Press B Back to Menu",new Vector2(210,330),Color.Red);                        
          spriteBatch.End();

        }
      else if(gameMode == GameMode.Help)
        {
          spriteBatch.Begin();
          spriteBatch.Draw(help,Vector2.Zero,Color.White);
          spriteBatch.End();
          last = Keyboard.GetState();
          if (last.IsKeyDown(Keys.Q)&&keybs.IsKeyUp(Keys.Q))
            {
              gameMode = GameMode.Menu;
            }
          keybs = last;          
        }
      else if(gameMode == GameMode.iniFileError)
        {
          spriteBatch.Begin();          
          spriteBatch.DrawString(hudFont,"The ini file is parsered incorrectly.Please close the game and check again.",new Vector2(0,graphics.PreferredBackBufferHeight/2),Color.White);
          spriteBatch.End();          
        }
      
      /*      spriteBatch.Begin();
              spriteBatch.DrawString(hudFont,"Game Over",new Vector2(graphics.PreferredBackBufferWidth/2,graphics.PreferredBackBufferHeight/2),Color.White);
              spriteBatch.End();*/
      // TODO: Add your drawing code here

      base.Draw(gameTime);
    }
  }
}
