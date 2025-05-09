###### **WARNING** 🔴

> _These scripts are still in development and they should not be used until their final version is complete._
>
> _These scripts are not meant to be run with .NET runtime under any circumstances. The only reason why there's a .csproj is because of the VSC local linting, nothing more. Again, DO NOT TRY TO RUN THE CODE (run in Streamer.bot)._

## **Esman Tasks**

Esman Tasks is for those Study Streamers who want to give a more interactive experience for all their viewers inside their Twitch streamings.


#### What do Esman Tasks do?

Esman Tasks gives your Twitch viewers the possibility to create personal study/work tasks for them to achieve their goals and mark up their progress along yours, in an interactive way. You can show the tasks' progress in your stream, for everyone to interact with each other's goals and make sense of progression among them.

---

#### How it works?

These "scripts" use only the C# standard library and are **NOT**, under any circumstance, going to send any kind of information to a third party. Everything is runned locally on your machine and does not have access past the [Streamer.bot](https://streamer.bot/) programm/directory.

Once you configure all the commands inside Streamer.bot, the first time one of your viewers uses a task command, the scripts are going to create a `.txt` file with their Twitch ID as the name, where all their information related to tasks is going to be stored. All the tasks they create are going to be stored in that file and two other `.txt` files (one directory up) which are dedicated to show all the tasks' information in the layout/scene. Every time anyone creates/deletes/completes a task it's going to update the browser source that's being displayed in the scene.

---

## **Installation**

(This installation guide assumes you already have [Streamer.bot](https://streamer.bot/) downloaded and have made some basic commands with it, after configuring it for your channel and your stream app).

Here you do not install anything. The only thing you need to do is **create actions for each script**. Let me tell you how:

1. **Create an action for each command**: You can name them as the command that they are going to execute (e.g., `!taskdone` command action could be named as the command itself: `!taskdone`).
   
2. **Action Trigger Setup**: These actions triggers are going to be **Core Commands (Command Triggered)**. The command to use is up to you, but I recommend you use the same as each script name, with the "!" at the beginning (e.g., the `taskdone` script command could be just `!taskdone`).

3. **Execute C# Code**: What the actions are going to do is always execute the script (the code inside each script in this repository) with the C# code compiler inside Streamer.bot. To do this, go to the Sub-Action section, **right-click → Core → C# → Execute C# Code**. A window is going to pop up showing a template C# function, so just delete everything inside of it and **paste the whole code** for that command of the selected script from this repository.

4. **Save and Compile**: Make sure that you save each script inside the Sub-Action by clicking **"Save and Compile"** below after pasting the code. Also, you want to go to **Settings** and check the **"Precompile on Application Start"** option.

> %% - TODO ADD CONFIG STEPS FOR THE SCENE CHANGES %% 

After you created all the actions for each script, it is crucial that you **run the starter command just once** and then disable it. This script will create all the necessary files and directories for the scripts to run (this is to avoid having to check it every time a script is run).

Just repeat the process for all the necessary scripts and test it to make sure everything works just fine and let your viewers know about your new implementation!

---

## **To Do**

- [X] Delete task description, `datecompleted` and `completed` from all scripts.
- [X] Task names should be able to contain spaces.
- [X] `!taskdone` should show how long that task took to complete, instead of the `DateAdded` and `DateCompleted`.
- [X] Make the layout in 2 files, one with the completed tasks and the other one with the created tasks. They will not have a limit, but the `createdTasks` file must delete the first found task once someone completes it.
- [X] `!task` script should add the task to the `TasksInQueue` tasklists inside `tasksLayout.txt`.
- [X] Create the `taskdeleter`, which should erase the tasks in the layout and all users' tasks. This script should execute every 24 hours or at will through a `!tasksclean` (only for mods and broadcaster).
- [X] Make a script that creates all necessary files and directories for the scripts to run perfectly.
- [ ] task names should not contain more than 25 chars.
- [ ] Make the scene for the layout to show. **//CURRENT**
    - [ ] First, make the HTML template.
        - [ ] HTML template should have styles and animations for when tasks get added or completed. This can be done by creating the first task of each group as a custom class `<p>` inside the template (since those are the tasks that are going to be animated).
    - [ ] Then the `task`, `taskdone`, `taskdelete` commands need to create and change the HTML template via file manipulation. It will also update the web source via `"CPH.SlobsSetBrowserSource(scene, source, url);"` method. This means that the commands are going to update the HTML template directly (lots of work I think).
