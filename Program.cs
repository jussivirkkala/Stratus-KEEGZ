/* Displaying stratuseeg.com patient information from .keegz or StaticInfo.xml 
 * @jussivirkkala
 * 2023-03-16 v1.0.7 .NET7 added stratuseeg.com
 * 2022-04-19 v1.0.4 .NET6 removed namespace, class, main
 * 2021-09-02 v1.0.3 Exclude file list, visit 
 * 2021-08-30 v1.0.2 Try, catch
 * 2021-08-29 v1.0.1 Group by Patient,Exam, Visit
 * 2021-08-28 v1.0.0 First version
 * 
 * dotnet publish keegz.csproj -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true
 */

using System;
using System.Xml;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Reflection;

// Main function

string r = "";
r += Line("Displaying stratus.eeg patient information from .keegz or StaticInfo.xml v" + 
    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion +
    "\ngithub.com/jussivirkkala/Stratus-keegz");

if (args.Length!=1)
{
    Console.WriteLine(@"Drag .keegz or StaticInfo.xml over exe or shortcut. Or use SendTo with %APPDATA%\Microsoft\Windows\SendTo");
    End();
    return;
}
// Filename as parameter
string f = args[0];
r += Line(f);
if (!f.EndsWith("StaticInfo.xml") & !f.EndsWith(".keegz"))
{
    Console.WriteLine("File should be .keegz or StaticInfo.xml");
    End();
    return;
}
string s ="";
if (f.EndsWith("StaticInfo.xml"))
{
    s=File.ReadAllText(f);
} else
using (ZipArchive archive = ZipFile.OpenRead(f))
{
    foreach (ZipArchiveEntry entry in archive.Entries)
    {
        // r += Line(entry.FullName);
        if (entry.FullName.Equals("StaticInfo.xml"))
        { 
            var sr=new StreamReader(entry.Open());
            s=sr.ReadToEnd();                        
        }
    }
}
if (s.Length==0)
{
    Console.WriteLine("No StaticInfo.xml");
    End();
    return;
}
XmlDocument doc=null;
try
{
    doc = new XmlDocument();
    doc.LoadXml(s);
}
catch
{
    Console.WriteLine("Error reading XLM");
    End();
    return;
}
r += Line("\nPatient:");
r += Entry(doc, "Id","Patient");
r += Entry(doc, "LastName", "Patient");
r += Entry(doc, "FirstName", "Patient");
r += Entry(doc, "MiddleNames", "Patient");
r += Entry(doc, "DateOfBirth", "Patient");
r += Entry(doc, "GestationalAge", "Patient");
r += Entry(doc, "GenderMale", "Patient");
r += Entry(doc, "Notes", "Patient");
r += Entry(doc, "PatientHistory", "Patient");
r += Entry(doc, "SocialSecurityNumber", "Patient");

r += Line("\nExam:");
r += Entry(doc, "RecordedDate", "Exam");
r += Entry(doc, "Id", "Exam");
r += Entry(doc, "Anesthesiologist", "Exam");
r += Entry(doc, "ClinicalReason", "Exam");
r += Entry(doc, "Diagnosis", "Exam");
r += Entry(doc, "Medication", "Exam");
r += Entry(doc, "Notes", "Exam");
r += Entry(doc, "PatientState", "Exam");
r += Entry(doc, "Neurophysiologist", "Exam");
r += Entry(doc, "Physician", "Exam");
r += Entry(doc, "RequestedBy", "Exam");
r += Entry(doc, "Secretary", "Exam");
r += Entry(doc, "Surgeon", "Exam");
/*
r += Line("\nVisit");
r += Entry(doc, "Height", "Visit");
r += Entry(doc, "Weight", "Visit");
*/
Console.WriteLine("");
f += ".txt";
if (File.Exists(f))
{
    Console.WriteLine("File " + f + " already exist!");
}
else
{
    try
    {
        File.WriteAllText(f, r);
        Console.WriteLine("File " + f + " created.");
    }
    catch
    {
        Console.WriteLine("Error creating " + f);
    }
}
End();
   

// Find certain element
static string Entry(XmlDocument doc, string header, string group)
{
    string r = "";
    XmlNodeList elemList = doc.GetElementsByTagName("a:" + header);   
    for (int i = 0; i < elemList.Count; i++)
    { 
        if (elemList[i].ParentNode.Name.Equals(group))
            r += Line(header + ": " + elemList[i].InnerXml);
    }
    return r;
}

// Display line
static string Line(string s)
{
    Console.WriteLine(s);
    return s + "\n";
}

// Close info
static void End()
{
    Console.Write("Press any key or close window...");
    Console.ReadKey();
}

// End
