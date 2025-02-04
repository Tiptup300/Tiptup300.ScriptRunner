﻿using System.Collections.Immutable;
using Tiptup300.Rnr.Integrations;

namespace Tiptup300.Rnr.Configuration;

public class ScriptLocater : IScriptLocater
{
   private IFileSystemIntegration _io;

   public ScriptLocater(IFileSystemIntegration fileSystemIntegration)
   {
      _io = fileSystemIntegration;
   }

   public ImmutableArray<ScriptFile> GetScripts(ScriptDirectory scriptLocationPath)
   {
      var files = _io.GetFilesFromPath(scriptLocationPath.FullPath);
      return files.Select(f => ScriptFile.Build(f, _io)).ToImmutableArray();
   }
}
