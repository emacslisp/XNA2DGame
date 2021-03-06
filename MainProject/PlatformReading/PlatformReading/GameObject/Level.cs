using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace PlatformReading
{
  /// <summary>
  /// This is a game component that implements IUpdateable.
  /// </summary>
  public class Level : IDisposable
  {
    private Block block;
    private Block[,] blocks;
    private List<Block> blk = new List<Block>();
    private Point exit = InvalidPosition;
    private static readonly Point InvalidPosition = new Point( -1, -1);
    public  List<TopPad> toppads = new List<TopPad>();
    public  List<LeftPad> leftpads = new List<LeftPad>();
    public List<AnimationPlayer> animations = new List<AnimationPlayer>();
    public List<Target> targets = new List<Target>();
    public Ball ball;
    public List<Dollar> dollars = new List<Dollar>();
    public Vector2 pongPosition;
    public Vector2 pongSpeed = new Vector2(4,5);

    //public AudioEngine audioEngine;
    //public WaveBank waveBank;
    //public //soundBank //soundBank;

    
    Random rnd = new Random((int)DateTime.Now.Ticks);
    private List<BoundingBox> firedownBoxes = new List<BoundingBox>();
    private List<BoundingBox> fireupBoxes = new List<BoundingBox>();


    public Dictionary<string,Keys> dict = new Dictionary<string,Keys>();
    

    public bool isBallDie = false;
    public bool isInsert = true;
    public bool goToNextLevel = true;    
    public int score = 0;

    ContentManager content;
    
    private Random random = new Random(354668);

    public ContentManager Content
    {
      get{return content;}
    }
      
    public Level(IServiceProvider serviceProvider, string path)
      {
        // TODO: Construct any child components here
        InitDictionary();        
        content = new ContentManager(serviceProvider,"Content");
        //audioEngine = new AudioEngine("Content\\Sound\\sound.xgs");
        //waveBank = new WaveBank(audioEngine,"Content\\Sound\\Wave Bank.xwb");
        //if(waveBank != null)
        //  {
        //    //soundBank = new //soundBank(audioEngine,"Content\\Sound\\Sound Bank.xsb");
        //  }
        LoadBlocks(path);
      }

    private void LoadBlocks(string path)
    {
      int width;
      List<string> lines = new List<string>();

      using (StreamReader reader = new StreamReader(path))
        {
          string line = reader.ReadLine();
          width = line.Length;
          while(line != null)
            {
              lines.Add(line);
              if(line.Length != width)
                throw new Exception(String.Format("The length of line is different.",lines.Count));
              line = reader.ReadLine();
            }
        }


      blocks = new Block[width,lines.Count];

      for(int y = 0;y<blocks.GetLength(1);++y)
        {
          for(int x = 0;x<blocks.GetLength(0);++x)
            {
              char BlockType = lines[y][x];
              blocks[x,y] = LoadBlock(BlockType,x,y);
            }
        }
    }

    private Block LoadBlock(char blockType,int x,int y)
    {
      switch(blockType)
        {
        case '.':
          return new Block(null,BlockCollision.Passable);
        case 'x':
          return LoadMapBlock("Sprite\\block",BlockCollision.Impassable);
        case 'p':
          return LoadLeftPad(x,y);
        case 'm':
          return LoadTopPad(x,y);
        case 'b':
          return LoadBall(x,y);
        case 't':
          return LoadTarget(x,y);
        case 'f':
          return LoadFireDown(x,y);
        case 'd':
          return LoadDollar(x,y);
        case 'e':
          return LoadFireUp(x,y);
          
        default:
          throw new NotSupportedException(String.Format("Unsupported character '{0}' at position {1},{2}.",blockType,x,y));
        }
    }

    private Block LoadDollar(int x,int y)
    {
      dollars.Add(new Dollar(this,new Vector2(x*Block.width,y*Block.height)));
      return new Block(null,BlockCollision.Passable);
    }

    private Block LoadTarget(int x,int y)
    {
      targets.Add(new Target(this,new Vector2(x*Block.width,y*Block.height)));
      return new Block(null,BlockCollision.Passable);
    }

    private Block LoadFireDown(int x,int y)
    {
      animations.Add(new AnimationPlayer(this,@"Animation\fire_up",new Vector2(x*Block.width,y*Block.height)));
      firedownBoxes.Add(new BoundingBox(new Vector3((x - 1)*Block.width,(y - 2)*Block.height,0),new Vector3(x*Block.width,(y + 1)*Block.height,0)));
      return new Block(null,BlockCollision.Passable);
    }

    private Block LoadFireUp(int x,int y)
    {
      animations.Add(new AnimationPlayer(this,@"Animation\fire_down",new Vector2(x*Block.width,y*Block.height)));
      fireupBoxes.Add(new BoundingBox(new Vector3((x - 1)*Block.width,(y - 2)*Block.height,0),new Vector3(x*Block.width,(y + 1)*Block.height,0)));
      return new Block(null,BlockCollision.Passable);
    }

    private Block LoadBall(int x,int y)
    {
      pongPosition = new Vector2(x*Block.width,y*Block.height);
      ball = new Ball(this,new Vector2(x*Block.width,y*Block.height));
      return new Block(null,BlockCollision.Passable);
    }
    
    protected void InitDictionary()
    {
      ConfigFile cf = new ConfigFile("main.ini");
      dict = cf.LoadFromFile();
    }
    

    public void LevelMoveTopPad()
    {
      KeyboardState keyboard = Keyboard.GetState();
      int min = 4;
      //      if (keyboard.IsKeyDown(Keys.Left))
      if (keyboard.IsKeyDown(dict["left"]))      
        {
          foreach(TopPad toppad in toppads)
            {
              int i = (int)(toppad.position.X/Block.width) - 1;
              int j = (int) (toppad.position.Y/Block.height);
              BoundingBox topPadBox = new BoundingBox(new Vector3(toppad.position.X - 10,toppad.position.Y,0),new Vector3(toppad.position.X,toppad.position.Y + 10,0));
              BoundingBox blockBox = new BoundingBox(new Vector3(i*Block.width,j*Block.height,0),new Vector3((i + 1)*Block.width,(j + 1)*Block.height,0));
              
              if(!(blockBox.Intersects(topPadBox)&&blocks[i,j].Collision == BlockCollision.Impassable))
                {
                  toppad.position.X -= min;  
                }
            }
        }
      //      if (keyboard.IsKeyDown(Keys.Right))
      if (keyboard.IsKeyDown(dict["right"]))        
        {

          foreach(TopPad toppad in toppads)
            {
              int i = (int)((toppad.position.X + 50)/Block.width) + 1;
              int j = (int) (toppad.position.Y/Block.height);
              BoundingBox topPadBox = new BoundingBox(new Vector3(toppad.position.X + 50,toppad.position.Y,0),new Vector3(toppad.position.X + 60,toppad.position.Y + 10,0));
              BoundingBox blockBox = new BoundingBox(new Vector3(i*Block.width,j*Block.height,0),new Vector3((i + 1)*Block.width,(j + 1)*Block.height,0));
     
              if(!(blockBox.Intersects(topPadBox)&&blocks[i,j].Collision == BlockCollision.Impassable))
                {
                  toppad.position.X += min;
                }
            }
        }
    }

    private Block LoadLeftPad(int x,int y)
    {
      leftpads.Add(new LeftPad(this,new Vector2(x*Block.width,y*Block.height)));
      return new Block(null,BlockCollision.Passable);
    }


    public bool dealWithCollisionWithTarget()
    {
      Vector2 pongPosition = new Vector2(ball.position.X,ball.position.Y);
      BoundingBox pongBox = new BoundingBox(new Vector3(pongPosition.X,pongPosition.Y,0),new Vector3(pongPosition.X + 10,pongPosition.Y + 10,0));
      foreach(Target t in targets)
        {
          BoundingBox tBox = new BoundingBox(new Vector3(t.position.X,t.position.Y,0),new Vector3(t.position.X + 50,t.position.Y + 50,0));
          if(tBox.Intersects(pongBox))
            {
              return true;              
            }
        }
      return false;
    }

    
    public Vector2 dealwithCollisionWithLeftPad(Vector2 pongPosition,Vector2 pongSpeed)
    {
      float x = pongPosition.X;
      float y = pongPosition.Y;

      BoundingBox r1 = new BoundingBox(new Vector3(x,y,0),new Vector3(x + 5,y + 5,0));
      BoundingBox r2 = new BoundingBox(new Vector3(x + 5,y,0),new Vector3(x + 10,y,0));
      BoundingBox r3 = new BoundingBox(new Vector3(x,y + 5,0),new Vector3(x + 5,y + 10,0));
      BoundingBox r4 = new BoundingBox(new Vector3(x + 5,y + 5,0),new Vector3(x + 10,y + 10,0));
      
      foreach(LeftPad leftpad in leftpads)
        {
          int direction = 0;          
          BoundingBox tempLeftpad = new BoundingBox(new Vector3(leftpad.position.X,leftpad.position.Y,0),new Vector3(leftpad.position.X + leftpad.width, leftpad.position.Y + leftpad.height,0));
          if(rectangleCollision(tempLeftpad,r1))
            direction |= 1;
          if(rectangleCollision(tempLeftpad,r2))
            direction |= 2;
          if(rectangleCollision(tempLeftpad,r3))
            direction |= 4;
          if(rectangleCollision(tempLeftpad,r4))
            direction |= 8;

          switch(direction)
            {
            case 12:
              if(pongSpeed.Y>0)
                {
                  pongSpeed.Y = -pongSpeed.Y;
                  ////soundBank.PlayCue("zap");              
                }
              return pongSpeed;
            case 3:
              if(pongSpeed.Y<0)
                {
                  pongSpeed.Y = -pongSpeed.Y;
                  ////soundBank.PlayCue("zap");              
                }
              return pongSpeed;
            case 10:
              if(pongSpeed.X>0)
                {
                  pongSpeed.X = -pongSpeed.X;
                  ////soundBank.PlayCue("zap");                              
                }
              return pongSpeed;
            case 5:
              if(pongSpeed.X<0)
                {
                  pongSpeed.X =  -pongSpeed.X;                  
                  ////soundBank.PlayCue("zap");
                }
              return pongSpeed;
              //            case 11:
              /*       case 13:
                       if(pongSpeed.X<0&&pongSpeed.Y>0)
                       {
                       pongSpeed.X =  -pongSpeed.X;
                       pongSpeed.Y =  -pongSpeed.Y;
                       }
                       return pongSpeed;
              */

            case 1:
              if(pongSpeed.X<0&&pongSpeed.Y<0)
                {
                  float temp = pongSpeed.X;
                  pongSpeed.X = -pongSpeed.Y;
                  pongSpeed.Y = -temp;
                  ////soundBank.PlayCue("zap");                  
                }
              else if(pongSpeed.X<0&&pongSpeed.Y>0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y<0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              ////soundBank.PlayCue("zap");              
              return pongSpeed;
            case 8:
              if(pongSpeed.X>0&&pongSpeed.Y>0)
                {
                  float temp = pongSpeed.X;
                  pongSpeed.X = -pongSpeed.Y;
                  pongSpeed.Y = -temp;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y>0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y<0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              ////soundBank.PlayCue("zap");              
              return pongSpeed;


            case 2:
              if(pongSpeed.X>0&&pongSpeed.Y<0)
                {
                  float temp1 = pongSpeed.X;
                  pongSpeed.X = pongSpeed.Y;
                  pongSpeed.Y = temp1;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y>0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y<0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              ////soundBank.PlayCue("zap");              
              return pongSpeed;
              
            case 4:
              if(pongSpeed.X<0&&pongSpeed.Y>0)
                {
                  float temp1 = pongSpeed.X;
                  pongSpeed.X = pongSpeed.Y;
                  pongSpeed.Y = temp1;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y>0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y<0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              ////soundBank.PlayCue("zap");              
              return pongSpeed;
            }

        }
      return pongSpeed;
    }

    public Vector2 dealwithCollisionWithTopPad(Vector2 pongPosition,Vector2 pongSpeed)
    {

      float x = pongPosition.X;
      float y = pongPosition.Y;

      BoundingBox r1 = new BoundingBox(new Vector3(x,y,0),new Vector3(x + 5,y + 5,0));
      BoundingBox r2 = new BoundingBox(new Vector3(x + 5,y,0),new Vector3(x + 10,y,0));
      BoundingBox r3 = new BoundingBox(new Vector3(x,y + 5,0),new Vector3(x + 5,y + 10,0));
      BoundingBox r4 = new BoundingBox(new Vector3(x + 5,y + 5,0),new Vector3(x + 10,y + 10,0));
      
      foreach(TopPad toppad in toppads)
        {
          int direction = 0;          
          BoundingBox tempTopPad = new BoundingBox(new Vector3(toppad.position.X,toppad.position.Y,0),new Vector3(toppad.position.X + toppad.width, toppad.position.Y + toppad.height,0));
          if(rectangleCollision(tempTopPad,r1))
            direction |= 1;
          if(rectangleCollision(tempTopPad,r2))
            direction |= 2;
          if(rectangleCollision(tempTopPad,r3))
            direction |= 4;
          if(rectangleCollision(tempTopPad,r4))
            direction |= 8;

          switch(direction)
            {
            case 12:
              if(pongSpeed.Y>0)
                pongSpeed.Y = -pongSpeed.Y;
              ////soundBank.PlayCue("zap");                            
              return pongSpeed;
            case 3:
              if(pongSpeed.Y<0)
                pongSpeed.Y = -pongSpeed.Y;
              ////soundBank.PlayCue("zap");                            
              return pongSpeed;
            case 10:
              if(pongSpeed.X>0)
                pongSpeed.X = -pongSpeed.X;
              ////soundBank.PlayCue("zap");                            
              return pongSpeed;
            case 5:
              if(pongSpeed.X<0)
                pongSpeed.X =  -pongSpeed.X;
              ////soundBank.PlayCue("zap");                            
              return pongSpeed;
              //            case 11:
              //            case 13:
              /*              if(pongSpeed.X<0&&pongSpeed.Y>0)
                              {
                              pongSpeed.X =  -pongSpeed.X;
                              pongSpeed.Y =  -pongSpeed.Y;
                              }
                              return pongSpeed;
              */
            case 15:
              if(Math.Abs(pongPosition.X - toppad.position.X)<Math.Abs(pongPosition.X - toppad.position.X - 50))
                {
                  if(pongSpeed.X>0)
                    {
                      pongSpeed.X =  - pongSpeed.X;
                    }
                }
              else
                {
                  if(pongSpeed.X<0)
                    {
                      pongSpeed.X =  -pongSpeed.X;
                    }
                }
              //soundBank.PlayCue("zap");                            
              return pongSpeed;
              
            case 1:
              if(pongSpeed.X<0&&pongSpeed.Y<0)
                {
                  float temp = pongSpeed.X;
                  pongSpeed.X = -pongSpeed.Y;
                  pongSpeed.Y = -temp;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y>0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y<0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              //soundBank.PlayCue("zap");                            
              return pongSpeed;
            case 8:
              if(pongSpeed.X>0&&pongSpeed.Y>0)
                {
                  float temp = pongSpeed.X;
                  pongSpeed.X = -pongSpeed.Y;
                  pongSpeed.Y = -temp;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y>0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y<0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              //soundBank.PlayCue("zap");                            
              return pongSpeed;


            case 2:
              if(pongSpeed.X>0&&pongSpeed.Y<0)
                {
                  float temp1 = pongSpeed.X;
                  pongSpeed.X = pongSpeed.Y;
                  pongSpeed.Y = temp1;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y>0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y<0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              //soundBank.PlayCue("zap");              
              return pongSpeed;
              
            case 4:
              if(pongSpeed.X<0&&pongSpeed.Y>0)
                {
                  float temp1 = pongSpeed.X;
                  pongSpeed.X = pongSpeed.Y;
                  pongSpeed.Y = temp1;
                }
              else if(pongSpeed.X>0&&pongSpeed.Y>0)
                {
                  pongSpeed.Y =  - pongSpeed.Y;
                }
              else if(pongSpeed.X<0&&pongSpeed.Y<0)
                {
                  pongSpeed.X =  - pongSpeed.X;
                }
              //soundBank.PlayCue("zap");                            
              return pongSpeed;
            }

        }

      return pongSpeed;
    }

    private Block LoadTopPad(int x,int y)
    {
      toppads.Add(new TopPad(this,new Vector2(x*Block.width,y*Block.height)));
      return new Block(null,BlockCollision.Passable);
    }

    private Block LoadMapBlock(string spriteName,BlockCollision collision)
    {
      return LoadBlock(spriteName,collision);
    }
    private Block LoadBlock(string spriteName,BlockCollision collision)
    {
      return new Block(Content.Load<Texture2D>(spriteName),collision);
    }
    /// <summary>
    /// Allows the game component to perform any initialization it needs to before starting
    /// to run.  This is where it can query for any required services and load content.
    /// </summary>
    public  void Initialize()
    {
      // TODO: Add your initialization code here

      //            base.Initialize();
    }

    /// <summary>
    /// Allows the game component to update itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update(GameTime gameTime)
    {
      // TODO: Add your update code here

      //            base.Update(gameTime);
      handleCollision();
      handleCollisionWithTopPad();
      handleCollisionWithLeftPad();
      handleCollisionWithFiredown(ball.position);
      handleCollisionWithFireup(ball.position);      
      //      pongPosition += pongSpeed; 
      // TODO: Add your update logic here
      LevelMoveTopPad();
      LevelMoveLeftPad();
      handleCollisionWithDollar(ball.position);

      ball.position += pongSpeed*((float)60*gameTime.ElapsedGameTime.Milliseconds)/(float)1000;
    }

    protected void handleCollisionWithFireup(Vector2 pongPosition)
    {
      BoundingBox ballBox = new BoundingBox(new Vector3(pongPosition.X,pongPosition.Y,0),new Vector3(pongPosition.X + 10,pongPosition.Y + 10,0));
      foreach(BoundingBox b in fireupBoxes)
        {
          if(b.Intersects(ballBox))
            {
              isBallDie = true;
              //soundBank.PlayCue("explode");
              return;
            }
        }
    }

    protected void handleCollisionWithFiredown(Vector2 pongPosition)
    {
      BoundingBox ballBox = new BoundingBox(new Vector3(pongPosition.X,pongPosition.Y,0),new Vector3(pongPosition.X + 10,pongPosition.Y + 10,0));
      foreach(BoundingBox b in firedownBoxes)
        {
          if(b.Intersects(ballBox))
            {
              isBallDie = true;
              //soundBank.PlayCue("explode");
              return;
            }
        }
    }


    protected void handleCollisionWithDollar(Vector2 pongPosition)
    {
      BoundingBox ball = new BoundingBox(new Vector3(pongPosition.X,pongPosition.Y,0),new Vector3(pongPosition.X + 10,pongPosition.Y + 10,0));
      int i =  -1;
      foreach(Dollar dollar in dollars)
        {
          i++;
          BoundingBox dollarBox = new BoundingBox(new Vector3(dollar.position.X,dollar.position.Y,0),new Vector3(dollar.position.X + 20,dollar.position.Y + 20,0));
          if(ball.Intersects(dollarBox))
            {
              //soundBank.PlayCue("plop");
              dollar.position = new Vector2(rnd.Next(20,500),rnd.Next(20,500));
              score++;
            }
        }
    }

    protected void handleCollisionWithLeftPad()
    {
      pongSpeed = dealwithCollisionWithLeftPad(ball.position,pongSpeed);
    }
    
    /*    protected void MoveLeftPad()
          {
          level.LevelMoveLeftPad();      
          }*/

    protected void handleCollisionWithTopPad()
    {
      pongSpeed = dealwithCollisionWithTopPad(ball.position,pongSpeed);
    }

    protected void handleCollision()
    {
      int BlockX = (int)Math.Floor((float)(ball.position.X + 5)/Block.width) - 1;
      int BlockY = (int)Math.Floor((float)(ball.position.Y + 5)/(float)10) - 1;
      pongSpeed = dealWithCollison(ball.position,pongSpeed,BlockX,BlockY);
    }


    public void Dispose()
    {
      Content.Unload();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      DrawTiles(spriteBatch);


      foreach(TopPad toppad in toppads)
        {
          toppad.Draw(spriteBatch);
        }

      foreach(LeftPad leftpad in leftpads)
        {
          leftpad.Draw(spriteBatch);
        }

      foreach(AnimationPlayer a in animations)
        {
          a.Draw(gameTime,spriteBatch);
        }

      foreach(Target t in targets)
        {
          t.Draw(spriteBatch);
        }

      foreach(Dollar d in dollars)
        {
          d.Draw(spriteBatch);
        }

      if(ball != null)
        ball.Draw(spriteBatch);
    }

    private void DrawTiles(SpriteBatch spriteBatch)
    {
      for(int y = 0;y<blocks.GetLength(1);++y)
        {
          for(int x = 0;x<blocks.GetLength(0);++x)
            {
              Texture2D texture1 = blocks[x,y].texture;
              if(texture1 != null)
                {
                  Vector2 position = new Vector2(x,y)*Block.size;
                  spriteBatch.Draw(texture1,position,Color.White);
                }
            }
        }
    }

    public void LevelMoveLeftPad()
    {
      KeyboardState keyboard = Keyboard.GetState();
      int min = 4;
      //      if (keyboard.IsKeyDown(Keys.Up))
      if (keyboard.IsKeyDown(dict["up"]))        
        {
          foreach(LeftPad leftpad in leftpads)
            {
              int i = (int)(leftpad.position.X/Block.width);
              int j = (int) (leftpad.position.Y/Block.height) - 1;
              BoundingBox padBox = new BoundingBox(new Vector3(leftpad.position.X,leftpad.position.Y - 10,0),new Vector3(leftpad.position.X + 10,leftpad.position.Y,0));
              BoundingBox blockBox = new BoundingBox(new Vector3(i*Block.width,j*Block.height,0),new Vector3((i + 1)*Block.width,(j + 1)*Block.height,0));
              
              if(i >= 0&&j >= 0&&!(rectangleCollision(padBox,blockBox)&&blocks[i,j].Collision == BlockCollision.Impassable))
                {
                  leftpad.position.Y -= min;  
                }
            }
        }
      //      if (keyboard.IsKeyDown(Keys.Down))
      if (keyboard.IsKeyDown(dict["down"]))        
        {

          foreach(LeftPad leftpad in leftpads)
            {
              int i = (int)((leftpad.position.X)/Block.width);
              int j = (int)((leftpad.position.Y + leftpad.height)/Block.height) + 1;
              BoundingBox blockBox = new BoundingBox(new Vector3(i*Block.width,j*Block.height,0),new Vector3((i + 1)*Block.width,(j + 1)*Block.height,0));
              BoundingBox padBox = new BoundingBox(new Vector3(leftpad.position.X,leftpad.position.Y + 50,0),new Vector3(leftpad.position.X + 10,leftpad.position.Y + 60,0));
              
              if(i >= 0&&j >= 0&&!(rectangleCollision(padBox,blockBox)&&blocks[i,j].Collision == BlockCollision.Impassable))
                {
                  leftpad.position.Y += min;
                }
            }
        }
    }
    protected bool padCollision(Vector2 padPosition,Vector2 blockPosition,int x,int y)
    {
      Rectangle object1 = new Rectangle((int)padPosition.X,(int)padPosition.Y,x,y);
      Rectangle object2 = new Rectangle((int)blockPosition.X,(int)blockPosition.Y,10,10);
      return object1.Intersects(object2);
    }

    protected bool collid(Vector2 pongPosition,Vector2 blockPosition)
    {
      Rectangle object1 = new Rectangle((int)pongPosition.X,(int)pongPosition.Y,10,10);
      Rectangle object2 = new Rectangle((int)blockPosition.X,(int)blockPosition.Y,Block.width,10);
      return object1.Intersects(object2);
    }

    protected bool rectangleCollision(BoundingBox r1,BoundingBox r2)
    {
      return r1.Intersects(r2);
    }
        
    public Vector2 dealWithCollison(Vector2 pongPosition,Vector2 speed,int x,int y)
    {
      int i = Block.width;
      int j = Block.height;
      float min = 0.1f;
      BoundingBox ball = new BoundingBox(new Vector3(pongPosition.X,pongPosition.Y,0),new Vector3(pongPosition.X + 10,pongPosition.Y + 10,0));

      BoundingBox block_left = new BoundingBox(new Vector3(x*i,(y + 1)*j,0),new Vector3((x + 1)*i,(y + 2)*j,0));

      bool switch_c = false;

      if((rectangleCollision(ball,block_left)&&blocks[x,y + 1].Collision == BlockCollision.Impassable))
        {
          if(speed.X<0)
            {
              speed.X =  -speed.X;
              switch_c = true;
            }
        }

      BoundingBox block_right = new BoundingBox(new Vector3((x + 2)*i,(y + 1)*j,0),new Vector3((x + 3)*i,(y + 2)*j + 10,0));
      
      if(rectangleCollision(ball,block_right)&&blocks[x + 2,y + 1].Collision == BlockCollision.Impassable)
        {
          if(speed.X>0)
            {
              speed.X =  -speed.X;
              switch_c = true;
            }
        }
      BoundingBox block_up = new BoundingBox(new Vector3((x + 1)*i,y*j,0),new Vector3((x + 2)*i,(y + 1)*j,0));

      if((rectangleCollision(ball,block_up)&&blocks[x + 1,y].Collision == BlockCollision.Impassable))
        {
          if(speed.Y<0)
            {
              speed.Y =  -speed.Y;
              switch_c = true;
            }
        }

      BoundingBox block_down = new BoundingBox(new Vector3((x + 1)*i,(y + 2)*j,0),new Vector3((x + 2)*i,(y + 3)*j,0));
      if(rectangleCollision(ball,block_down)&&blocks[x + 1,y + 2].Collision == BlockCollision.Impassable)
        {
          if(speed.Y>0)
            {
              speed.Y =  -speed.Y;
              switch_c = true;
            }
        }

      if(switch_c)
        {
          //soundBank.PlayCue("zap");                        
          return speed;
        }

      BoundingBox block_up_left = new BoundingBox(new Vector3(x*i,y*j,0),new Vector3((x + 1)*i,(y + 1)*j,0));
      if(rectangleCollision(ball,block_up_left)&&blocks[x,y].Collision == BlockCollision.Impassable)//&&(pongPosition.X - (x + 1)*i)*(pongPosition.X - (x + 1)*i) + (pongPosition.Y - (y + 1)*j)*(pongPosition.Y - (y + 1)*j) - 5*5<min)
        {
          if(speed.X<0&&speed.Y<0)
            {
              float temp = speed.X;
              speed.X =  -speed.Y;
              speed.Y =  -temp;
              //soundBank.PlayCue("zap");                            
              return speed;
            }
          else if(speed.Y<0&&speed.X>0)
            {
              speed.Y =  - speed.Y;
              //soundBank.PlayCue("zap");                            
              return speed;
            }
          else if(speed.Y>0&&speed.X<0)
            {
              speed.X =  - speed.X;
              //soundBank.PlayCue("zap");                            
              return speed;
            }
          return speed;
        }

      BoundingBox block_down_right = new BoundingBox(new Vector3((x + 2)*i,(y + 2)*j,0),new Vector3((x + 3)*i,(y + 3)*j + 10,0));
      
      if(rectangleCollision(ball,block_down_right)&&blocks[x + 2,y + 2].Collision == BlockCollision.Impassable)//&&(pongPosition.X - (x + 2)*i)*(pongPosition.X - (x + 2)*i) + (pongPosition.Y - (y + 2)*j)*(pongPosition.Y - (y + 2)*j) - 5*5<min)
        {
          if(speed.X>0&&speed.Y>0)
            {
              float temp = speed.X;
              speed.X =  -speed.Y;
              speed.Y = -temp;
              //soundBank.PlayCue("zap");                            
              return speed;
            }
          else if(speed.Y<0&&speed.X>0)
            {
              speed.X =  - speed.X;
              return speed;
            }
          else if(speed.Y>0&&speed.X<0)
            {
              speed.Y =  - speed.Y;
              //soundBank.PlayCue("zap");                            
              return speed;
            }
          return speed;
        }

      BoundingBox block_up_right = new BoundingBox(new Vector3((x + 2)*i,y*j,0),new Vector3((x + 3)*i,(y + 1)*j,0));
      
      if((rectangleCollision(ball,block_up_right)&&blocks[x + 2,y].Collision == BlockCollision.Impassable))//&&((pongPosition.X - (x + 2)*i)*(pongPosition.X - (x + 2)*i) + (pongPosition.Y - (y + 1)*j)*(pongPosition.Y - (y + 1)*j) - 5*5<min)))
        {
          if(speed.X>0&&speed.Y<0)
            {
              float temp = speed.X;
              speed.X =  speed.Y;
              speed.Y = temp;
              //soundBank.PlayCue("zap");                        
              return speed;
            }
          else if(speed.Y>0&&speed.X>0)
            {
              speed.X =  - speed.X;
              //soundBank.PlayCue("zap");                        
              return speed;
            }
          else if(speed.Y<0&&speed.X<0)
            {
              speed.Y =  - speed.Y;
              //soundBank.PlayCue("zap");                        

              return speed;
            }
          return speed;
        }
         
      BoundingBox block_down_left = new BoundingBox(new Vector3(x*i,(y + 2)*j,0),new Vector3((x + 1)*i,(y + 3)*j,0));
      
      if(rectangleCollision(ball,block_down_left)&&blocks[x,y + 2].Collision == BlockCollision.Impassable)//&&((pongPosition.X - (x + 1)*i)*(pongPosition.X - (x + 1)*i) + (pongPosition.Y - (y + 2)*j)*(pongPosition.Y - (y + 2)*j) - 5*5<min))
        {
          //exchange the speed and
          if(speed.X<0&&speed.Y>0)
            {
              float temp = speed.X;
              speed.X = speed.Y;
              speed.Y = temp;
              //soundBank.PlayCue("zap");                        
              
              return speed;
            }
          else if(speed.Y>0&&speed.X>0)
            {
              speed.Y =  - speed.Y;
              //soundBank.PlayCue("zap");                        
              
              return speed;
            }
          else if(speed.Y<0&&speed.X<0)
            {
              speed.X =  - speed.X;
              //soundBank.PlayCue("zap");                        
              
              return speed;
            }
          return speed;
        }

      return speed;
    }
  }
}