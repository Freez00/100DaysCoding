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

    public Person(string name, string passportNumber)
    {
        if(people.Where(x => x.PassportNumber == passportNumber).FirstOrDefault() != null)
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
    public string FlightNumber { get; private set; }
    public List<Passenger> Passengers { get; private set; }

    public CrewMember Captain { get; private set; }

    public List<CrewMember> Crew { get; private set; }

    public int PassengerCapacity { get; private set; }

    // CONTINUE
}

enum CrewRole
{
    Pilot,
    FlightAttendant
}