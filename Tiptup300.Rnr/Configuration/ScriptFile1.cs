﻿using Tiptup300.Rnr.Integrations;

namespace Tiptup300.Rnr.Configuration;

public class ScriptFile
{
   public string FullPath { get; private set; }

   private ScriptFile(string fullPath)
   {
      FullPath = fullPath;
   }

   public static ScriptFile Build(string filePath, IFileSystemIntegration io)
   {
      if (!io.FileExists(filePath))
      {
         throw new ArgumentException("File does not exist");
      }
      return new ScriptFile(filePath);
   }
}
