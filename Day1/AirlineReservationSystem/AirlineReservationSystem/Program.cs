
internal class Program
{
    private static void Main(string[] args)
    {
        //SeedApp();
        //FlightManager.ListAvailableFlights();

        Menu menu1 = new Menu("hello");
        menu1.AddOption("option 1", new ActionMenu(sayHey));
        menu1.AddOption("option 2", new ActionMenu(sayHey2));

        Menu menu2 = new Menu("authentication screen");

        menu2.AddOption("login", new ActionMenu((object? ctx) => { Console.WriteLine("cant login" + ctx); }));
        menu2.AddOption("register", new ActionMenu((object? _) => { Console.WriteLine("service unavailable atm"); }));
        menu2.AddOption("menu1", new SubMenu(menu1));

        menu1.AddOption("option 3", new SubMenu(menu2));

        menu1.Run();


    }

    private static void sayHey(object? _)
    {
        Console.WriteLine("Hey");
    }

    private static void sayHey2(object? _)
    {
        Console.WriteLine("Heyyyy");
    }

    private static void SeedApp()
    {
        // === CREW MEMBERS ===
        var captains = new List<CrewMember>
    {
        new CrewMember("Daniel Grant", "C001", CrewRole.Pilot),
        new CrewMember("Elena Novak", "C002", CrewRole.Pilot),
        new CrewMember("Thomas Becker", "C003", CrewRole.Pilot),
    };

        var crewMembers = new List<CrewMember>
    {
        new CrewMember("Sara Jensen", "C004", CrewRole.FlightAttendant),
        new CrewMember("Liam Carter", "C005", CrewRole.FlightAttendant),
        new CrewMember("Natalie Brooks", "C006", CrewRole.FlightAttendant),
        new CrewMember("Victor Schmidt", "C007", CrewRole.FlightAttendant),
        new CrewMember("Anna Petrova", "C008", CrewRole.FlightAttendant),
    };

        // === FLIGHTS ===
        var flight1 = new Flight("FL100", 4, captains[0], crewMembers[0], crewMembers[1]);
        var flight2 = new Flight("FL200", 3, captains[1], crewMembers[2], crewMembers[3]);
        var flight3 = new Flight("FL300", 5, captains[2], crewMembers[4]);
        var flight4 = new Flight("FL400", 2, captains[0], crewMembers[2]);
        var flight5 = new Flight("FL500", 6, captains[1], crewMembers[0], crewMembers[3]);

        var allFlights = new List<Flight> { flight1, flight2, flight3, flight4, flight5 };

        // === ASSIGN FLIGHTS TO CREW ===
        foreach (var crew in captains.Concat(crewMembers))
        {
            foreach (var flight in allFlights.Where(f => f.Crew.Contains(crew)))
            {
                crew.AssignFlight(flight);
            }
        }

        // === PASSENGERS ===
        var passengers = new List<Passenger>
    {
        new Passenger("Michael Adams", "P100"),
        new Passenger("Julia Meyer", "P101"),
        new Passenger("Oscar Lind", "P102"),
        new Passenger("Chloe Park", "P103"),
        new Passenger("Ivan Dimitrov", "P104"),
        new Passenger("Zara Ahmed", "P105"),
        new Passenger("Lucas Marin", "P106"),
        new Passenger("Sofia Costa", "P107"),
    };

        // === ASSIGN PASSENGERS TO FLIGHTS ===
        passengers[0].BookFlight(flight1); flight1.AddPassenger(passengers[0]);
        passengers[1].BookFlight(flight1); flight1.AddPassenger(passengers[1]);
        passengers[2].BookFlight(flight2); flight2.AddPassenger(passengers[2]);
        passengers[3].BookFlight(flight3); flight3.AddPassenger(passengers[3]);
        passengers[4].BookFlight(flight4); flight4.AddPassenger(passengers[4]);
        passengers[5].BookFlight(flight5); flight5.AddPassenger(passengers[5]);
        passengers[6].BookFlight(flight5); flight5.AddPassenger(passengers[6]);
        passengers[7].BookFlight(flight2); flight2.AddPassenger(passengers[7]);

        // === ACCOUNTS ===
        foreach (var passenger in passengers)
        {
            AccountManager.RegisterAccount(passenger.Name.ToLower().Split()[0], "pass123", passenger);
        }

        foreach (var crew in captains.Concat(crewMembers))
        {
            var uname = crew.Name.ToLower().Split()[0];
            AccountManager.RegisterAccount(uname, "crewpass", crew);
        }
    }
}

class Person
{
    private static List<Person> people = new List<Person>();
    public string Name { get; protected set; }
    public string PassportNumber { get; protected set; }

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

    public static List<Flight> GetFlights()
    {
        return flights;
    }

    public override string ToString()
    {
        return $"[{FlightNumber}] cap.{Captain} --- Available seats: {PassengerCapacity - Passengers.Count()}";
    }
}

class Account
{
    public string Username { get; private set; }
    private string _password;

    public Person Person { get; private set; }

    public Account(string username, string password)
    {
        Username = username;
        _password = password;
        Person = new Person();
    }
    public Account(string username, string password, Person person)
    {
        Username = username;
        _password = password;
        Person = person;
    }

    public void AssignPerson(Person person)
    {
        Person = person;
    }

    public bool VerifyPassword(string password)
    {
        return _password == password;
    }
}

static class AccountManager
{
    public static Dictionary<string, Account> Accounts { get; private set; } = new Dictionary<string, Account>();

    public static bool RegisterAccount(Account acc)
    {
        if(Accounts.Any(x => x.Key == acc.Username))
            return false;

        Accounts.Add(acc.Username, acc);
        return true;
    }

    public static Account Authenticate(string username, string password)
    {
        Account account = getAccountByUsername(username);
        if ( account != null && account.VerifyPassword(password))
            return account;

        return null;
    }

    public static bool RegisterAccount(string username, string password, Person person)
    {
        Account acc = new Account(username, password, person);
        return RegisterAccount(acc);
    }

    public static bool RegisterAccount(string username, string password)
    {
        Account acc = new Account(username, password);
        return RegisterAccount(acc);
    }

    private static Account getAccountByUsername(string username)
    {
        Account account;
        if (Accounts.TryGetValue(username, out account))
            return account;

        return null;
    }
}

static class FlightManager
{
    public static List<Flight> Flights { get { return Flight.GetFlights(); }}

    public static List<Flight> ListAvailableFlights()
    {
        var flights = Flights.Where(x => x.PassengerCapacity - x.Passengers.Count() != 0).ToList();

        foreach(var f in flights)
        {
            Console.WriteLine("\n"+f);
            Console.WriteLine("Passengers on board:");
            foreach (var p in f.Passengers)
                Console.WriteLine("\t"+p);
            Console.WriteLine("Crew:");
            foreach (var c in f.Crew)
                Console.WriteLine("\t" + c);
            Console.WriteLine();
        }

        return flights;
    }
}

class Menu 
{
    private string? _title;
    private Dictionary<string, IMenuItem> _options;

    private int selectedIndex = 0;

    public Menu(Dictionary<string, IMenuItem> options, string? title = null)
    {
        _title = title;
        _options = options;
    }

    public Menu(string? title = null)
    {
        _title = title;
        _options = new Dictionary<string, IMenuItem>();
    }

    public void AddOption(string label, IMenuItem action)
    {
        _options.Add(label, action);
    }

    public void Run()
    {
        while (true)
        {
            DisplayOptions();
            TakeInput();
        }
    }

    private void DisplayOptions()
    {
        Console.Clear();
        Console.WriteLine(_title);
        for (int i = 0; i < _options.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.WriteLine(">" + _options.ElementAt(i).Key);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(_options.ElementAt(i).Key);
            }
            
        }
    }

    private void TakeInput()
    {
        ConsoleKey key = Console.ReadKey().Key;

        switch (key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                selectedIndex = selectedIndex == 0 ? _options.Count - 1 : selectedIndex - 1;
                break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                selectedIndex = selectedIndex == _options.Count - 1 ? 0 : selectedIndex + 1;
                break;

            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                _options.ElementAt(selectedIndex).Value.Run();
                Console.ReadLine();
                break;
        }
    }
}

interface IMenuItem
{
    void Run(object? context = null);
}

class SubMenu : IMenuItem
{
    private readonly Menu _submenu;

    public SubMenu(Menu submenu)
    {
        _submenu = submenu;
    }

    public void Run(object? context = null)
    {
        _submenu.Run();
    }
}

class ActionMenu : IMenuItem
{
    private readonly Action<object?> _action;

    public ActionMenu(Action<object?> action)
    {
        _action = action;
    }

    public void Run(object? context = null)
    {
        _action(context);
    }
}

enum CrewRole
{
    Pilot,
    FlightAttendant
}