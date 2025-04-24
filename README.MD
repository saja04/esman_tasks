###### WARNING🔴
These scripts are still in development and they should not be used until their final version is complete.
These scripts are not meant to be runned with .NET runtime any under circumbstances. The only reason why there's a .csproj is because of the VSC local linting, nothing more. Again, DO NOT TRY TO RUN THE CODE (run in Streamer.bot).
## Esman Tasks
Esman Tasks is for those Study Streamers who want to give a more interactive experience for all their viewers inside their Twitch streamings.
##### What do Esman Tasks do?
Esman Tasks gives your twitch viewers the possibility to create personal study/work tasks for them to achieve their goals and mark up their progress along yours, in an interactive way. You can show the tasks progress in your stream, for everyone to interact with the other's progress and make sense of progression among them.

##### How it works?
The first time one of your viewers uses a task command, this is going to create a .txt file with their twitch id as a name, where all their information is going to be stored. You can check this
##### "Installation"
(this installation guide assumes you already have Streamer.bot downloaded and have made some basic commands with it, after configuring it for you channel and your stream app).
Here you do not install anything. The only thing you need to do is create actions for each script, let me tell you how:
- First let's create an action for each command. You can name them as the command that they are going to execute (eg. !taskdone command action could be name as the command itself: !taskdone).
- These actions triggers are going to be Core Commands (Command Triggered), the command to use is up to you, but I recommend you use the same as each script name, with the "!" on the beggining (eg. the taskdone script command could be just !taskdone).
- What the actions are going to do is always execute the script (the code inside each script in this repository) with the C# code compiler inside Streamer.bot. To do this, go to the Sub-Action section, right click -> Core -> C# ->Execute C# Code. A window is going to pop up showing a template C# function, so just delete everything inside of it and paste the whole code for that command of the selected script from this repository.
- Make sure that you save each script inside the Sub-Action by clicking "Save and Compile" bellow after pasting the code. Also you want to go to Settings and check the "Precompile on Application Start" option.
%% -    TODO ADD CONFIG STEPS FOR THE SCENE  CHANGES %%  
After you created all the actions for each script, it is crucial that you run the startercommand just once and then disable it. This script will create all the necessary files and directories for the scripts to run (this is to not having to check it every time a script is runned).
Just repeat the process for all the necessary scripts and test it to make sure everything works just fine and let your viewers know of your new implementation!
  

#### To Do
- [X] delete task description, datecompleted and completed from all scripts.
- [X] task names should be able to contain spaces.
- [X] !taskdone should show how long that task took to complete, instead of the DateAdded and DateCompleted.
- [X] Make the layout in 2 files, one with the completed tasks and the other one with the created tasks. They will not have a limit, but the createdTasks file must delete the first found task once someone completes it.
- [X] !task script should add the task to the TasksInQueue tasklists inside tasksLayout.txt.
- [X] Create the taskdeleter, which should erase the tasks in the layout and all users tasks. This script should execute every 24 hours or at will through a !tasksclean (only for mods and broadcaster). 
- [ ] Make a script that creates all necessary files and directories for the scripts to run perfectly. //CURRENT
- [ ] Make the scene for the layout to show.
<!-- - [ ] Add focus command (viewer can send !startfocus to save the focusTimeStarted and then !stopfocus to return how much he was on focus [returns currentDate - focusTimeStarted]). This could have a counter for the top-time-focus viewers. -->