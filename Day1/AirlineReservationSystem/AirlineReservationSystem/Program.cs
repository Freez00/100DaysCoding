using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        createAccountExample();
        loadAccountExample();
    }

    static void createAccountExample()
    {
        //Create some Person objects
        Person passenger1 = new Passenger("John Doe", "P12345");
        Person crewMember1 = new CrewMember("Alice Smith", "C67890", CrewRole.Pilot);

        // Create Accounts for these Persons
        Account account1 = new Account("john_doe", "password123", passenger1);
        Account account2 = new Account("alice_smith", "securepass", crewMember1);

        // Add Accounts to AccountManager
        AccountManager.AddAccount(account1);
        AccountManager.AddAccount(account2);

        // Save Accounts to a JSON file
        AccountManager.SaveAccountsToFile("accounts.json");

        Console.WriteLine("Accounts saved to file.");
    }

    static void loadAccountExample()
    {
        string filePath = "accounts.json";
        AccountManager.LoadAccountsFromFile(filePath);

        // Display information about all accounts
        foreach (var accountEntry in AccountManager.Accounts)
        {
            var account = accountEntry.Value;
            Console.WriteLine($"Username: {account.Username}");
            Console.WriteLine($"Person: {account.Person.Name}, Passport: {account.Person.PassportNumber}, Type: {account.Person.Type}");
            Console.WriteLine();
        }
    }
}

abstract class Person
{
    private static List<Person> people = new List<Person>();
    public string Name { get; protected set; }
    public string PassportNumber { get; protected set; }
    public abstract string Type { get; }

    [JsonConstructor]
    public Person(string name, string passportNumber)
    {
        if(people.Any(x => x.PassportNumber == passportNumber))
            throw new Exception("Person with this passport number already exists");
        
        Name = name;
        PassportNumber = passportNumber;
        
        people.Add(this);
    }

    public Person()
    {

    }

    public override string ToString()
    {
        return $"{Name} [{PassportNumber}]";
    }
}

class Passenger : Person
{
    public List<Flight> BookedFlights { get; private set; }
    public override string Type => "Passenger";

    [JsonConstructor]
    public Passenger(string name, string passportNumber, params Flight[] flights)
        :base(name, passportNumber)
    {
        BookedFlights = new List<Flight>();
        BookedFlights.AddRange(flights);
    }

    public void BookFlight(Flight flight)
    {
        if (BookedFlights.Contains(flight))
        {
            Console.WriteLine($"Flight {flight} already booked for {this}");
            return;
        }

        BookedFlights.Add(flight);
    }

    public void CancelFlight(Flight flight)
    {
        BookedFlights.Remove(flight);
    }

    public void CancelFlight(string flightNumber)
    {
        BookedFlights.RemoveAll(x => x.FlightNumber == flightNumber);
    }
}

class CrewMember : Person
{
    public CrewRole Role { get; private set; }
    public List<Flight> AssignedFlights { get; private set; }
    public override string Type => "CrewMember";

    [JsonConstructor]
    public CrewMember(string name, string passportNumber, params Flight[] assignedFlights)
        :base(name, passportNumber)
    {
        AssignedFlights = new List<Flight>();
        AssignedFlights.AddRange(assignedFlights);
    }

    public CrewMember(string name, string passportNumber, CrewRole role, params Flight[] assignedFlights)
        :this(name, passportNumber, assignedFlights)
    {
        Role = role;
    }

    public void AssignRole(CrewRole role)
    {
        Role = role;
    }

    public void AssignFlight(Flight flight)
    {
        if (AssignedFlights.Contains(flight))
        {
            Console.WriteLine($"Flight {flight} already assigned to {this}");
            return;
        }

        AssignedFlights.Add(flight);
    }

    public void RemoveFlight(Flight flight)
    {
        AssignedFlights.Remove(flight);
    }

    public void RemoveFlight(string flightNumber)
    {
        AssignedFlights.RemoveAll(x => x.FlightNumber == flightNumber);
    }
}

class Flight
{
    private static List<Flight> flights = new List<Flight>();
    public string FlightNumber { get; private set; }
    public List<Passenger> Passengers { get; private set; }
    public CrewMember Captain { get; private set; }
    public List<CrewMember> Crew { get; private set; }
    public int PassengerCapacity { get; private set; }

    public Flight(string flightNumber, int passengerCapacity, 
        CrewMember captain, params CrewMember[] crew)
    {
        if(flights.Any(x => x.FlightNumber == flightNumber))
            throw new Exception($"Flight with that number already exists [{flightNumber}]");

        FlightNumber = flightNumber;
        PassengerCapacity = passengerCapacity;
        Passengers = new List<Passenger>();
        
        Crew = new List<CrewMember>();
        Crew.AddRange(crew);

        if(captain != null)
            AssignCaptain(captain);

        flights.Add(this);
    }

    public Flight(string flightNumber, int passengerCapacity, params CrewMember[] crew)
        :this(flightNumber, passengerCapacity, null, crew)
    {

    }

    public Flight(string flightNumber, int passengerCapacity)
        : this(flightNumber, passengerCapacity, [])
    {

    }

    public bool AddPassenger(Passenger passenger)
    {
        if (Passengers.Count() + 1 > PassengerCapacity)
            return false;

        if (!Passengers.Contains(passenger))
            Passengers.Add(passenger);

        return true;
    }

    public void RemovePassenger(Passenger passenger)
    {
        Passengers.Remove(passenger);
    }

    public bool AssignCaptain(CrewMember captain)
    {
        if (captain.Role != CrewRole.Pilot)
            return false;

        if (!Crew.Contains(captain))
            Crew.Add(captain);

        Captain = captain;

        
        return true;
    }

    public void AssignCrewMember(CrewMember member)
    {
        if (!Crew.Contains(member))
            Crew.Add(member);
    }
    public void RemoveCrewMember(CrewMember member)
    {
        if (member != Captain)
            Crew.Remove(member);
    }
}


class Account
{
    public string Username { get; private set; }
    public string _password { get; private set; }
    
    public Person Person { get; private set; }

    [JsonConstructor]
    public Account(string username, string password, Person person)
    {
        Username = username;
        _password = password;
        Person = person;
    }
    public Account() { }

    public static Account Create(string username, string password, Person person)
    {
        if (AccountManager.Accounts.ContainsKey(username))
            return null;

        return new Account(username, password, person);
    }

    public bool VerifyPassword(string password)
    {
        return _password == password;
    }
}

static class AccountManager
{
    public static Dictionary<string, Account> Accounts { get; private set; } = new Dictionary<string, Account>();

    public static bool AddAccount(Account account)
    {
        if (Accounts.Any(x => x.Key == account.Username))
            return false;

        Accounts.Add(account.Username, account);
        return true;
    }

    public static bool RegisterAccount(string username, string password, Person person)
    {
        Account account = Account.Create(username, password, person);
        
        if(account != null)
            return false;

        Accounts.Add(account.Username, account);
        return true;
    }

    public static Account GetAccountByName(string username)
    {
        Account acc;
        if (Accounts.TryGetValue(username, out acc))
            return acc;

        return null;
    }

    public static Account Authenticate(string username, string password)
    {
        var acc = GetAccountByName(username);
        if (acc != null && acc.VerifyPassword(password))
            return acc;

        return null;
    }

    // Serialization - Saving to file
    public static void SaveAccountsToFile(string filePath)
    {
        var json = JsonSerializer.Serialize(Accounts);
        File.WriteAllText(filePath, json);
    }

    // Deserialization - Loading from file
    public static void LoadAccountsFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                Converters = { new PersonConverter(), new AccountJsonConverter() }
            };

            string json = File.ReadAllText(filePath);
            var accounts = JsonSerializer.Deserialize<Dictionary<string, Account>>(json, options);
        }
    }

}

class PersonConverter : JsonConverter<Person>
{
    public override Person Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty("Type", out var typeProperty))
        {
            var type = typeProperty.GetString();
            switch (type)
            {
                case "Passenger":
                    return JsonSerializer.Deserialize<Passenger>(root.GetRawText(), options);
                case "CrewMember":
                    return JsonSerializer.Deserialize<CrewMember>(root.GetRawText(), options);
            }
        }

        throw new JsonException("Unknown person type");
    }

    public override void Write(Utf8JsonWriter writer, Person value, JsonSerializerOptions options)
    {
        if (value is Passenger passenger)
        {
            JsonSerializer.Serialize(writer, passenger, options);
        }
        else if (value is CrewMember crew)
        {
            JsonSerializer.Serialize(writer, crew, options);
        }
        else
        {
            throw new JsonException("Unknown type");
        }
    }
}


class AccountJsonConverter : JsonConverter<Account>
{
    public override Account Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var username = root.GetProperty("Username").GetString();
        var password = root.GetProperty("_password").GetString();
        var personJson = root.GetProperty("Person").GetRawText();

        var person = JsonSerializer.Deserialize<Person>(personJson, options);

        return new Account(username, password, person);
    }

    public override void Write(Utf8JsonWriter writer, Account value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Username", value.Username);
        writer.WriteString("_password", value._password);
        writer.WritePropertyName("Person");
        JsonSerializer.Serialize(writer, value.Person, options);
        writer.WriteEndObject();
    }
}




enum CrewRole
{
    Pilot,
    FlightAttendant
}