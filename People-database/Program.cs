using System;
using System.Collections.Generic;
using System.IO;

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
        private static void StartupPrep()
        {
            //Console.Write(File.ReadAllText(path));
            //var jsontxt = File.ReadAllText(path);
            //JsonSerializerOptions options = new() {WriteIndented = true};
            //string jsonString = JsonSerializer.Serialize(testlist, options);
            //File.WriteAllText(path, jsonString);
        }
        private static void CommandListener()
        {
            const string path = @"C:\Users\Kacper\RiderProjects\ConsoleApp1\ConsoleApp1\database.json"; //temp path
            string jsontxt = File.ReadAllText(path);
            
            // TODO Add deserializing json file to a list
            
            List<Person> people = new List<Person>();
            while(true)
            {
                string command = Console.ReadLine();
                String[] strlist = command.Split(" ");
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
