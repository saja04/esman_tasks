using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

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

public class Task
{
    public string TaskName;
    public bool TaskCompleted;
    public string TaskCreator;
    public string TaskDescription;
    public string TaskDateAdded;
    public string TaskDateCompleted;
}

public class UserFile
{
    public void Check(User user, string filePath)
    {
        if (!File.Exists(filePath))
        {
            CreateFile(user, filePath, null);
        }
        else
        {
            UpdateData(user, filePath);
        }
    }
    public void UpdateData(User newUser, string filePath)
    {

        User oldUser = ReadUser(filePath);
        string tasks = ReadTasks(filePath);

        newUser.DateAdded = oldUser.DateAdded;
        File.Delete(filePath);

        CreateFile(newUser, filePath, tasks);

    }
    public User ReadUser(string filePath)
    {
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

    public string ReadTasks(string filePath)
    {

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

    public void CreateFile(User user, string filePath, string? tasks)
    {
        string userData = $"UserName: {user.UserName}\nUserId: {user.UserId}\nIsOp: {user.IsOp}\nDateAdded: {user.DateAdded}\nLastUpdated: {user.LastUpdated}";

        File.WriteAllText(filePath, userData + tasks);
    }
}

public class UserTasks
{
    public Task Check(string taskName, string filePath)
    {
        Task foundTask = Find(taskName, filePath);

        return foundTask;

    }

    public Task Find(string taskName, string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        Task readedTask = new Task();

        if (lines.Length <= 5)
        {
            return readedTask;
        }

        bool foundTask = false;

        for (int i = 5; i < lines.Length; i++)
        {

            string line = lines[i];

            if (line.StartsWith("TaskName:"))
            {
                readedTask.TaskName = line.Substring("TaskName:".Length).Trim();
                if (readedTask.TaskName == taskName || readedTask.TaskName.Replace(" ", "") == taskName)
                {
                    foundTask = true;
                }
            }

            if (foundTask)
            {
                if (line.StartsWith("TaskName:"))
                {
                    readedTask.TaskName = line.Substring("TaskName:".Length).Trim();
                }
                else if (line.StartsWith("TaskDateAdded:"))
                {
                    readedTask.TaskDateAdded = line.Substring("TaskDateAdded:".Length).Trim();
                }
                else if (line.StartsWith("TaskDescription:"))
                {
                    readedTask.TaskDescription = line.Substring("TaskDescription:".Length).Trim();
                    foundTask = false; //bc it's the end of a task inside the default task structure
                    break;
                }
            }
        }

        if(readedTask.TaskDescription == null ){
            readedTask.TaskName = "notfound";
            return readedTask;
        } else return readedTask;


    }
}

public class CPHInline
{
    public bool Execute()
    {
        string currentDir = Directory.GetCurrentDirectory();
        string tasksDir = @$"{currentDir}\Tasks\DB";

        //GET USER INFO FROM MESSAGE
        string userId = (string)args["userId"];
        string username = (string)args["user"];
        bool isSubscribed = (bool)args["isSubscribed"];
        bool isMod = (bool)args["isModerator"];
        bool isVip = (bool)args["isVip"];

        bool userIsOp = isSubscribed || isVip || isMod ? true : false;

        TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
        DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
        string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");

        //USER
        string userFilePath = Path.Combine(tasksDir, $"{userId}.txt");

        User user = new User(username, userId, userIsOp, formattedDate, formattedDate);



        UserFile userFile = new UserFile();

        userFile.Check(user, userFilePath);




        // TASKSEARCH
        UserTasks userTasks = new UserTasks();

        string taskName = (string)args["input0"];

        Task foundTask = userTasks.Find(taskName, userFilePath);



        string message = "";

        if (foundTask.TaskName != "notfound")
        {
                    string dateDifference = (currentDate - DateTime.Parse(foundTask.TaskDateAdded)).ToString(@"h\:m\:s");
        

        string[] separatedTimes = dateDifference.Split(':');

        string timeValue = "";

        for (int i = 0; i < separatedTimes.Length; i++)
        {
            string time = separatedTimes[i];

            if(i == 0 && time != "0"){
                timeValue = $"{time} horas y {separatedTimes[1]} minutos.";
                break;
            } 
            else if (i == 1 && time != "0") {
                timeValue = $"{time} minutos y {separatedTimes[2]} segundos.";
                break;
            }
            else if (i == 2) {
                timeValue = time + " segundos.";
            }

        }
            message = $"Nombre de la tarea: {foundTask.TaskName}. Descripcion de la tarea: {foundTask.TaskDescription}. La tarea fue creada hace " + timeValue;
        }
        else
        {
            message = "No tenes una tarea con ese nombre. Usa !taskslist para ver todas tus tareas.";
        }



        string msgId = (string)args["msgId"];




        // Send the reply
        CPH.TwitchReplyToMessage(message, msgId, false, false);

        return true;
    }
}
