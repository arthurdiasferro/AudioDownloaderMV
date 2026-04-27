namespace AudioDownloader.Presentation.Menu;

public class MenuSystem
{
    private readonly List<MenuOption> _options = new();
    private readonly string _title;

    public MenuSystem(string title)
    {
        _title = title;
    }

    public void AddOption(int key, string description, Func<Task> action)
    {
        _options.Add(new MenuOption(key, description, action));
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_title);
            Console.WriteLine(new string('-', _title.Length));

            foreach (var option in _options.OrderBy(o => o.Key))
            {
                Console.WriteLine($"{option.Key} - {option.Description}");
            }

            Console.Write("\nType option: ");

            string? input = null;
            try
            {
                input = Console.ReadLine();
            }
            catch (InvalidOperationException)
            {
                Thread.Sleep(100);
                continue;
            }

            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int selectedOption))
            {
                var option = _options.FirstOrDefault(o => o.Key == selectedOption);
                if (option != null)
                {
                    await option.Action.Invoke();
                    continue;
                }
            }

            ShowInvalidOptionMessage();
        }
    }

    public void Exit()
    {
        Environment.Exit(0);
    }

    private void ShowInvalidOptionMessage()
    {
        Console.Clear();
        Console.Write("Invalid option.");
        Thread.Sleep(TimeSpan.FromSeconds(3));
    }
}