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

public class UserFile {
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
    public int Check(User user, Task task, string filePath)
    {

        if (File.Exists(filePath))
        {

            Task[] readedTasks = Read(filePath);


            foreach(Task t in readedTasks) {
                if(t.TaskName == task.TaskName) {
                    return 4;
                }
            }

            if (readedTasks.Length == 6)
            {
                return 1;
            }
            else if (readedTasks.Length >= 1 && !user.IsOp)
            {
                return 2;
            }
            else
            {
                Create(task, filePath);
                return 10;
            }
        }
        else return 3;
    }
    public void Create(Task newTask, string filePath)
    {

        string taskData = $"TaskName: {newTask.TaskName}\nTaskCompleted: {newTask.TaskCompleted}\nTaskCreator: {newTask.TaskCreator}\nTaskDateAdded: {newTask.TaskDateAdded}\nTaskDateCompleted: {newTask.TaskDateCompleted}\nTaskDescription: {newTask.TaskDescription}";

        File.AppendAllText(filePath, "\n" + taskData);

    }
    public Task[] Read(string filePath)
    {

        string[] lines = File.ReadAllLines(filePath);

        List<Task> allTasks = new List<Task>();

        Task readedTask = new Task();

        if (lines.Length <= 5)
        {
            return allTasks.ToArray();
        }

        for (int i = 5; i < lines.Length; i++)
        {

            string line = lines[i];

            if (line.StartsWith("TaskName:"))
            {
                readedTask.TaskName = line.Substring("TaskName:".Length).Trim();
            }
            else if (line.StartsWith("TaskCompleted:"))
            {
                readedTask.TaskCompleted = bool.Parse(line.Substring("TaskCompleted:".Length).Trim());
            }
            else if (line.StartsWith("TaskCreator:"))
            {
                readedTask.TaskCreator = lines[0].Substring("UserName:".Length).Trim();
            }
            else if (line.StartsWith("TaskDateAdded:"))
            {
                readedTask.TaskDateAdded = line.Substring("TaskDateAdded:".Length).Trim();
            }
            else if (line.StartsWith("TaskDateCompleted:"))
            {
                readedTask.TaskDateCompleted = line.Substring("TaskDateCompleted:".Length).Trim();
            }
            else if (line.StartsWith("TaskDescription:"))
            {
                readedTask.TaskDescription = line.Substring("TaskDescription:".Length).Trim();
            }


            if ((i - 5) % 6 == 5)
            {
                allTasks.Add(readedTask);
                readedTask = new Task();
            }

        }
        
        return allTasks.ToArray();
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




        // TASKS
        string taskName = (string)args["input0"];
        string taskDescription = (string)args["input1"];
        string msgId = (string) args["msgId"];

        Task task = new Task();
        UserTasks userTasks = new UserTasks();

        task.TaskCreator = username;
        task.TaskName = taskName;
        task.TaskDescription = taskDescription;
        task.TaskCompleted = false;
        task.TaskDateAdded = formattedDate;
        task.TaskDateCompleted = formattedDate;


        int status = userTasks.Check(user, task, userFilePath);

        string message = "";

        if(status == 10){
            message = $"{username} creo una nueva tarea 📝: '{taskName}'";
            CPH.TwitchAnnounce(message, false, "red", false);
        } else if (status == 1) {
            message= "El maximo de tareas para subs es de 5!";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        } else if (status == 2) {
            message= "Solo podes tener una tarea a la vez. Subeate para tener hasta 5!";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        } else if (status == 3) {
            message = "Ocurrio un error inesperado al crear la tarea";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        } else if (status == 4) {
            message = "Ya tenes una tarea con ese nombre. Proba con otro nombre o borra la tarea existente.";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        }


        return true;
    }
}
