using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PeopleDatabase
{
    public enum Sex
    {
        Unknown,
        Male,
        Female,
    }

    public class Person
    {
        public string Id { get; set; } = string.Empty;
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string PersonalId { get; set; } = string.Empty;
        
        public DateTimeOffset DateOfBirth { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Sex Sex { get; set; }

    }
    public static class PersonExtensions
    {
        public static Person AddPerson(this Person p)

        {
            string id = Guid.NewGuid().ToString();
            Console.Write("Enter first name: ");
            string fn = Console.ReadLine() ?? string.Empty;;
            Console.Write("Enter last name: ");
            string ln = Console.ReadLine() ?? string.Empty;;
            Console.Write("Enter personal ID: ");
            string personalId = Console.ReadLine() ?? string.Empty;;
            Console.Write("Enter date of birth: ");
            string date = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter sex: ");
            string sex = Console.ReadLine() ?? string.Empty;
            Person person = new()
            {
                Id = id,
                FirstName = fn,
                LastName = ln,
                PersonalId = personalId,
                DateOfBirth = DateTimeOffset.Parse(date),
                Sex = Enum.Parse<Sex>(sex),
            };
            return person;
        }

        public static Person SetFirstName(this Person p)
        {
            Console.Write("Enter new first name: ");
            string fn = Console.ReadLine() ?? string.Empty;;
            p.FirstName = fn;
            return p;
        }
        public static Person SetLastName(this Person p)
        {
            Console.Write("Enter new last name: ");
            string ln = Console.ReadLine() ?? string.Empty;;
            p.LastName = ln;
            return p;
        }
        public static Person SetPersonalId(this Person p)
        {
            Console.Write("Enter new personal ID: ");
            string id = Console.ReadLine() ?? string.Empty;;
            p.PersonalId = id;
            return p;
        }
        public static Person SetDateOfBirth(this Person p)
        {
            Console.Write("Enter new date of birth: ");
            string dob = Console.ReadLine() ?? string.Empty;
            p.DateOfBirth = DateTimeOffset.Parse(dob);
            return p;
        }
        public static Person SetSex(this Person p)
        {
            Console.Write("Enter new sex: ");
            string sex = Console.ReadLine() ?? string.Empty;
            p.Sex = Enum.Parse<Sex>(sex);
            return p;
        }
    }
    internal static class Program
    {
        private static void Main()
        {
            string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            const string databaseFileName = "database.json";
            string databaseFilePath = Path.Combine(applicationDataPath, databaseFileName);
            
            if (!File.Exists(databaseFilePath))
            {
                File.WriteAllText(databaseFilePath, contents: "[]");
            }
            
            string databaseFileContent = File.ReadAllText(databaseFilePath);
            var people = JsonSerializer.Deserialize<List<Person>>(databaseFileContent) ?? new List<Person>();
            
            while (true)
            {
                Console.Write("\n>");
                string? command = Console.ReadLine();
                if (command == null) continue;
                
                string[] args = command.Split(" ");
                if (args[0] == "help")
                {
                    if (args.Length > 1)
                    {
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else
                    {
                        Console.WriteLine("\nAvailable commands:");
                        Console.WriteLine("add          Adds a new entry into the database");
                        Console.WriteLine("edit         Modifies an entry by given ID");
                        Console.WriteLine("delete       Removes an entry by given ID");
                        Console.WriteLine("list         Lists all entries in currently in the database");
                        Console.WriteLine("commit       Finalizes all changes and applies them to the database");
                        Console.WriteLine("discard      Discards all pending changes to the database");
                        Console.WriteLine("exit         Exits the application");
                    }
                }
                else if (args[0] == "add")
                {
                    if (args.Length > 1)
                    {
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else
                    {
                        Person obj = new();
                        people.Add(obj.AddPerson());
                    }
                }
                else if (args[0] == "list")
                {
                    if (args.Length > 1)
                    {
                        
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else
                    {
                        var originalPeople = JsonSerializer.Deserialize<List<Person>>(databaseFileContent) ?? new List<Person>();
                        foreach (var person in originalPeople)
                        {
                            Console.WriteLine($"\nID: {person.Id}");
                            Console.WriteLine($"First name: {person.FirstName}");
                            Console.WriteLine($"Last name: {person.LastName}");
                            Console.WriteLine($"Personal ID: {person.PersonalId}");
                            Console.WriteLine($"Date of birth: {person.DateOfBirth:d}");
                            Console.WriteLine($"Sex: {person.Sex}");
                        }
                    }
                }
                else if (args[0] == "edit")
                {
                    if (args.Length == 2)
                    {
                        string guid = args[1];
                        Person? personToEdit = people.FirstOrDefault(person => person.Id == guid);
                        if (personToEdit == null)
                        {
                            Console.WriteLine("Error: Incorrect ID");
                            continue;
                        }
                        
                        Console.WriteLine("Which attribute do you want to edit?");
                        Console.WriteLine("First name (fname)");
                        Console.WriteLine("Last name (lname)");
                        Console.WriteLine("Personal ID (pid)");
                        Console.WriteLine("Date of birth (dob)");
                        Console.WriteLine("Sex (sex)");
                        Console.Write(">");
                        string choice = Console.ReadLine()?.ToUpper() ?? string.Empty;
                        switch (choice)
                        {
                            case "FNAME":
                                personToEdit.SetFirstName();
                                break;
                            case "LNAME":
                                personToEdit.SetLastName();
                                break;
                            case "PID":
                                personToEdit.SetPersonalId();
                                break;
                            case "DOB":
                                personToEdit.SetDateOfBirth();
                                break;
                            case "SEX":
                                personToEdit.SetSex();
                                break;
                            default:
                                Console.WriteLine("Error: Incorrect attribute name");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: ID not specified or too many arguments passed");
                    }
                }
                else if (args[0] == "delete")
                {
                    if (args.Length == 2)
                    {
                        string guid = args[1];
                        Person? personToDelete = people.SingleOrDefault(person => person.Id == guid);
                        if (personToDelete == null)
                        {
                            Console.WriteLine("Error: Incorrect ID");
                        }
                        else
                        {
                            people.Remove(personToDelete);
                            Console.WriteLine($"Entry with ID {personToDelete.Id} was removed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: ID not specified or too many arguments passed");
                    }
                }
                else if (args[0] == "commit")
                {
                    JsonSerializerOptions options = new() { WriteIndented = true };
                    databaseFileContent = JsonSerializer.Serialize(people, options);
                    File.WriteAllText(databaseFilePath, contents: databaseFileContent);
                }
                else if (args[0] == "discard")
                {
                    people = JsonSerializer.Deserialize<List<Person>>(databaseFileContent) ?? new List<Person>();
                }
                else if (args[0] == "exit")
                {
                    break;
                }
            }
        }
    }
}
