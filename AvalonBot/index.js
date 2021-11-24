require('dotenv').config();
const Discord = require('discord.js');
const config = require('./config.json');
const tables = require('./game_tables.json');

const client = new Discord.Client({ intents: ["GUILD_MESSAGES", "DIRECT_MESSAGES", "GUILDS"] });
const active_games_by_author = {};
const active_games_by_channel = {};

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
    this.author = author;
    this.participants = participants;
    this.started = false;

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
    this.populate_characters = (good, bad, merlin, assassin) => {
        this.allies = good
        this.minions = bad
        this.merlin = merlin
        this.assassin = assassin
    }
    this.close_thread = () => Promise.resolve(this.thread.delete( "Clearing Out Finished Thread" ) );
    this.print_state = () => {
        // This should just print out:
        // Current score (Successes vs Fails, maybe like a sword?)
        // Current state
    }
}

// Pairs each command with it's result
const commandSet = {
    ping : (args, message) => {
        message.reply("PONG! ARGS: " + args);
    },
    create : (args, message) => {   
        // TODO: NEED TO ADD A CHECK FOR WHETHER THE CHARACTER IS A PLAYER IN AN EXISTING GAME
        if (message.channel.type !== "GUILD_TEXT"){
            message.reply("New games can only be created in a standard text channel");
            return;
        }
        else if (active_games_by_author.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + ">'s Already has a game active. Please +destroy this game before creating a new one!");
            return;
        }
        else if( message.mentions.users.size + 1 < config.MIN_PARTICIPANTS || message.mentions.users.size + 1 > config.MAX_PARTICIPANTS ){
            message.reply("Sorry! This game only supports " + config.MIN_PARTICIPANTS + "-" + config.MAX_PARTICIPANTS + " players : " + message.mentions.users.size);
            return;
        }
        
        let participants = [];
        participants.push(message.author);
        message.mentions.users.forEach(user => {
            participants.push(user);
        });
        
        let game = new Game(message.author, participants, message.channel);
        let thread_promise = game.open_thread();
        let players = "";
        game.participants.forEach(player => {
            players += " <@" + (player.id) + ">"; 
        });
        thread_promise.then(() => active_games_by_author[message.author] = game)
            .then(() => active_games_by_channel[game.thread.id] = game);
        message.reply("Created Game with:" + players);
    },
    destroy : (args, message) => {
        if (!active_games_by_author.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + "> had no active game!");
            return;
        }
        let game = active_games_by_author[message.author];
        game.close_thread().then(()=>{
            delete active_games_by_author[message.author];
            delete active_games_by_channel[game.channel.id];
        });
        message.reply("<@" + message.author.id + ">'s game has been removed!");
    },
    start : (args, message) => {
        if (!active_games_by_channel.hasOwnProperty(message.channel.id)) {
            message.reply("Games can only be started from their own thread. Create a new game + thread using +create @user1 @user2")

        } else if (active_games_by_author.hasOwnProperty(message.author) && active_games_by_channel[message.channel.id].author !== message.author) {
            SendThreadMessage("Only the owner <@" + active_games_by_channel[message.channel.id].author + "> can start the game!", active_games_by_channel[message.channel.id].thread)

        } else {
            // ASSIGN PLAYER ROLES
            let game = active_games_by_channel[message.channel.id];
            
            SendThreadMessage("" + game.participants.length + " ", game.thread);
            let num_players = game.participants.length;
            let game_table = tables.PLAYER_COUNTS[num_players.toString()];

            let participants_indices = Array.from({length: num_players}, (_, i) => i);
            let allies = Array.from({length: game_table.GOOD}, (_, i) => {
                var randomIndex = Math.floor(Math.random()*participants_indices.length);
                return participants_indices.splice(randomIndex, 1)[0];
            });
            let minions = participants_indices;
            let merlin = allies[0];
            let assassin = minions[0];

            game.populate_characters(allies, minions, merlin, assassin);

            SendDirectMessage("Started game: " + game.thread_name, game.author)
            .then(()=> {
                // SEND ROLE MESSAGES
                let names = "Your fellow Minions of Mordred are: "
                minions.forEach(minion => {
                    names += game.participants[minion].username + " "
                })
                
                message = ""
                minions.forEach(minion => {
                    if (game.participants[minion].id != game.participants[assassin].id){
                        message += "Hello Minion!\nYour task is to ensure the heroes do not beat three quests. Be careful and use your cunning to avoid detection!\n\n"
                    } else {
                        message += "Hello Assassin!\nYour main task is to ensure the heroes cannot beat three quests. Keep a sharp eye though. \nIf they do succeed, you will have one final shot to assassinate Merlin and steal the victory!\n\n"
                    }
                    message += names;
                    SendDirectMessage(message, game.participants[assassin]);
                })

                message = ""
                
                allies.forEach(ally => {
                    if (game.participants[ally].id != game.participants[merlin].id){
                        message += "Hello Ally!\n";
                    }
                    else {
                        message += "Hello Merlin!\n";
                    }
                    message += "You must use your wit and will to complete three quests without revealing Merlin's Identity!";
                    SendDirectMessage(message, game.participants[ally]);
                })

                SendThreadMessage("Please check your DMs to see your role!", game.thread);
            }).then(() => {
                // Print Game
                // This will print out the current counters + the current phase.
                // Depending on phase, it will either print out:
                    // Leader needs help deciding on a team!
                    // Waiting for all votes!
            });
            
            // Need to initiate game
            message.reply("<@" + message.author + "> Started the game!.");
        }
    },
    send : (args, message) => {     // TESTER FUNCTION
        if (active_games_by_author.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + "> Sent message:" + args + "!");
        }
    }
}

function SendThreadMessage(text, thread)
{
    return Promise.resolve(thread.send(text))
}

function SendDirectMessage(text, user)
{
    if (user.bot)
    {
        return Promise.resolve(() => console.log(text))
    }
    return Promise.resolve(user.send(text))
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
