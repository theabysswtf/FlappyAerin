require('dotenv').config();
const Discord = require('discord.js');
const config = require('./config.json');

const client = new Discord.Client({ intents: ["GUILD_MESSAGES", "DIRECT_MESSAGES", "GUILDS"] });
const active_games = {};

client.on("ready", () => {
    console.log("Logged in as " + client.user.tag + "!");
});

client.on("threadCreate", (thread) => {
    if (thread.name.search(config.THREAD_NAME) > -1)
    {
        thread.send(config.START_MESSAGE);
    }
});

// Defines Game objects
function Game(author, participants, channel) {
    this.thread_name = author.username + "'s " + config.THREAD_NAME;
    this.channel = channel;
    this.participants = participants;

    this.tostring = () => this.participants;
    this.open_thread = () => Promise.resolve(this.channel.threads.create({
        name: this.thread_name,
        autoArchiveDuration: 60,
        reason: config.START_MESSAGE
    }).then((thread) => {
        this.participants.forEach(participant => {
            thread.members.add(participant);
        })
        this.thread = thread;
    }));
    this.send_thread_message = (text) => Promise.resolve(this.thread.send(text));
}

// Pairs each command with it's result
const commandSet = {
    ping : (args, message) => {
        message.reply("PONG! ARGS: " + args);
    },
    create : (args, message) => {   
        // TODO: NEED TO ADD A CHECK FOR WHETHER THE CHARACTER IS A PLAYER IN AN EXISTING GAME
        if (active_games.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + ">'s Already has a game active. Please +remove this game before making a new one!");
        } else {
            if( message.mentions.users.size + 1 < config.MIN_PARTICIPANTS || message.mentions.users.size + 1 > config.MAX_PARTICIPANTS ){
                message.reply("Sorry! This game only supports " + config.MIN_PARTICIPANTS + "-" + config.MAX_PARTICIPANTS + " players : " + message.mentions.users.size);
                return;
            }
            
            let participants = [];
            participants.push(message.author.id);
            message.mentions.users.forEach(user => {
                participants.push(user.id);
            });
            
            let game = new Game(message.author, participants, message.channel);
            let thread_promise = game.open_thread();

            let players = "";
            game.participants.forEach(playerID => {
                players += " <@" + (playerID) + ">"; 
            });
            active_games[message.author] = game;
            message.reply("Created Game with:" + players);
        }
    },
    remove : (args, message) => {
        if (active_games.hasOwnProperty(message.author)) {
            delete active_games[message.author];
            message.reply("<@" + message.author.id + ">'s game has been removed!");
        } else {
            message.reply("<@" + message.author.id + "> had no active game!");
        }
    },
    send : (args, message) => {     // TESTER FUNCTION
        if (active_games.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + "> Sent a message to the thread!");
            active_games[message.author].send_thread_message("Halleilujah");
        }
    }
}

const prefix = "+"
// Controls Input Routing
client.on("messageCreate", async (message) => {
    if (message.author.bot) return; // Do not process bot commands
    if (!message.content.startsWith(prefix)) return;

    const body = message.content.slice(prefix.length);
    const args = body.split(" ");
    const command = args.shift().toLowerCase();

    if (commandSet.hasOwnProperty(command))
    {
        commandFunc = commandSet[command];
        commandFunc(args, message);
    }
    else {
        console.log(Object.keys(commandSet));
        message.reply("args: " + args + " | command: " + command);
    }
    
    /*
    Alrighty. How's this going to work? 
    
    https://www.ultraboardgames.com/avalon/game-rules.php

    1. need to store the Tableaus for each game size (Players + Good/Evil spread)
    2. Need to allow players to join a game, probably by passing in a game name + password
    3. Enemies get to see each other
    4. Merlin gets DMed the opponents
    5. Cycle between Team Building + Quest phase.
    6. TEAM BUILDING:    
            Need to store the # people per quest for 5 to 10 players.
            Wait for leader to say "+ProposeQuest (People)"
            If Majority Rules, continue.
            Else Repeat with NEXT leader
            IF THIS HAPPENS 5 TIMES, EVIL WINS

    7. QUEST PHASE:
            Players say +submit SUCCESS or FAILURE in their DMs to the bot
            GOOD players must choose success, evil characters pick either pass or fail.
            * 4th quest in games of 7+ people MUST have 2+ fails to count. Otherwise it's all good.
            On success, update tableau to show another pass
            Otherwise, assign a failure.

            If you reach 3 successful or failed quests, the game ends.
            Evil wins if:
                5 teams rejected in one round
                3 Failed quests
                Assassin is able to guess Merlin.

                * Percival, Good, known to Merlin   (Tilts towards good)
                * Mordred, Evil, invisible to Merlin @ start of game    (Tilts towards bad)
                * Oberon, Evil, Invisible to evil @ start of game   (Tilts towards good)
                * Morgana, Evil, Masquerades as Merlin, she would also be revealed to Percival. (Tilt towards evil)
    */
});

client.login(config.BOT_TOKEN);
