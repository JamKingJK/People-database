using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace People_database
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string DateOfBirth { get; set; }
        public string Sex { get; set; }

    }
    public static class PersonExtensions
    {
        public static Person AddPerson(this Person p)
        {
            Console.Write("Enter first name: ");
            string fn = Console.ReadLine();
            Console.Write("Enter last name: ");
            string ln = Console.ReadLine();
            Console.Write("Enter personal ID: ");
            string id = Console.ReadLine();
            Console.Write("Enter date of birth: ");
            string date = Console.ReadLine();
            Console.Write("Enter sex: ");
            string sex = Console.ReadLine();
            Person person = new()
            {
                FirstName = fn,
                LastName = ln,
                PersonalId = id,
                DateOfBirth = date,
                Sex = sex,
            };
            return person;
        }
    }
    internal static class Program
    {
        private static void CommandListener()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            const string filename = "\\database.json";
            string fullpath = path + filename;
            using (var sw = (File.Exists(fullpath)) ? File.AppendText(fullpath) : File.CreateText(fullpath))                 
            {
                Console.WriteLine("Database location: " + fullpath);
            } 
            List<Person> people = new List<Person>();
            if( new FileInfo(fullpath).Length != 0 )
            {
                var jsontxt = File.ReadAllText(fullpath);
                var existingPeople = JsonSerializer.Deserialize<List<Person>>(jsontxt);
                people.AddRange(existingPeople);
            }
            while(true)
            {
                string command = Console.ReadLine();
                if (command == null) continue;
                string[] strlist = command.Split(" ");
                if (strlist[0] == "help")
                {
                    if (strlist.Length > 1)
                    {
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else
                    {
                        Console.Write("\nUsage: [add]\n");
                    }
                }
                else if (strlist[0] == "add")
                {
                    if (strlist.Length > 1)
                    {
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else
                    {
                        Person obj = new();
                        people.Add(obj.AddPerson());
                    }
                }
                else if (strlist[0] == "list") // TODO Fix Unhandled exception when given no arguments
                {
                    if (strlist.Length > 2)
                    {
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else if (strlist.Length <= 0)
                    {
                        Console.WriteLine("Error: Too few arguments");
                    }
                    else
                    {
                        switch (strlist[1])
                        {
                            case "all":
                                Console.Write(people.ToArray()); //TODO Make it list all values of the objects 
                                break;
                            case "existing":
                                break;
                        }
                    }
                }
                else if (strlist[0] == "commit")
                {
                    JsonSerializerOptions options = new() {WriteIndented = true};
                    string jsonString = JsonSerializer.Serialize(people, options);
                    File.WriteAllText(fullpath, jsonString);
                }
                else if (strlist[0] == "exit")
                {
                    break;
                }
            }
        }
        private static void Main(string[] args)
        {
            CommandListener();
        }
    }
}
