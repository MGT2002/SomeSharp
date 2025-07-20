using MainProject.Models;
using MGTFileGenerator.Generators;

var xmlDocPath = "D:\\Garik\\MyProjects\\SomeSharp\\ProjectRoot\\MainProject\\bin\\Debug\\net8.0\\MainProject.xml";
var outputDirPath = "D:\\Garik\\MyProjects\\SomeSharp\\ProjectRoot\\FileGenForMyProject\\Output\\";
//outputDirPath = null;

var fg = new FileGenerator(xmlDocPath, outputDirectory: outputDirPath);

fg.CreateTableFromClass(typeof(BaseModel));
