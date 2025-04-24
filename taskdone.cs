// TODO ADD HOW LONG THE TASK TOOK TO COMPLETE

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
    public string Check(string taskName, DateTime currentDate, string filePath){        

        string layoutFilePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), "tasksLayout.txt"); // goes two directories up
        LayoutTask foundTask = Find(taskName, filePath);

        if(foundTask.DateAdded == "" || foundTask.DateAdded == null || foundTask.DateAdded.Replace(" ", "") == ""){
            return "not-found"; // taskname not found on existent tasks 
        }

        if(foundTask.Name != null || foundTask.Name != "" || foundTask.Name.Replace(" ","") != ""){
            int status = UpdateLayout(foundTask, layoutFilePath);
            Delete(foundTask.Name, filePath);
            
            string dateAdded = foundTask.DateAdded ;
            string dateDifference = (currentDate - DateTime.Parse(dateAdded)).ToString(@"h\:m\:s");
            string[] separatedTimes = dateDifference.Split(':');

            string timeValue = "";
            for (int j = 0; j < separatedTimes.Length; j++) {
                string time = separatedTimes[j];

                if(j == 0 && time != "0"){
                    if(time == "1"){
                        timeValue = $"{time} hora y {separatedTimes[1]}";
                        if(separatedTimes[1] == "1") {
                            timeValue = timeValue + " minuto.";
                        } else {
                            timeValue = timeValue + " minutos.";
                        }
                        break;
                    } else {
                        timeValue = $"{time} horas y {separatedTimes[1]}";
                        if(separatedTimes[1] == "1") {
                            timeValue = timeValue + " minuto.";
                        } else {
                            timeValue = timeValue + " minutos.";
                        }
                        break;
                    }
                           
                } 
                else if (j == 1 && time != "0") {
                    if(time == "1"){
                        timeValue = $"{time} minuto y {separatedTimes[2]}";
                        if(separatedTimes[2] == "1") {
                            timeValue = timeValue + " segundo.";
                        } else {
                            timeValue = timeValue + " segundos.";
                        }
                        break;
                    } else {
                        timeValue = $"{time} minutos y {separatedTimes[2]}";
                        if(separatedTimes[2] == "1") {
                            timeValue = timeValue + " segundo.";
                        } else {
                            timeValue = timeValue + " segundos.";
                        }
                        break;
                    }
                           
                }
                 else if (j == 2) {
                    if(time == "1") {
                         timeValue = time + " segundo.";
                         break;
                    } else {
                        timeValue = time + " segundos.";
                        break;
                       }
                }
            }                
            string completedTimeAgo = timeValue;
            return completedTimeAgo; // status returns 10 if it is all ok 
        }

        return "";

    }

    public string TaskToText(Task newTask, string filePath)
    {
        if(newTask.TaskName == null){
            return "";
        }
        else{
            string taskData = $"\nTaskName: {newTask.TaskName}\nTaskCreator: {newTask.TaskCreator}\nTaskDateAdded: {newTask.TaskDateAdded}";
            return taskData;    
        }
        
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

    
    public bool Delete(string taskName, string filePath) {

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

    public LayoutTask Find(string taskName, string filePath) {
        string[] lines = File.ReadAllLines(filePath);

        LayoutTask readedTask = new LayoutTask();

        if (lines.Length <= 5)
        {
            return readedTask;
        }

        for (int i = 5; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.StartsWith("TaskName:") && line.Substring("TaskName:".Length).Trim() == taskName || line.Substring("TaskName:".Length).Trim().Replace(" ", "") == taskName)
            {
                     readedTask.Name = line.Substring("TaskName:".Length).Trim();
                     readedTask.Creator = lines[i+1].Substring("TaskCreator:".Length).Trim();
                     readedTask.DateAdded = lines[i+2].Substring("TaskDateAdded:".Length).Trim();

                    TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
                    DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
                    string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");
                    readedTask.DateCompleted = formattedDate;
                    break;
            }

                   
            }
    
            return readedTask;

    }

    public TaskLayoutContainer ReadLayout (string filePath) {

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

    public string TaskLayoutToText(LayoutTask newTask) {
        string taskText = $"\nTaskName: {newTask.Name}\nCreator: {newTask.Creator}\nDateAdded: {newTask.DateAdded}\nDateCompleted: {newTask.DateCompleted}";
        return taskText;
    }

    public int UpdateLayout (LayoutTask newCompletedTask, string filePath) {

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
    private object args;

    public bool Execute()
    {
        string currentDir = Directory.GetCurrentDirectory();
        string tasksDir = @$"{currentDir}\Tasks\DB";

        //GET USER INFO FROM MESSAGE
        string userId;
        CPH.TryGetArg<string>("userId", out userId);
        string username;
        CPH.TryGetArg<string>("userName", out username);
        bool isSubscribed;
        CPH.TryGetArg<bool>("isSubscribed", out isSubscribed);
        bool isMod;
        CPH.TryGetArg<bool>("isModerator", out isMod);
        bool isVip;
        CPH.TryGetArg<bool>("isVip", out isVip);
        string taskName;
        CPH.TryGetArg<string>("rawInput", out taskName);


        bool userIsOp = isSubscribed || isVip || isMod ? true : false;

        TimeZoneInfo buenosAiresTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
        DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, buenosAiresTimeZone);
        string formattedDate = currentDate.ToString("dd/MM/yyyy HH:mm:ss");

        //USER
        string userFilePath = Path.Combine(tasksDir, $"{userId}.txt");

        User user = new User(username, userId, userIsOp, formattedDate, formattedDate);

        UserFile userFile = new UserFile();

        UserTasks userTasks = new UserTasks();

        string completedTimeAgo = userTasks.Check(taskName, currentDate, userFilePath);


        string message = "";



        if(completedTimeAgo != "" && completedTimeAgo != "not-found") {
            message = $"✅ Completaste la tarea '{taskName}' en {completedTimeAgo}";
            CPH.LogInfo("La tarea se agrego al layout y se borro de la db del usuario");
            CPH.SendMessage(message, false, false);

        } 
        else if (completedTimeAgo == "not-found") {
                message = $"No tenés una tarea con el nombre '{taskName}'";
                CPH.SendMessage(message, false, false);

        }
        else if (completedTimeAgo == "") {
                message = "Ocurrio un error inesperado al completar la tarea. Contactá con el streamer.";
                CPH.LogInfo("Ocurrio un error inesperado al agregar la tarea al layout");
                CPH.SendMessage(message, false, false);
        }

        return true;
    }
}
