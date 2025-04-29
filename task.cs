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

public class CreatedTask {
    public string Name;
    public string Creator;
    public string DateAdded;
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
    public int Check(User user, Task task, string filePath) {

        if (File.Exists(filePath)) {
            Task[] readedTasks = Read(filePath);

            if (readedTasks.Length == 5) {
                return 1;
            }
            if (readedTasks.Length >= 1 && !user.IsOp) {
                return 2;
            }

            foreach(Task t in readedTasks) {
                if(t.TaskName == task.TaskName) {
                    return 4;
                }
            }

            string layoutFilePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), "createdTasks.txt"); // goes two directories up and in to tasksLayout.txt
            CreatedTask newLayoutTask = new CreatedTask(); //parse Task to CreatedTask
            newLayoutTask.Name = task.TaskName;
            newLayoutTask.Creator = task.TaskCreator;
            newLayoutTask.DateAdded = task.TaskDateAdded;


            Create(task, filePath);
            int status = UpdateLayout(newLayoutTask, layoutFilePath); // take that parsed CreatedTask and add it to the updated tasks in layout

            return 10;
            
        }
        else return 3;
    }

    public void Create(Task newTask, string filePath) {

        string taskData = $"TaskName: {newTask.TaskName}\nTaskCreator: {newTask.TaskCreator}\nTaskDateAdded: {newTask.TaskDateAdded}";

        File.AppendAllText(filePath, "\n" + taskData);

    }

    public Task[] Read(string filePath) {

        string[] lines = File.ReadAllLines(filePath);

        List<Task> allTasks = new List<Task>();

        Task readedTask = new Task();

        if (lines.Length <= 5)
        {
            return allTasks.ToArray();
        }


        int taskLineCounter = 0;
        for (int i = 5; i < lines.Length; i++) {

            string line = lines[i];

            if (line.StartsWith("TaskName:"))
            {
                readedTask.TaskName = line.Substring("TaskName:".Length).Trim();
                taskLineCounter ++;
            }
            else if (line.StartsWith("TaskCreator:"))
            {
                readedTask.TaskCreator = lines[0].Substring("UserName:".Length).Trim();
                taskLineCounter ++;
            }
            else if (line.StartsWith("TaskDateAdded:"))
            {
                readedTask.TaskDateAdded = line.Substring("TaskDateAdded:".Length).Trim();
                taskLineCounter ++;
            }

            if (taskLineCounter == 3)
            {
                allTasks.Add(readedTask);
                readedTask = new Task();
                taskLineCounter = 0;
            }

        }
        
        return allTasks.ToArray();
    }

    public CreatedTask[] ReadLayout (string filePath) {

        string[] lines = File.ReadAllLines(filePath);

        List<CreatedTask> allCreatedTasks = new List<CreatedTask>();
        CreatedTask eachTask = new CreatedTask();

        int createdTasksNumber = int.Parse(lines[0].Substring("CreatedTasks:".Length).Trim());

        for(int i = 1; i <= createdTasksNumber * 3; i = i + 3){ //queued tasks forloop

            eachTask.Name = lines[i].Substring("TaskName:".Length).Trim();
            eachTask.Creator = lines[i+1].Substring("Creator:".Length).Trim();
            eachTask.DateAdded = lines[i+2].Substring("DateAdded:".Length).Trim();

            allCreatedTasks.Add(eachTask);
            eachTask = new CreatedTask();
        }
        CreatedTask[] allTasksToArray = allCreatedTasks.ToArray();
        return allTasksToArray;
    }
    

    public string TaskLayoutToText(CreatedTask newTask) {
        string taskText = $"\nTaskName: {newTask.Name}\nCreator: {newTask.Creator}\nDateAdded: {newTask.DateAdded}";
        return taskText;
    }

    public int UpdateLayout (CreatedTask newCompletedTask, string filePath) {

        // OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT
        CreatedTask[] allCreatedTasks = ReadLayout(filePath);

        string createdTasksToText = "";

        foreach(CreatedTask createdTask in allCreatedTasks) {
            createdTasksToText = createdTasksToText + TaskLayoutToText(createdTask);
        }
        // OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT

        string createdTasksTitle = "CreatedTasks: ";

        int createdTasksNumber = allCreatedTasks.Length;


        string newTaskToText = TaskLayoutToText(newCompletedTask);
        createdTasksNumber = createdTasksNumber + 1;

        File.WriteAllText(filePath, createdTasksTitle + createdTasksNumber.ToString() + newTaskToText + createdTasksToText );

        return 10; //all good status code
    }


}

public class Source {
    public void SourceGenerator() {

    }
    public void ReadFirstPart(){
        
    }
}

public class CPHInline {
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
        string taskName = (string)args["rawInput"];
        string msgId = (string) args["msgId"];

        Task task = new Task();
        UserTasks userTasks = new UserTasks();

        task.TaskCreator = username;
        task.TaskName = taskName;
        task.TaskDateAdded = formattedDate;


        int status = userTasks.Check(user, task, userFilePath);


        string message = "";
        if(status == 10){
            message = $"{username} comenzó una nueva tarea 📝: '{taskName}'";
            CPH.TwitchAnnounce(message, false, "red", false);
        } else if (status == 1) {
            message= "Usaste todas tus tareas disponibles! Intentá borrar/completar algunas.";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        } else if (status == 2) {
            message= "Sólo podes tener una tarea a la vez. Subeate para tener hasta 5!";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        } else if (status == 3) {
            message = "Ocurrió un error inesperado al crear la tarea.";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
            CPH.LogInfo("user file does not exist, try debugging bitch.");
        } else if (status == 4) {
            message = "Ya tenés una tarea con ese nombre. Probá con otro nombre o borrá la tarea existente.";
            CPH.TwitchReplyToMessage(message, msgId, false, false);
        }


        return true;
    }
}
