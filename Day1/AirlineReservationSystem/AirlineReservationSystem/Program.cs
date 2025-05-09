
internal class Program
{
    private static void Main(string[] args)
    {

    }
}

class Person
{
    private static List<Person> people = new List<Person>();
    public string Name { get; protected set; }
    public string PassportNumber { get; protected set; }

    public PersonType Type { get; protected set; }

    public Person(string name, string passportNumber)
    {
        if(people.Any(x => x.PassportNumber == passportNumber))
            throw new Exception("Person with this passport number already exists");
        
        Name = name;
        PassportNumber = passportNumber;
        
        people.Add(this);
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

        Type = PersonType.Person;
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

        Type = PersonType.CrewMember;
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
    private string _password;

    public Person Person { get; private set; }


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
}
enum CrewRole
{
    Pilot,
    FlightAttendant
}

enum PersonType
{
    Person,
    CrewMember
}