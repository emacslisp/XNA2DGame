using System;
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
    /// This is a game component that implements IUpdateable.
    /// </summary>
  enum BlockCollision
    {
      
      Passable = 0,
      Impassable = 1,
      Platformer = 2,
    }

  struct Block
  {
    public Texture2D texture;
    public BlockCollision Collision;
    public const int width = 10;
    public const int height = 10;

    public static readonly Vector2 size = new Vector2(width,height);

    public Block(Texture2D t,BlockCollision collision)
      {
        texture = t;
        Collision = collision;
      }
  }
}