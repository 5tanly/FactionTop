## What?

This is a script designed to display faction top information and hoverinfo specifically made for SaicoPvP Realms, I can not guarantee it works for any other servers.

## Why do I need this?

You're already afking tons of accounts on a realm that you are playing on, why not put them to work? Most mainstream ways players collect faction top information is either screenshotting every reboot and posting it in discord, or laboriously coding a discord bot from scratch that can only utilize one account at a time. This script is super lightweight and works for an unlimited amount of accounts.

## Why should I trust you that this ain't a rat?
I admire your online security concerns, but, the code is open source and you can read through it to see what it does, along with comments to aid you along your way. I assure you this is safe.

## How do I use this?
Follow these steps and you will be well on your way

------------

##### 1. Download the files from the GitHub repo

##### 2. Place the files in the directory where you run MinecraftClient.exe from; i.e. working directory

##### 3. Make sure [ScriptScheduler] is enabled in MinecraftClient.ini

	[ScriptScheduler]
	enabled=true
	tasksfile=tasks.ini

##### 4. Add the following tasks to tasks.ini; if tasks.ini does not exist, create it

  [ScriptScheduler]
  enabled=true
  tasksfile=tasks.ini

  [Task]
  triggerOnTime=true
  timeValue=
  script=factiontop.cs

  [Task]
  triggerOnTime=true
  timeValue=
  script=factiontop.cs

##### 5. Make sure to set the reboot times correctly! I recommend setting the time to 15 minutes after a reboot. i.e. if reboot for your realm is at 16:45 make sure to set the time to 17:00. Time MUST be in 24hrs format

  [Task]
  triggerOnTime=true
  timeValue=17:00
  script=factiontop.cs

##### 6. Create a Discord Webhook for your server. Server Settings > Webhooks > Create Webhook. Set the channel to the desired channel and copy the webhook url

##### 7. Enable and make sure your webhook url is set properly in factiontop.ini. Make sure there are no spaces before or after the equals sign

	[FactionTop]
	enabled=true
	webhook=PASTE_WEBHOOK_URL_HERE

##### 8. If your account isn't in a realm at the time you specified, it will not post faction top data in your discord. If you want to manually do so, enter the realm and type "/script factiontop.cs"
