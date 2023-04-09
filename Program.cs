using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


class Program
{
    static void Main(string[] args)
    {
        int DELAY = 1000;

        string filename = "";

        FileManager fileManager = new FileManager();
        Journal journal = new Journal();

        bool running = true;
        while (running)
        {

            UserInterface.DisplayMainMenu();
            string input = UserInterface.GetMenuOption();

            switch (input)
            {
                case "1":
                    string prompt = UserInterface.GetRandomPrompt();
                    string response = UserInterface.GetEntryResponse(prompt);
                    journal.AddEntry(new Entry(prompt, response));
                    UserInterface.DisplayMessage("Entry saved to journal.");
                    Thread.Sleep(DELAY);
                    break;

                case "2":
                    UserInterface.DisplayJournal(journal);
                    Thread.Sleep(DELAY);
                    break;

                case "3":
                    DateTime start = UserInterface.GetDate("starting");
                    DateTime end = UserInterface.GetDate("ending").AddDays(1);

                    Journal dateSearch = new Journal(journal.FindEntriesByDateRange(start, end));
                    UserInterface.DisplayJournal(dateSearch);
                    Thread.Sleep(DELAY);
                    break;

                case "4":
                    string keyword = UserInterface.GetKeyword();
                    Journal keywordSearch = new Journal(journal.FindEntriesByKeyword(keyword));
                    UserInterface.DisplayJournal(keywordSearch);
                    Thread.Sleep(DELAY);
                    break;

                case "5":
                    filename = UserInterface.GetFileName();
                    FileManager.SaveJournalToFile(journal, filename);
                    Thread.Sleep(DELAY);
                    break;

                case "6":
                    filename = UserInterface.GetFileName();
                    journal = FileManager.LoadJournalFromFile(filename);
                    Thread.Sleep(DELAY);
                    break;

                case "7":
                    running = false;

                    if (UserInterface.PromptSaveChanges())
                    {
                        filename = UserInterface.GetFileName();
                        FileManager.SaveJournalToFile(journal, filename);
                    }
                    UserInterface.DisplayMessage("Thank you!");
                    break;

                default:
                    UserInterface.DisplayMessage("Invalid input, please try again.");
                    Thread.Sleep(DELAY);
                    break;
            }
        }
    }
}


class UserInterface
{

    enum YesOrNo { N = 0, NO = 0, Y = 1, YES = 1};

    public static void DisplayMainMenu()
    {

        int DELAY = 50;

        Console.WriteLine("\nJournal App");
        Thread.Sleep(DELAY);
        Console.WriteLine("===========");
        Thread.Sleep(DELAY);
        Console.WriteLine("1. Write a new journal entry");
        Thread.Sleep(DELAY);
        Console.WriteLine("2. Display all journal entries");
        Thread.Sleep(DELAY);
        Console.WriteLine("3. Search for entries by date range");
        Thread.Sleep(DELAY);
        Console.WriteLine("4. Search for entries by keyword");
        Thread.Sleep(DELAY);
        Console.WriteLine("5. Save journal to a file");
        Thread.Sleep(DELAY);
        Console.WriteLine("6. Load journal from a file");
        Thread.Sleep(DELAY);
        Console.WriteLine("7. Exit\n");
    }

    public static void DisplayEntryPrompt(string prompt)
    {
        Console.WriteLine(prompt);
    }

    public static string GetEntryResponse(string prompt)
    {
        DisplayEntryPrompt(prompt);
        Console.Write("> ");
        return Console.ReadLine();
    }

    public static void DisplayJournal(Journal journal)
    {
        List<Entry> entries = journal.GetAllEntries();

        if (entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
        }
        else
        {
            Console.WriteLine("Journal Entries");
            Console.WriteLine("===============");

            foreach (Entry entry in entries)
            {
                Console.WriteLine($"Date: {entry.date.ToString()}");
                Console.WriteLine($"Prompt: {entry.prompt}");
                Console.WriteLine($"Response: {entry.response}");
                Console.WriteLine();
            }
        }
    }

    public static string GetFileName()
    {
        string fileName = "";
        bool isValidFileName = false;

        while (!isValidFileName)
        {
            Console.Write("Enter file name: ");
            fileName = Console.ReadLine();

            // Check if the file name is valid
            try
            {
                new FileInfo(fileName);
                isValidFileName = true;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid file name. Please enter a valid file name.");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("File path is too long. Please enter a shorter file name.");
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("Invalid file name. Please enter a valid file name.");
            }
        }

        return fileName;
    }

    public static void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }

    public static string GetRandomPrompt()
    {

        Random random = new Random();
        int randomNumber = random.Next(0, 15);

        Dictionary<int, string> prompts = new Dictionary<int, string>()
        {
            { 0, "What are your goals for the next month, and how do you plan to achieve them?" },
            { 1, "Write about a time when you faced a significant challenge and how you overcame it." },
            { 2, "Describe a place that holds special meaning to you and why." },
            { 3, "Write about a person who has influenced your life and explain how they have impacted you." },
            { 4, "What is something you have been putting off, and why? What can you do to start working towards it?" },
            { 5, "Write about a recent experience that made you feel grateful." },
            { 6, "What is your favorite memory from your childhood, and why does it stand out to you?" },
            { 7, "Describe a recent decision you made and how it has affected your life." },
            { 8, "Write about a mistake you made and what you learned from it." },
            { 9, "What is a hobby or interest you have that you would like to explore more deeply, and why?" },
            { 10, "Write about a time when you felt particularly proud of yourself." },
            { 11, "Describe a dream you had recently and what you think it might mean." },
            { 12, "Write about a time when you felt particularly anxious or stressed and what you did to manage those feelings." },
            { 13, "What is something that you are currently struggling with, and what steps can you take to overcome it?" },
            { 14, "Write about a recent experience where you learned something new about yourself." }
        };


        return prompts[randomNumber];
    }

    public static string GetMenuOption()
    {
        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();

            // check if input is a valid number between 1 and 7
            if (int.TryParse(input, out int number) && number >= 1 && number <= 7)
            {
                return input;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 7.");
            }
        }
    }

    public static string GetKeyword()
    {
        Console.Write("Enter keyword: ");
        return Console.ReadLine();
    }

    public static DateTime GetDate(string typeOfDate)
    {
        DateTime date = new DateTime();
        bool isValid = false;

        while (!isValid)
        {
            Console.Write($"Enter {typeOfDate} date: ");
            string input = Console.ReadLine();

            try
            {
                date = DateTime.ParseExact(input, "d", CultureInfo.InvariantCulture);
                isValid = true;
            }
            catch (FormatException)
            {
                Console.WriteLine($"Invalid date format. Enter MM/DD/YYY (e.g. 01/01/1999).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        return date;
    }

    public static bool PromptSaveChanges()
    { 
        Console.WriteLine("Do you want to save your changes? ");
        Console.Write("Yes or No: ");
        string userInput = Console.ReadLine().ToLower();

        bool isUserInputValid = false;
        while (!isUserInputValid)
        {
            if (userInput == "yes")
            {
                return true;
            }
            else if (userInput == "no")
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter Yes or No.");
                Console.Write("Yes or No: ");
                userInput = Console.ReadLine().ToLower();
            }
        }
        return false;
    }
}


class FileManager
{
    public static void SaveJournalToFile(Journal journal, string filename)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                options.Converters.Add(new JsonStringEnumConverter());
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                string json = JsonSerializer.Serialize(journal.GetAllEntries(), options);
                writer.Write(json);
            }

            Console.WriteLine($"Journal saved to file: {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving journal to file: {ex.Message}");
        }
    }

    public static Journal LoadJournalFromFile(string filename)
    {
        Journal journal = new Journal();

        try
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string json = reader.ReadToEnd();
                List<Entry> entries = JsonSerializer.Deserialize<List<Entry>>(json);

                foreach (Entry entry in entries)
                {
                    journal.AddEntry(entry);
                }
            }

            Console.WriteLine($"Journal loaded from file: {filename}");
        }
        catch
        {
            Console.WriteLine($"Error loading journal from file: {filename}");
        }

        return journal;
    }

}


class Journal
{
    private List<Entry> Entries;

    public Journal()
    {
        Entries = new List<Entry>();
    }

    public Journal(List<Entry> entries)
    {
        Entries = entries;
    }

    public void AddEntry(Entry entry)
    {
        Entries.Add(entry);
    }

    public void RemoveEntry(int index)
    {
        Entries.RemoveAt(index);
    }

    public List<Entry> GetAllEntries()
    {
        return Entries;
    }

    public void ClearEntries()
    {
        Entries.Clear();
    }

    public List<Entry> FindEntriesByDateRange(DateTime start, DateTime end)
    {
        List<Entry> entriesInRange = new List<Entry>();
        foreach (Entry entry in Entries)
        {
            if (entry.date >= start && entry.date <= end)
            {
                entriesInRange.Add(entry);
            }
        }
        return entriesInRange;
    }

    public List<Entry> FindEntriesByKeyword(string keyword)
    {
        List<Entry> matchingEntries = new List<Entry>();
        string lowerKeyword = keyword.ToLower(); // convert keyword to lowercase
        foreach (Entry entry in Entries)
        {
            if (entry.prompt.ToLower().Contains(lowerKeyword) ||
                entry.response.ToLower().Contains(lowerKeyword))
            {
                matchingEntries.Add(entry);
            }
        }
        return matchingEntries;
    }
}


class Entry
{
    [JsonPropertyName("prompt")]
    private string _prompt;

    [JsonPropertyName("response")]
    private string _response;

    [JsonPropertyName("date")]
    private DateTime _date;

    public string prompt
    {
        get { return _prompt; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Prompt cannot be null or empty.");
            _prompt = value;
        }
    }

    public string response
    {
        get { return _response; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Response cannot be null or empty.");
            _response = value;
        }
    }

    public DateTime date
    {
        get { return _date; }
        set { _date = value; }
    }

    public Entry()
    {
        // Default constructor for deserialization
    }

    public Entry(string prompt, string response)
    {
        this.prompt = prompt;
        this.response = response;
        _date = DateTime.Now;
    }

    public Entry(string prompt, string response, DateTime date)
    {
        this.prompt = prompt;
        this.response = response;
        _date = date;
    }
}
