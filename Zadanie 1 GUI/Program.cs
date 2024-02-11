/**************************************************************************************************************
 
    Po spustení aplikácie sa do objektov typu záznam (možnosť využiť vstavané triedy jazyka/.NET alebo definovať vlastné; 
    neodporúčame typ "record") uložia jednotlivé vety zo stránky http://mvi.mechatronika.cool/sites/default/files/berces.html (ignorujte prípadné problémy s kódovaním znakov). 
    Texty sa sťahujú zo stránky dynamicky priamo v aplikácii (pomôcka: pozrite si metódu DownloadString triedy WebClient). Vetou sa chápe text ukončený bodkou.

    Do každého prvku objektu záznam sa uloží veta, jej ID a autor tejto vety. 
    Pri inicializácií aplikácie sa autor viet vyplní automaticky ľubovoľným textom. 
    Prípadne je možné spýtať sa na meno priamo užívateľa.

    Následne sú užívateľovi sprístupnené nasledovné funkcionality:
    a) vypísanie všetkých záznamov
    b) zmazanie záznamu podľa vlastného výberu
    c) editovanie záznamu podľa vlastného výberu, vrátane autora vety
    d) pridanie záznamu

    Aplikácia funguje cyklicky, čiže napríklad po zmazaní záznamu môže užívateľ ďalej pokračovať výberom funkcionalít a) až d).

***************************************************************************************************************/

using System.Net;
using System.Text;

public class Sentence
{
    public int ID { get; private set; }
    public string Author { get; private set; }
    public string Content { get; private set; }

    public Sentence(int id, string author,  string content)
    {
        ID = id;
        Author = author;
        Content = content;
    }

    // Methods for changing private variables
    public void ChangeAuthor(string newAuthor)
    {
        Author = newAuthor;
    }

    public void EditContent(string newContent)
    {
        Content = newContent;
    }
}

class Program
{
    static void Main()
    {
        string URL = "http://mvi.mechatronika.cool/sites/default/files/berces.html";    // Page to retrieve the the text from

        string data = GetData(URL);     // Getting the data

        // Test1(data);    // Data download Test

        string[] sentences = data.Split('.');

        int sentenceId = 1;
        string defaultAuthor = "Unspecified";

        List<Sentence> sentencesList = new List<Sentence>();

        foreach (string text in sentences)  // Make an object for each sentence in downloaded data
        {
            if (!string.IsNullOrWhiteSpace(text))   // Skip empty
            {
                Sentence sentence = new Sentence(sentenceId, defaultAuthor, text.Trim());
                sentencesList.Add(sentence);
                sentenceId++;
            }
        }

        Menu(sentencesList);

        // Test2(sentencesList);   // List save and sentence separation test


        Console.WriteLine("\n\nEnd of Program!\nPress enter to end the program...");
        Console.ReadKey();
    }

    // Menu
    static void Menu(List<Sentence> sentencesList)
    {
        while (true) // Menu loop
        {
            Console.WriteLine("MENU:" +
                              "\np - print a sentence" +
                              "\na - add a sentence" +
                              "\nd - delete the specified sentence" +
                              "\nc - change the author or edit the sentence" +
                              "\ne - EXIT");

            Console.Write("\nEnter your choice: ");
            char choice = Console.ReadKey().KeyChar; // Input prompt
            Console.ReadKey();

            switch (choice)
            {
                case 'p':
                    PrintSentence(sentencesList); // Printing
                    break;

                case 'a':
                    AddSentence(sentencesList); // Adding
                    break;

                case 'd':
                    DeleteSentence(sentencesList); // Deleting
                    break;

                case 'c':
                    ChangeOrEditSentence(sentencesList); // Editing
                    break;

                case 'e':
                    Console.WriteLine("\nExiting the program..."); // Exit
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please try again."); // Invalid Input
                    break;
            }
        }
    }


    // Sentence List control methods
    static void PrintSentence(List<Sentence> sentencesList)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("PRINTER:" +
                            "\na - Print all" +
                            "\ns - Specify sentence" +
                            "\nr - Cancel");

            Console.Write("\nEnter your choice: ");

            int choice = Console.ReadKey().KeyChar;     // Input prompt
            Console.ReadKey();

            switch (choice)
            {
                case 'a':
                    Console.Clear();
                    PrintAll(sentencesList);
                    return;
                case 's':
                    Console.Clear();
                    PrintSpecific(sentencesList);
                    return;
                case 'c':
                    Console.Clear();
                    return;

                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    break;

            }
        }
    }

    static void AddSentence(List<Sentence> sentencesList)
    {
        Console.Clear();
        while (true)
        {
            Console.Write("\nEnter the ID for the new sentence (or 'c' to cancel): ");
            string input = Console.ReadLine();

            if (input.ToLower() == "c")
            {
                Console.WriteLine("Operation canceled.");
                Console.Clear();
                return;
            }

            if (int.TryParse(input, out int newID))
            {
                if (sentencesList.Any(s => s.ID == newID))
                {
                    Console.WriteLine($"Sentence with ID {newID} already exists. Please choose another ID.");
                }
                else
                {
                    Console.Write("Enter the content for the new sentence: ");
                    string content = Console.ReadLine();

                    Console.Write("Enter the author for the new sentence: ");
                    string author = Console.ReadLine();

                    Sentence newSentence = new Sentence(newID, author, content);

                    // Display details to the user
                    Console.WriteLine($"\nNew Sentence Details:\nID: {newSentence.ID}\nAuthor: {newSentence.Author}\nSentence: {newSentence.Content}");

                    Console.Write("Enter 's' to save or 'c' to cancel: ");
                    string saveChoice = Console.ReadLine().ToLower();

                    if (saveChoice == "s")
                    {
                        sentencesList.Add(newSentence);
                        Console.Clear();
                        Console.WriteLine($"New sentence added with ID {newID}");
                        return;
                    }
                    else if (saveChoice == "c")
                    {
                        Console.WriteLine("Operation canceled.");
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 's' to save or 'c' to cancel.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid input for ID. Please enter a valid integer or 'c' to cancel.");
            }
        }
    }


    static void DeleteSentence(List<Sentence> sentencesList)
    {
        Console.Write("\nEnter the ID of the sentence to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            // Find the sentence with the specified ID
            Sentence sentenceToDelete = sentencesList.FirstOrDefault(s => s.ID == id);

            if (sentenceToDelete != null)
            {
                // Remove the found sentence from the list
                sentencesList.Remove(sentenceToDelete);
                Console.WriteLine($"Sentence with ID {id} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"No sentence found with ID {id}.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid ID.");
        }
    }


    static void ChangeOrEditSentence(List<Sentence> sentencesList)
    {
        Console.Write("\nEnter the ID of the sentence to edit: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Sentence sentenceToEdit = sentencesList.FirstOrDefault(s => s.ID == id);

            if (sentenceToEdit != null)
            {
                Console.WriteLine($"\nEditing sentence with ID {id}: {sentenceToEdit.Content}");

                Console.WriteLine("Choose an option:");
                Console.WriteLine("a - Change Author");
                Console.WriteLine("e - Edit Sentence");
                Console.WriteLine("c - Cancel");

                Console.Write("Enter your choice: ");
                char choice = Console.ReadKey().KeyChar;

                switch (choice)
                {
                    case 'a':       // Edit Author
                        Console.Write("\nEnter the new author: ");
                        string newAuthor = Console.ReadLine();
                        sentenceToEdit.ChangeAuthor(newAuthor);
                        Console.WriteLine("Author changed successfully!");
                        break;

                    case 'e':       // Edit Sentence
                        Console.Write("\nEnter the new sentence: ");
                        string newSentence = Console.ReadLine();
                        sentenceToEdit.EditContent(newSentence);
                        Console.WriteLine("Sentence edited successfully!");
                        break;

                    case 'c':       // Cancel
                        Console.WriteLine("\nOperation canceled.");
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice. Operation canceled.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Sentence with ID {id} not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid ID.");
        }
    }




    // TESTS
    // Data download Test
    static void Test1(string data)
    {
        Console.WriteLine(data);
        Console.ReadKey();
    }

    // List save and sentence separation test
    static void Test2(List<Sentence> sentencesList)
    {
        foreach (Sentence sentence in sentencesList)
        {
            Console.WriteLine($"\n> ID: {sentence.ID}; {sentence.Content}, Author: \"{sentence.Author}\"");
            Console.ReadKey();
        }
    }


    //Other Methods

    // WEB Data Getter
    static string GetData(string URL)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        WebClient client = new WebClient();  // Initialize the WebClient
        client.Encoding = Encoding.GetEncoding("windows-1250");     // Set Encoding

        return client.DownloadString(URL);     // Download and return content
    }
    
    // Print methods
    static void PrintAll(List<Sentence> sentencesList)
    {
        Console.WriteLine("\nPrinting all sentences:");

        foreach (Sentence sentence in sentencesList)
        {
            Console.WriteLine($"\n> ID: {sentence.ID}, Author: {sentence.Author}\n{sentence.Content}");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }

    static void PrintSpecific(List<Sentence> sentencesList)
    {
        Console.Write("\nEnter the ID of the sentence to print: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Sentence specificSentence = sentencesList.FirstOrDefault(s => s.ID == id);

            if (specificSentence != null)
            {
                Console.WriteLine($"\nPrinting sentence with ID {id}:");
                Console.WriteLine($"> ID: {specificSentence.ID}, Author: {specificSentence.Author}, Sentence: {specificSentence.Content}");
            }
            else
            {
                Console.WriteLine("Sentence not found. Please enter a valid ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid ID.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}
