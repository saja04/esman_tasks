using System;
using System.IO;
using System.Text;

public class User
{
    public string UserName { get; set; }
    public string UserId { get; set; }
    public bool IsOp { get; set; }
    public string DateAdded { get; set; }
    public string LastUpdated { get; set; }
    public User(
        string userName = null,
        string userId = null,
        bool isOp = false,
        string dateAdded = null,
        string lastUpdated = null)
    {
        UserName = userName;
        UserId = userId;
        IsOp = isOp;
        DateAdded = dateAdded;
        LastUpdated = lastUpdated;
    }
}

public class UserDataFile
{
    public void Check(User user, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"User {user.UserId} is not on the db");
            CreateFile(user, filePath);
        }
        else
        {
            UpdateData(user, filePath);
        }
    }
    public void UpdateData(User newUser, string filePath)
    {

        User oldUser = ReadData(filePath);

        newUser.DateAdded = oldUser.DateAdded;
        File.Delete(filePath);

        CreateFile(newUser, filePath);

    }
    public User ReadData(string filePath)
    {
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            return null;
        }

        // Read all lines from the file
        string[] lines = File.ReadAllLines(filePath);

        // Initialize user object
        User user = new User();
        // Loop through each line and extract the data
        foreach (string line in lines)
        {
            if (line.StartsWith("UserName:"))
            {
                user.UserName = line.Substring("UserName:".Length).Trim();
            }
            else if (line.StartsWith("UserId:"))
            {
                user.UserId = line.Substring("UserId:".Length).Trim();
            }
            else if (line.StartsWith("IsOp:"))
            {
                user.IsOp = bool.Parse(line.Substring("IsOp:".Length).Trim());
            }

            else if (line.StartsWith("DateAdded:"))
            {
                user.DateAdded = line.Substring("DateAdded:".Length).Trim();
            }
            else if (line.StartsWith("LastUpdated:"))
            {
                user.LastUpdated = line.Substring("LastUpdated:".Length).Trim();
            }
        }

        // Return the populated User object
        return user;
    }

    public void CreateFile(User user, string filePath)
    {
        string userData = $"UserName: {user.UserName}\nUserId: {user.UserId}\nIsOp: {user.IsOp}\nDateAdded: {user.DateAdded}\nLastUpdated: {user.LastUpdated}";

        File.WriteAllText(filePath, userData);
        Console.WriteLine($"User {user.UserId} has been added/updated");
    }

    // public void UpdateUserFile ();
}

public class UserTasksFile
{
    public void Create() {

    }
}
public class CPHInline
{
    public bool Execute()
    {
        string currentDir = Directory.GetCurrentDirectory();
        string tasksDir = @$"{currentDir}\Tasks\DB";

        string userId = (string)args["userId"];
        string username = args["user"] as string;
        bool isSubscribed = (bool)args["isSubscribed"];
        bool isMod = (bool)args["isModerator"];
        bool isVip = (bool)args["isVip"];

        bool userIsOp = isSubscribed || isVip || isMod ? true : false;

        // Get Buenos Aires time
        TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
        DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
        string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");



        User user = new User(username, userId, userIsOp, formattedDate, formattedDate);

        // create filepath for the user via userid
        string userFilePath = Path.Combine(tasksDir, $"{userId}.txt");

        UserDataFile userFile = new UserDataFile();

        userFile.Check(user, userFilePath);


        // Get the ID of the message you're replying to
        string msgId = (string) args["msgId"];

        string replyText = "Podés crear una tarea para marcar cierto objetivo dentro de tu estudio/trabajo y, una vez que la completes, compartirás tu progreso con el resto!😀 Escribi !taskshelp para ver la lista de comandos";


        // Send the reply
        CPH.TwitchReplyToMessage(replyText, msgId, false, false);

        return true;
    }
}
