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

public class TaskInfo {
    public string TaskName;
    public string TaskCreator;
    public string CompletedTimeAgo;
}

public class CheckReturn {
    public bool Found;
    public TaskInfo Task;
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
    public CheckReturn Check(string taskName, DateTime currentDate, string filePath) {

        TaskInfo foundTask = Find(taskName, currentDate, filePath);

        CheckReturn response = new CheckReturn();

        if(foundTask.TaskName != null) {
            response.Found = true;
            response.Task = foundTask;
        } else {
            response.Found = false;
            response.Task = foundTask;
        } 
        return response;
    }

    public TaskInfo Find(string taskName, DateTime currentDate, string filePath) {

        string[] lines = File.ReadAllLines(filePath);

        TaskInfo readedTask = new TaskInfo();

        if (lines.Length <= 5)
        {
            return readedTask;
        }

        for (int i = 5; i < lines.Length; i++) {
            string line = lines[i];

            if (line.StartsWith("TaskName:")) {
                if (line.Substring("TaskName:".Length).Trim() == taskName || line.Substring("TaskName:".Length).Trim().Replace(" ", "") == taskName) {
                    readedTask.TaskName = line.Substring("TaskName:".Length).Trim();
                    readedTask.TaskCreator = lines[i + 1].Substring("TaskCreator:".Length).Trim();

                    string dateAdded = lines[i + 2].Substring("TaskDateAdded:".Length).Trim();


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
                    readedTask.CompletedTimeAgo = timeValue;
                }
            }
        }
        
        return readedTask;
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
        CheckReturn foundTask = userTasks.Check(taskName, currentDate, userFilePath); // calls to check if any task matches taskname and returns it


        string message = "";

        CPH.LogInfo("taskname: " + foundTask.Task.TaskName);

        if (foundTask.Found) {
            message = $"Nombre de la tarea: '{foundTask.Task?.TaskName}'. Creador/a: {foundTask.Task?.TaskCreator}. Se creó hace " + foundTask.Task?.CompletedTimeAgo;
        }
        else {
            message = "No tenés una tarea con ese nombre. Usá !taskslist para ver todas tus tareas";
        }

        string msgId = (string)args["msgId"];
        CPH.TwitchReplyToMessage(message, msgId, false, false);

        return true;
    }
}
