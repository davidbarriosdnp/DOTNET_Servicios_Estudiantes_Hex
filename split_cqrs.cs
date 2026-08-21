using System;
using System.IO;
using System.Text.RegularExpressions;

string basePath = @"D:\PRUEBA\DOTNET_Servicios_Estudiantes_Hex\Servicios_Estudiantes.Aplicacion\CasosUso\Catalogos";
string[] files = { "ProgramasCreditoCasosUso.cs", "ProfesoresCasosUso.cs", "MateriasCasosUso.cs" };

foreach (var file in files) {
    string filePath = Path.Combine(basePath, file);
    string content = File.ReadAllText(filePath);
    
    string folderName = file.Replace("CasosUso.cs", "");
    string dirComandos = Path.Combine(basePath, folderName, "Comandos");
    string dirConsultas = Path.Combine(basePath, folderName, "Consultas");
    
    Directory.CreateDirectory(dirComandos);
    Directory.CreateDirectory(dirConsultas);
    
    // Extract standard using statements
    string usings = @"using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos
{";
    
    // Split by public sealed record/class
    var matches = Regex.Matches(content, @"(?:public sealed (?:record|class) ([\w]+)[\s\S]*?(?=\s+public sealed (?:record|class)|$))");
    
    string currentCommandName = "";
    string currentCommandContent = "";
    
    foreach (Match m in matches) {
        string block = m.Value;
        string className = m.Groups[1].Value;
        
        if (className.EndsWith("Command") || className.EndsWith("Query")) {
            // Save previous command if exists
            if (!string.IsNullOrEmpty(currentCommandName)) {
                string targetDir = currentCommandName.EndsWith("Command") ? dirComandos : dirConsultas;
                File.WriteAllText(Path.Combine(targetDir, currentCommandName + ".cs"), usings + "\n    " + currentCommandContent.TrimEnd() + "\n}\n");
            }
            currentCommandName = className;
            currentCommandContent = block + "\n";
        } else {
            // It's a handler or validator, append to current command
            currentCommandContent += "\n    " + block + "\n";
        }
    }
    
    // Save last command
    if (!string.IsNullOrEmpty(currentCommandName)) {
        string targetDir = currentCommandName.EndsWith("Command") ? dirComandos : dirConsultas;
        File.WriteAllText(Path.Combine(targetDir, currentCommandName + ".cs"), usings + "\n    " + currentCommandContent.TrimEnd() + "\n}\n");
    }
    
    // Remove original file
    File.Delete(filePath);
}
