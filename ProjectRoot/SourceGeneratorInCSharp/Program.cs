using SourceGeneratorInCSharp;
using System;
using CodeGenHello;

Console.WriteLine(Hello.Hi);
Console.WriteLine(Generated.Fib9.Value);
const int a = Generated.Fib2.Value;
Console.WriteLine(a);
