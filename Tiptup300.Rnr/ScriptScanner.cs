﻿using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Tiptup300.Rnr;

public class ScriptScanner : IScriptScanner
{
   private readonly IScriptMetadataScanner _scriptMetadataScanner;
   private readonly ILogger<ScriptScanner> _logger;

   public ScriptScanner(IScriptMetadataScanner scriptMetadataScanner, ILogger<ScriptScanner> logger)
   {
      _scriptMetadataScanner = scriptMetadataScanner;
      _logger = logger;
   }

   private const string RNR_SCRIPT_EXTENSION = ".rnr.ps1";

   public ImmutableArray<ScriptModel> GetScripts(ImmutableArray<string> scriptLocations)
   {
      var scripts = new List<ScriptModel>();
      // Get all script files from the script locations
      // that end in .rnr.ps1
      foreach (var location in scriptLocations)
      {
         if (!Directory.Exists(location))
         {
            _logger.LogError("Directory not found: {location}", location);
            continue;
         }

         var files = Directory.GetFiles(location, "*.rnr.ps1")
            .Select(GetScriptFromFile)
            .OfType<ScriptModel>() // filter out nulls
            .Select(script => script with { ScriptLocation = location });

         scripts.AddRange(files);
      }

      return scripts.ToImmutableArray();
   }

   private ScriptModel? GetScriptFromFile(string filePath)
   {
      if (!File.Exists(filePath))
      {
         _logger.LogError("File not found: {filePath}", filePath);
         return null;
      }
      if (!filePath.EndsWith(RNR_SCRIPT_EXTENSION))
      {
         _logger.LogError("File must be a .rnr.ps1 file: {filePath}", filePath);
         return null;
      }
      var fileTag = BuildTagFromFile(filePath);
      if (fileTag == null)
      {
         return null;
      }
      var metaData = _scriptMetadataScanner.ScanScriptFileForMetadata(filePath);
      if (metaData is null)
         return null;

      var tag = metaData.TagOverride ?? fileTag;
      var usage = metaData.Usage;

      return new ScriptModel
      {
         Title = metaData.Title ?? tag,
         Path = filePath,
         Description = metaData.Description,
         Tag = tag,
         Usage = metaData.Usage
      };
   }

   private string? BuildTagFromFile(string filePath)
   {
      // remove .rnr.ps1, have to do more then without extension
      // because it would pickup as just a .ps1 file
      var scriptTag = filePath.Substring(0, filePath.Length - RNR_SCRIPT_EXTENSION.Length);

      // confirm file name only has alphanumeric, hyphen, periods, and underscore
      if (!Regex.IsMatch(scriptTag, @"^[a-zA-Z0-9\-\._]+$"))
      {
         _logger.LogError("File name must only contain alphanumeric characters, hyphens, periods, and underscores: {filePath}", filePath);
         return null;
      }

      return scriptTag;
   }

}
