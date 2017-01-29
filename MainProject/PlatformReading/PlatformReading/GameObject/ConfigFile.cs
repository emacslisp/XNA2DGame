using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;


namespace PlatformerReading
{
  /// <summary>
  /// This is a game component that implements IUpdateable.
  /// </summary>
  public class ConfigFile
  {
    private string iniFilePath;
    public Dictionary<string,Keys> dict = new Dictionary<string,Keys>();
    protected void InitDictionary()
    {
      dict.Add("a",Keys.A);
      dict.Add("b",Keys.B);
      dict.Add("c",Keys.C);
      dict.Add("d",Keys.D);
      dict.Add("e",Keys.E);
      dict.Add("f",Keys.F);
      dict.Add("g",Keys.G);
      dict.Add("h",Keys.H);
      dict.Add("i",Keys.I);
      dict.Add("j",Keys.J);
      dict.Add("k",Keys.K);
      dict.Add("l",Keys.L);
      dict.Add("m",Keys.M);
      dict.Add("n",Keys.N);
      dict.Add("o",Keys.O);
      dict.Add("p",Keys.P);
      dict.Add("q",Keys.Q);
      dict.Add("r",Keys.R);
      dict.Add("s",Keys.S);
      dict.Add("t",Keys.T);
      dict.Add("u",Keys.U);
      dict.Add("v",Keys.V);
      dict.Add("w",Keys.W);
      dict.Add("x",Keys.X);
      dict.Add("y",Keys.Y);
      dict.Add("z",Keys.Z);
      dict.Add("up",Keys.Up);
      dict.Add("down",Keys.Down);
      dict.Add("left",Keys.Left);
      dict.Add("right",Keys.Right);
    }
    
    public ConfigFile(string filePath)
    {
      string temp = String.Format(filePath);
      //@todo: path to content
      temp = Path.Combine(""/*StorageContainer.TitleLocation*/,"Content/" + temp);
      iniFilePath = temp;
      InitDictionary();
    }

    public Dictionary<string,Keys> LoadFromFile()
    {
      Dictionary<string,Keys> strKeys = new Dictionary<string,Keys>();

      int lineNumber = 0;
      using(StreamReader reader = new StreamReader(iniFilePath))
        {
          while(!reader.EndOfStream)
            {
              string line = reader.ReadLine();
              lineNumber++;

              if(line.Contains("#"))
                {
                  if(line.IndexOf("#") == 0)
                    continue;
                  line = line.Substring(0,line.IndexOf("#"));
                }

              line = line.Trim();

              Match match = Regex.Match(line,"\\[[a-zA-Z\\d\\s]+\\]");

              if(match.Success)
                {
                  if (match.Value.Length == 2)
                    throw new Exception(string.Format("Group must have name (line {0})", lineNumber));
                }
              else if(line.Contains("="))
                {
                  string[] parts = line.Split('=');

                  if(parts.Length != 2)
                    throw new Exception(string.Format("Settings must be in the format 'name = value' (line {0})", lineNumber));
                  parts[0] = parts[0].Trim();
                  parts[1] = parts[1].Trim();
                  if(dict.ContainsKey(parts[1]))
                    strKeys.Add(parts[0],dict[parts[1]]);
                }
            }
        }
      return strKeys;
    }
  }
}