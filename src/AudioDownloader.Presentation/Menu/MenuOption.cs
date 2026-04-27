namespace AudioDownloader.Presentation.Menu;

public class MenuOption
{
    public int Key { get; }
    public string Description { get; }
    public Func<Task> Action { get; }

    public MenuOption(int key, string description, Func<Task> action)
    {
        Key = key;
        Description = description;
        Action = action;
    }
}