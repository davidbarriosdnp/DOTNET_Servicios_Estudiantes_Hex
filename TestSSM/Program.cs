using System;
using System.IO;
using System.Text.RegularExpressions;

string basePath = @"D:\PRUEBA\DOTNET_Servicios_Estudiantes_Hex\Servicios_Estudiantes.Aplicacion\CasosUso\Catalogos";
string[] files = { "ProgramasCreditoCasosUso.cs", "ProfesoresCasosUso.cs", "MateriasCasosUso.cs" };

foreach (var file in files) {
    string filePath = Path.Combine(basePath, file);
    if (!File.Exists(filePath)) continue;
    
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

namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos." + folderName + (folderName == "ProgramasCredito" ? ".Comandos" : ".Comandos") + @"
{"; // We'll fix the namespace per file later

    
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
                bool isCommand = currentCommandName.EndsWith("Command");
                string targetDir = isCommand ? dirComandos : dirConsultas;
                string ns = "Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos." + folderName + "." + (isCommand ? "Comandos" : "Consultas");
                string finalUsings = usings.Replace("Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.ProgramasCredito.Comandos", ns)
                                           .Replace("Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Profesores.Comandos", ns)
                                           .Replace("Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos.Materias.Comandos", ns);
                
                File.WriteAllText(Path.Combine(targetDir, currentCommandName + ".cs"), finalUsings + "\n    " + currentCommandContent.TrimEnd() + "\n}\n");
            }
            currentCommandName = className;
            currentCommandContent = block + "\n";
        } else {
            // It's a handler or validator, append to current command
            currentCommandContent += "    " + block + "\n";
        }
    }
    
    // Save last command
    if (!string.IsNullOrEmpty(currentCommandName)) {
        bool isCommand = currentCommandName.EndsWith("Command");
        string targetDir = isCommand ? dirComandos : dirConsultas;
        string ns = "Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos." + folderName + "." + (isCommand ? "Comandos" : "Consultas");
        
        string baseUsing = @"using FluentValidation;
using MediatR;
using Servicios_Estudiantes.Aplicacion.DTOs;
using Servicios_Estudiantes.Aplicacion.Envoltorios;
using Servicios_Estudiantes.Aplicacion.Puertos;

namespace " + ns + @"
{";
        File.WriteAllText(Path.Combine(targetDir, currentCommandName + ".cs"), baseUsing + "\n    " + currentCommandContent.TrimEnd() + "\n}\n");
    }
    
    // Rewrite all the previous ones to ensure correct namespace
    foreach(var f in Directory.GetFiles(dirComandos)) {
        string t = File.ReadAllText(f);
        t = Regex.Replace(t, @"namespace Servicios_Estudiantes\.Aplicacion\.CasosUso\.Catalogos\..*", "namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos." + folderName + ".Comandos");
        File.WriteAllText(f, t);
    }
    foreach(var f in Directory.GetFiles(dirConsultas)) {
        string t = File.ReadAllText(f);
        t = Regex.Replace(t, @"namespace Servicios_Estudiantes\.Aplicacion\.CasosUso\.Catalogos\..*", "namespace Servicios_Estudiantes.Aplicacion.CasosUso.Catalogos." + folderName + ".Consultas");
        File.WriteAllText(f, t);
    }
    
    // Remove original file
    File.Delete(filePath);
}
