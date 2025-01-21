﻿namespace Tiptup300.ScriptRunner;

public record Arg
{
   public string Name { get; }
   public string Value { get; }

   public Arg(string name, string value)
   {
      if (string.IsNullOrEmpty(name))
      {
         throw new ArgumentException("Argument name cannot be null or empty", nameof(name));
      }
      if (string.IsNullOrEmpty(value))
      {
         throw new ArgumentException("Argument value cannot be null or empty", nameof(value));
      }
      Name = name;
      Value = value;
   }
}
