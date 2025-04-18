// TODO ADD HOW LONG THE TASK TOOK TO COMPLETE

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

public class LayoutTask {
    public string Name;
    public string Creator;
    public string DateCompleted;
    public string DateAdded;
}

public class TaskLayoutContainer {
    public LayoutTask[] QueuedTasks {get; set; }
    public LayoutTask[] CompletedTasks {get; set; }

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
    public int Check(string taskName, string filePath){        

        string layoutFilePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), "tasksLayout.txt"); // goes two directories up
        LayoutTask foundTask = Find(taskName, filePath);

        if(foundTask.DateAdded == "" || foundTask.DateAdded == null || foundTask.DateAdded.Replace(" ", "") == ""){
            return 2; // taskname not found on existent tasks
        }

        if(foundTask.Name != null || foundTask.Name != "" || foundTask.Name.Replace(" ","") != ""){
            int status = UpdateLayout(foundTask, layoutFilePath);
            Delete(foundTask.Name, filePath);
            return status; // status returns 10 if it is all ok 
        } else return 1; // 1 if there's no found task with that name

    }



    public string TaskToText(Task newTask, string filePath)
    {
        if(newTask.TaskName == null){
            return "";
        }
        else{
            string taskData = $"\nTaskName: {newTask.TaskName}\nTaskCompleted: {newTask.TaskCompleted}\nTaskCreator: {newTask.TaskCreator}\nTaskDateAdded: {newTask.TaskDateAdded}\nTaskDateCompleted: {newTask.TaskDateCompleted}\nTaskDescription: {newTask.TaskDescription}";
            return taskData;    
        }
        
    }

    public Task[] Read(string filePath)
    {

        string[] lines = File.ReadAllLines(filePath);

        List<Task> allTasks = new List<Task>();

        Task readedTask = new Task();

        if (lines.Length < 6)
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

    
    public bool Delete(string taskName, string filePath)
    {

        string[] lines = File.ReadAllLines(filePath);

        UserFile newUserFile = new UserFile();

        User user = newUserFile.ReadUser(filePath);

        Task[] readedTasks = Read(filePath);

        List<Task> remainingTasks = new List<Task>();

        foreach (Task t in readedTasks)
        {
            if (t.TaskName == taskName) {
                continue;
            } 
            else {
                remainingTasks.Add(t);
            }
        }
        Task[] remainingTasksArray = remainingTasks.ToArray();
        if(readedTasks.Length == remainingTasksArray.Length) {
            return false;
        } 
        else {
            string allTasksToText = "";
            foreach(Task t in remainingTasks) {
                allTasksToText = allTasksToText + TaskToText(t, filePath);
            }
            newUserFile.CreateFile(user, filePath, allTasksToText);
            return true;
        }

    }

    public LayoutTask Find(string taskName, string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        LayoutTask readedTask = new LayoutTask();

        if (lines.Length <= 5)
        {
            return readedTask;
        }

        bool foundTask = false;

        for (int i = 5; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.StartsWith("TaskName:") && line.Substring("TaskName:".Length).Trim() == taskName || line.Substring("TaskName:".Length).Trim().Replace(" ", "") == taskName)
            {
                     readedTask.Name = line.Substring("TaskName:".Length).Trim();
                     readedTask.Creator = lines[i+2].Substring("TaskCreator:".Length).Trim();
                     readedTask.DateAdded = lines[i+3].Substring("TaskDateAdded:".Length).Trim();

                    TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
                    DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
                    string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");
                    readedTask.DateCompleted = formattedDate;
                    break;
            }

                   
            }
    
            return readedTask;

    }

    public TaskLayoutContainer ReadLayout (string filePath){

        string[] lines = File.ReadAllLines(filePath);

        List<LayoutTask> queuedTasks = new List<LayoutTask>();
        List<LayoutTask> completedTasks = new List<LayoutTask>();

        LayoutTask layoutTask = new LayoutTask();

        int  queuedTasksNumber = 0;
        int  completedTasksNumber = 0;

        foreach(string line in lines) {
            if(line.StartsWith("TasksInQueue:")) {
                queuedTasksNumber = int.Parse(line.Substring("TasksInQueue:".Length).Trim());
            } else if (line.StartsWith("CompetedTasks:")) {
                completedTasksNumber = int.Parse(line.Substring("CompetedTasks:".Length).Trim());
            }
        }

        for(int i = 1; i <= queuedTasksNumber * 4; i = i + 4){ //queued tasks forloop
            string line = lines[i];

            if(line.StartsWith("CompetedTasks:")){
              break;
            }
            else {
                layoutTask.Name = lines[i].Substring("TaskName:".Length).Trim();
                layoutTask.Creator = lines[i+1].Substring("Creator:".Length).Trim();
                layoutTask.DateAdded = lines[i+2].Substring("DateAdded:".Length).Trim();
                layoutTask.DateCompleted = lines[i+3].Substring("DateCompleted:".Length).Trim();

                queuedTasks.Add(layoutTask);
                layoutTask = new LayoutTask();

            }
        }
        for(int i = queuedTasksNumber * 4 + 2; i < lines.Length; i = i + 4) {
                string line = lines[i];

                layoutTask.Name = lines[i].Substring("TaskName:".Length).Trim();
                layoutTask.Creator = lines[i+1].Substring("Creator:".Length).Trim();
                layoutTask.DateAdded = lines[i+2].Substring("DateAdded:".Length).Trim();
                layoutTask.DateCompleted = lines[i+3].Substring("DateCompleted:".Length).Trim();

                completedTasks.Add(layoutTask);
                layoutTask = new LayoutTask();
        }

        TaskLayoutContainer allTasks = new TaskLayoutContainer();

        allTasks.QueuedTasks = queuedTasks.ToArray();
        allTasks.CompletedTasks = completedTasks.ToArray();

        return allTasks;
    }

    public string TaskLayoutToText(LayoutTask newTask){
        string taskText = $"\nTaskName: {newTask.Name}\nCreator: {newTask.Creator}\nDateAdded: {newTask.DateAdded}\nDateCompleted: {newTask.DateCompleted}";
        return taskText;
    }

    public int UpdateLayout (LayoutTask newCompletedTask, string filePath){

        // OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT
        TaskLayoutContainer oldTasks = ReadLayout(filePath);
        LayoutTask[] oldQueuedTasks = oldTasks.QueuedTasks;
        LayoutTask[] oldCompletedTasks = oldTasks.CompletedTasks; 

        if(oldCompletedTasks.Length == 5) { // if there are already 5 completed tasks, delete the last one to make room for the new one
            LayoutTask[] newArr = new LayoutTask[oldCompletedTasks.Length - 1];
            Array.Copy(oldCompletedTasks, newArr, oldCompletedTasks.Length - 1);
            oldCompletedTasks = newArr;
        }

        string queuedTasksToText = "";
        string completedTasksToText = "";

        foreach(LayoutTask queuedtask in oldQueuedTasks) {
            queuedTasksToText = queuedTasksToText + TaskLayoutToText(queuedtask);
        }
        foreach(LayoutTask completedtask in oldCompletedTasks) {
            completedTasksToText = completedTasksToText + TaskLayoutToText(completedtask);
        }
        // OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT OLD TASKS TO TEXT

        string queuedTitle = "TasksInQueue: ";
        string completedTitle = "CompetedTasks: ";

        int queuedTasksNumber = oldQueuedTasks.Length;
        int completedTasksNumber = oldCompletedTasks.Length;   


        string completedTaskToText = TaskLayoutToText(newCompletedTask);
        completedTasksNumber = completedTasksNumber + 1;

        File.WriteAllText(filePath, queuedTitle + queuedTasksNumber.ToString() + queuedTasksToText + "\n" + completedTitle + completedTasksNumber.ToString() + completedTaskToText + completedTasksToText );

        return 10; //all good status code
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
        string taskName = (string)args["input0"];


        bool userIsOp = isSubscribed || isVip || isMod ? true : false;

        TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
        DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
        string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");

        //USER
        string userFilePath = Path.Combine(tasksDir, $"{userId}.txt");

        User user = new User(username, userId, userIsOp, formattedDate, formattedDate);

        UserFile userFile = new UserFile();

        UserTasks userTasks = new UserTasks();

        int status = userTasks.Check(taskName, userFilePath);


        string message = "";

        if(status == 10) {
            message = "Tarea completada!";
            CPH.LogInfo("La tarea se agrego al layout y se borro de la db del usuario");
            CPH.SendMessage(message, false, false);

        } 
        else if (status == 2) {
                message = "No existe una tarea con ese nombre";
                CPH.LogInfo("Tarea no encontrada");
                CPH.SendMessage(message, false, false);

        }
        else if (status == 1) {
                message = "Ocurrio un error inesperado al agregar la tarea al layout";
                CPH.LogInfo("Ocurrio un error inesperado al agregar la tarea al layout");
                CPH.SendMessage(message, false, false);
        }

        // if(status == true){
        //     message = $"Completaste tu tarea '{taskName}'.";
        //     CPH.TwitchReplyToMessage(message, msgId, false, false);
        // } else if (status == false) {
        //     message= "No tienes una tarea con ese nombre. Usa !taskslist para ver tus tareas.";
        //     CPH.TwitchReplyToMessage(message, msgId, false, false);
        // } 


        return true;
    }
}
