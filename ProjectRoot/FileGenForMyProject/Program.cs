using MainProject.Models;
using T4FileGenerator.Generators;

var path = "D:\\Garik\\MyProjects\\SomeSharp\\ProjectRoot\\MainProject\\bin\\Debug\\net8.0\\MainProject.xml";

var fg = new FileGenerator(path);

fg.CreateTableFromClass(typeof(BaseModel));
