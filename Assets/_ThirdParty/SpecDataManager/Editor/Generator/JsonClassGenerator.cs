using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class JsonClassGenerator
{
    public static void GenerateClasses(string json, string namespaceName, string outputDirectory)
    {
        JObject jObject = JObject.Parse(json);
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();

        if (!namespaceName.Equals(""))
        {
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");
        }

        ParseObject(jObject, sb);

        if (!namespaceName.Equals(""))
            sb.AppendLine("}");

        File.WriteAllText(Path.Combine(outputDirectory, "GeneratedClasses.cs"), sb.ToString());

        GenerateDataManager(namespaceName, jObject, outputDirectory);
    }

    private static void ParseObject(JObject jObject, StringBuilder sb)
    {
        foreach (var property in jObject.Properties())
        {
            string propertyName = property.Name;
            JTokenType propertyType = property.Value.Type;

            string csharpType = GetCSharpType(propertyType, propertyName, property.Value, sb);
            if(!csharpType.Equals(""))
                sb.AppendLine($"    public {csharpType} {propertyName} {{ get; set; }}");
        }
    }

    private static void ParseObjectArray(JObject jObject, StringBuilder sb)
    {
        foreach (var property in jObject.Properties())
        {
            string propertyName = property.Name;
            JTokenType propertyType = property.Value.Type;

            string csharpType = GetCSharpTypeArray(propertyType, propertyName, property.Value, sb);
            if(!csharpType.Equals(""))
                sb.AppendLine($"    public {csharpType} {propertyName} {{ get; set; }}");
        }
    }

    private static void GenerateClass(JObject jObject, string className, StringBuilder sb)
    {
        sb.AppendLine($"public partial class {className}");
        sb.AppendLine("{");

        ParseObject(jObject, sb);

        sb.AppendLine("}");
        sb.AppendLine();
    }

    private static string GetCSharpType(JTokenType propertyType, string propertyName, JToken propertyValue, StringBuilder sb)
    {
        if (propertyName.Contains("E_"))
        {
            var enumName = propertyName.Split('_');
            return enumName[1];
        }

        switch (propertyType)
        {
            case JTokenType.Object:
                string className = propertyName + "Class";
                GenerateClass((JObject)propertyValue, className, sb);
                return className;
            case JTokenType.Array:
                JArray array = (JArray)propertyValue;
                if (array.Count > 0 && array[0].Type == JTokenType.Object)
                {
                    string elementClassName = propertyName;
                    GenerateClass((JObject)array[0], elementClassName, sb);
                    //return $"List<{elementClassName}>";
                }
                //return "List<object>"; // Fallback for arrays of primitive types
                return ""; // Fallback for arrays of primitive types
            case JTokenType.Integer:
                return "int";
            case JTokenType.Float:
                return "float";
            case JTokenType.String:
                return "string";
            case JTokenType.Boolean:
                return "bool";
            default:
                return "object"; // Fallback for any other type
        }
    }

    private static string GetCSharpTypeArray(JTokenType propertyType, string propertyName, JToken propertyValue, StringBuilder sb)
    {
        switch (propertyType)
        {
            case JTokenType.Array:
                JArray array = (JArray)propertyValue;
                if (array.Count > 0 && array[0].Type == JTokenType.Object)
                {
                    string elementClassName = propertyName;
                    return $"List<{elementClassName}>";
                }
                return "List<object>"; // Fallback for arrays of primitive types
        }

        return "";
    }

    private static void GenerateDataManager(string namespaceName, JObject jObject, string outputDirectory)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        if(!namespaceName.Equals(""))
            sb.AppendLine($"using {namespaceName};");
        sb.AppendLine();

        sb.AppendLine("public partial class SpecData");
        sb.AppendLine("{");
        ParseObjectArray(jObject, sb);
        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(outputDirectory, "SpecData.cs"), sb.ToString());
    }
}
