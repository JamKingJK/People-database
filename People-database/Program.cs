using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace People_database
{
    public class Person
    {
        public string Id { get; set; }
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
            string id = Guid.NewGuid().ToString();
            Console.Write("Enter first name: ");
            string fn = Console.ReadLine();
            Console.Write("Enter last name: ");
            string ln = Console.ReadLine();
            Console.Write("Enter personal ID: ");
            string personalId = Console.ReadLine();
            Console.Write("Enter date of birth: ");
            string date = Console.ReadLine();
            Console.Write("Enter sex: ");
            string sex = Console.ReadLine();
            Person person = new()
            {
                Id = id,
                FirstName = fn,
                LastName = ln,
                PersonalId = personalId,
                DateOfBirth = date,
                Sex = sex,
            };
            return person;
        }

        public static Person SetFirstName(this Person p)
        {
            Console.Write("Enter new first name: ");
            string fn = Console.ReadLine();
            p.FirstName = fn;
            return p;
        }
        public static Person SetLastName(this Person p)
        {
            Console.Write("Enter new last name: ");
            string ln = Console.ReadLine();
            p.LastName = ln;
            return p;
        }
        public static Person SetPersonalId(this Person p)
        {
            Console.Write("Enter new personal ID: ");
            string id = Console.ReadLine();
            p.PersonalId = id;
            return p;
        }
        public static Person SetDateOfBirth(this Person p)
        {
            Console.Write("Enter new date of birth: ");
            string dob = Console.ReadLine();
            p.DateOfBirth = dob;
            return p;
        }
        public static Person SetSex(this Person p)
        {
            Console.Write("Enter new sex: ");
            string sex = Console.ReadLine();
            p.Sex = sex;
            return p;
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
            if (new FileInfo(fullpath).Length != 0)
            {
                var jsontxt = File.ReadAllText(fullpath);
                var existingPeople = JsonSerializer.Deserialize<List<Person>>(jsontxt);
                people.AddRange(existingPeople);
            }
            while(true)
            {
                Console.Write("\n>");
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
                else if (strlist[0] == "list")
                {
                    if (strlist.Length > 1)
                    {
                        
                        Console.WriteLine("Error: Too many arguments");
                    }
                    else
                    {
                        var jsontxt = File.ReadAllText(fullpath);
                        var existingPeople = JsonSerializer.Deserialize<List<Person>>(jsontxt);
                        foreach (var person in existingPeople)
                        {
                            Console.WriteLine("\nID: " + person.Id);
                            Console.WriteLine("First name: " + person.FirstName);
                            Console.WriteLine("Last name: " + person.LastName);
                            Console.WriteLine("Personal ID: " + person.PersonalId);
                            Console.WriteLine("Date of birth: " + person.DateOfBirth);
                            Console.WriteLine("Sex: " + person.Sex);
                        }
                    }
                }
                else if (strlist[0] == "edit")
                {
                    if (strlist.Length == 2)
                    {
                        string guid = strlist[1];
                        var tempPerson = people.Find(person => person.Id == guid);
                        if (tempPerson == null)
                        {
                            Console.WriteLine("Error: Incorrect ID");
                        }
                        else
                        {
                            Console.WriteLine("Which attribute do you want to edit?");
                            Console.WriteLine("First name (fname)");
                            Console.WriteLine("Last name (lname)");
                            Console.WriteLine("Personal ID (pid)");
                            Console.WriteLine("Date of birth (dob)");
                            Console.WriteLine("Sex (sex)");
                            Console.Write(">");
                            string choice = Console.ReadLine().ToUpper();
                            switch (choice)
                            {
                                case "FNAME":
                                    tempPerson.SetFirstName();
                                    break;
                                case "LNAME":
                                    tempPerson.SetLastName();
                                    break;
                                case "PID":
                                    tempPerson.SetPersonalId();
                                    break;
                                case "DOB":
                                    tempPerson.SetDateOfBirth();
                                    break;
                                case "SEX":
                                    tempPerson.SetSex();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: ID not specified or too many arguments passed");
                    }
                }
                else if (strlist[0] == "delete")
                {
                    if (strlist.Length == 2)
                    {
                        string guid = strlist[1];
                        var deletee = people.Find(person => person.Id == guid);
                        if (deletee == null)
                        {
                            Console.WriteLine("Error: Incorrect ID");
                        }
                        else
                        {
                            people.Remove(deletee);
                            Console.WriteLine($"Entry with ID {deletee.Id} was removed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: ID not specified or too many arguments passed");
                    }
                }
                else if (strlist[0] == "commit")
                {
                    JsonSerializerOptions options = new() {WriteIndented = true};
                    string jsonString = JsonSerializer.Serialize(people, options);
                    File.WriteAllText(fullpath, jsonString);
                }
                else if (strlist[0] == "discard")
                {
                    people.Clear();
                    var jsontxt = File.ReadAllText(fullpath);
                    var existingPeople = JsonSerializer.Deserialize<List<Person>>(jsontxt);
                    people.AddRange(existingPeople);
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
