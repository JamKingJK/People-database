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
        private static void PrintHelp()
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

        private static void AddPatient(List<Person> peopleTemporary)
        {
            string firstName = GetUserInput("Enter first name: ");
            string lastName = GetUserInput("Enter last name: ");
            string personalId = GetUserInput("Enter personal ID: ");
            string dateString = GetUserInput("Enter date of birth: ");
            string sexString = GetUserInput("Enter sex: ");

            var person = new Person
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                PersonalId = personalId,
                DateOfBirth = DateTimeOffset.Parse(dateString),
                Sex = Enum.Parse<Sex>(sexString),
            };
            
            peopleTemporary.Add(person);
        }

        private static string GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

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
            var peopelOriginal = JsonSerializer.Deserialize<List<Person>>(databaseFileContent) ?? new List<Person>();
            var peopleTemporary = new List<Person>(peopelOriginal);
            
            while (true)
            {
                Console.Write("\n>");
                string? command = Console.ReadLine();
                if (command == null) continue;
                
                string[] args = command.Split(" ");
                switch (args[0])
                {
                    case "add":
                        AddPatient(peopleTemporary);
                        break;
                    case "list":
                        ListPatients(args, peopelOriginal);
                        break;
                    case "edit":
                        EditPatient(args, peopleTemporary);
                        break;
                    case "delete":
                        DeletePatient(args, peopleTemporary);
                        break;
                    case "commit":
                        databaseFileContent = CommitChanges(peopleTemporary, databaseFilePath);
                        peopelOriginal.Clear();
                        peopleTemporary.ForEach(val => { peopelOriginal.Add(val); });
                        break;
                    case "discard":
                        peopleTemporary = DiscardChanges(databaseFileContent);
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine($"Unknown option: {args[0]}");
                        goto case "help";
                    case "help":
                        PrintHelp();
                        break;
                }
            }
        }

        private static List<Person> DiscardChanges(string databaseFileContent)
        {
            return JsonSerializer.Deserialize<List<Person>>(databaseFileContent) ?? new List<Person>();
        }

        private static string CommitChanges(List<Person>? peopleTemporary, string databaseFilePath)
        {
            JsonSerializerOptions options = new() {WriteIndented = true};
            string databaseFileContent = JsonSerializer.Serialize(peopleTemporary, options);
            File.WriteAllText(databaseFilePath, contents: databaseFileContent);
            
            return databaseFileContent;
        }

        private static void DeletePatient(string[] args, List<Person> people)
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

        private static void EditPatient(string[] args, List<Person> people)
        {
            if (args.Length == 2)
            {
                string guid = args[1];
                Person? personToEdit = people.FirstOrDefault(person => person.Id == guid);
                if (personToEdit == null)
                {
                    Console.WriteLine("Error: Incorrect ID");
                    return;
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

        private static void ListPatients(string[] args, List<Person>originalPeople)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Error: Too many arguments");
                return;
            }
            
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
}
