// TODO THIS SHOULD BE ABLE TO BE CALLED BY ANYONE TO CHECK ANYONE'S TASK INFO OR ITS OWN, IF NO INDEX0 SHOULD CHECK IT'S OWN, IF NOT SHOULD CHECK THAT USER'S TASK
// TODO ALSO IT SHOULD SAVE EACH USERS TASKS BY ID AND NOT BY NAME, CUZ PEOPLE MIGHT CHANGE IT'S NAME



using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class User {
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

public class Task {
    public string TaskName;
    public string TaskCreator;
    public string TaskDateAdded;
}

public class UserFile {
    public void Check(User user, string filePath) {
        if (!File.Exists(filePath))
        {
            CreateFile(user, filePath, null);
        }
        else
        {
            UpdateData(user, filePath);
        }
    }

    public void UpdateData(User newUser, string filePath) {

        User oldUser = ReadUser(filePath);
        string tasks = ReadTasks(filePath);

        newUser.DateAdded = oldUser.DateAdded;
        File.Delete(filePath);

        CreateFile(newUser, filePath, tasks);

    }

    public User ReadUser(string filePath) {
        string[] lines = File.ReadAllLines(filePath);

        User user = new User();

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

        return user;
    }

    public string ReadTasks(string filePath) {

        string[] lines = File.ReadAllLines(filePath);
        string tasks = "";

        foreach (string line in lines)
        {

            if (line.StartsWith("UserName:"))
            {
                continue;
            }
            else if (line.StartsWith("UserId:"))
            {
                continue;
            }
            else if (line.StartsWith("IsOp:"))
            {
                continue;
            }
            else if (line.StartsWith("DateAdded:"))
            {
                continue;
            }
            else if (line.StartsWith("LastUpdated:"))
            {
                continue;
            }
            else
            {
                tasks = tasks + "\n" + line;
            }
        }

        return tasks;
    }

    public void CreateFile(User user, string filePath, string? tasks) {
        string userData = $"UserName: {user.UserName}\nUserId: {user.UserId}\nIsOp: {user.IsOp}\nDateAdded: {user.DateAdded}\nLastUpdated: {user.LastUpdated}";

        File.WriteAllText(filePath, userData + tasks);
    }

}

public class UserTasks {
    public string[] Check(string filePath) {

        Task[] foundTasks = FindAllTasks(filePath);

        List<string> taskNames = new List<string>();

        if(foundTasks.Length == 0) {
            return taskNames.ToArray();
        } 
        else {
            foreach(Task task in foundTasks) {
                taskNames.Add(task.TaskName);
            }
            return taskNames.ToArray();
        } 
    }

    public Task[] FindAllTasks(string filePath) {

        string[] lines = File.ReadAllLines(filePath);

        int tasksNumber = (lines.Length - 5) / 3;

        List<Task> allTasks = new List<Task>();
        Task eachTask = new Task();

        if (lines.Length <= 5)
        {
            return allTasks.ToArray();
        }

        for(int i = 5; i < lines.Length; i = i + 3){ 
            eachTask.TaskName = lines[i].Substring("TaskName:".Length).Trim();
            eachTask.TaskCreator = lines[i+1].Substring("Creator:".Length).Trim();
            eachTask.TaskDateAdded = lines[i+2].Substring("DateAdded:".Length).Trim();

            allTasks.Add(eachTask);
            eachTask = new Task();
        }

        return allTasks.ToArray();
    }

}

public class CPHInline
{
    public bool Execute() {
        string currentDir = Directory.GetCurrentDirectory();
        string tasksDir = @$"{currentDir}\Tasks\DB";

        //GET USER INFO FROM MESSAGE
        string userId = (string)args["userId"];
        string username = (string)args["user"];
        bool isSubscribed = (bool)args["isSubscribed"];
        bool isMod = (bool)args["isModerator"];
        bool isVip = (bool)args["isVip"];
        string taskName = (string)args["rawInput"];

        bool userIsOp = isSubscribed || isVip || isMod ? true : false;

        TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
        DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
        string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");

        //USER
        string userFilePath = Path.Combine(tasksDir, $"{userId}.txt");

        User user = new User(username, userId, userIsOp, formattedDate, formattedDate); //creates user inside context
        UserFile userFile = new UserFile();

        userFile.Check(user, userFilePath); // updates/creates user file


        // TASKSEARCH
        UserTasks userTasks = new UserTasks();
        string[] taskNames = userTasks.Check(userFilePath); // calls to check if any task matches taskname and returns it

        string message = "";

        if(taskNames.Length > 0){
            message = "Estas son tus tareas activas: ";

            for(int i = 0; i < taskNames.Length; i++){
                message = message + $"  ━━━━━ Tarea {(i+1).ToString()}: '{taskNames[i]}' ";
            }
        }
        else {
            message = "no tienes tareas";
        }

        string msgId = (string)args["msgId"];
        CPH.TwitchReplyToMessage(message, msgId, false, false);

        return true;
    }
}
