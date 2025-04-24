// TODO SHOULD DELETE ALL TASKS THAT HAVE MORE THAN 24HS SINCE THEY HAVE BEEN CREATED. IT'S GOING TO GO TROUGH ALL THE TASKS IN THE DB


using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

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

public class UserTasks {

    public bool Deleter(bool isMod, string directory) {

        if(isMod){

            string[] allFiles = Directory.GetFiles(directory);

            foreach(string userFile in allFiles ){
                CleanAllTasks(userFile);
            }

            CleanAllLayouts(directory);

            return true;
        }
        else return false;

    }

  
    public void CleanAllTasks(string filePath) {

        string[] allLines = File.ReadAllLines(filePath);

        List<string> linesList = [..allLines];

        linesList.RemoveRange(5, linesList.Count - 5);
        string[] clearedLines = linesList.ToArray();

        string linesToText = "";

        int i = 0;
        foreach(string line in clearedLines){
            if(i == 0){
                linesToText = linesToText + $"{line}";
                i++;
            } else {
                linesToText = linesToText + $"\n{line}";
                i++;
            }
        }

        File.Delete(filePath);
        File.WriteAllText(filePath, linesToText);
    }

    public void CleanAllLayouts(string filePath) {

        string createdTasksDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), "createdTasks.txt");
        string completedTasksDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), "completedTasks.txt");

        string createdTasksText = "CreatedTasks: 0";
        string completedTasksText = "CompletedTasks: 0";

        File.Delete(createdTasksDir);
        File.Delete(completedTasksDir);

        File.WriteAllText(createdTasksDir, createdTasksText);
        File.WriteAllText(completedTasksDir, completedTasksText);
    }
}

public class CPHInline {
    public bool Execute()
    {
        string currentDir = Directory.GetCurrentDirectory();
        string tasksDir = @$"{currentDir}\Tasks\DB\";

        bool isMod = (bool)args["isModerator"];

        if(!isMod) {
            return true;
        }

        string msgId = (string) args["msgId"];

        UserTasks userTasks = new UserTasks();

        bool status = userTasks.Deleter(isMod, tasksDir);

        if(status == true) {
            CPH.TwitchReplyToMessage("Limpiaste todas las tareas!", msgId, false, false);

        } else {
            CPH.LogInfo("Sin privilegios");

        }

        return true;
    }
}