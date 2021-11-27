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
    this.populate_characters = (table, good, bad, merlin, assassin) => {
        this.table = table;
        this.allies = good;
        this.minions = bad;
        this.merlin = merlin;
        this.assassin = assassin;
    }
    this.close_thread = () => Promise.resolve(this.thread.delete( "Clearing Out Finished Thread" ) );
    this.init_game = () => {
        this.ally_wins = 0;
        this.minion_wins = 0;
        this.team_build_attempts = 0;
        this.current_quest = 0;
    }
    this.get_state = () => {
        message = "Currently on quest: " + (this.current_quest) + "\n";
        message += this.current_message + "\n";
        message += "Allies <";
        message += config.ALLY_EMOTE.repeat(this.ally_wins);
        message += "|";
        message += config.MINION_EMOTE.repeat(this.minion_wins);
        message += "> Minions";

        return message;
    }
    this.print_state = () => {
        message = this.get_state();
        SendThreadMessage(message, this.thread);
    }
}

function SetupProposalState (game) {
    game.state = config.TEAM_PROPOSAL;
    game.voters = game.participants;
    game.votes_remaining = game.voters.length;
    game.team = [];
    game.passes = 0;
    game.attempts = 0;
    game.leader = Math.floor(Math.random()*game.participants.length);
    game.current_message = config.TEAM_PROPOSAL + "\n <@" + game.participants[game.leader].id + "> must use +propose " + "@team_member ".repeat(game.table[game.current_quest]);
}
function SetupQuestState (game) {
    game.state = config.QUESTING;
    game.voters = game.team;
    game.votes_remaining = game.voters.length;
    game.passes = 0;
    game.attempts = 0;
    game.leader = Math.floor(Math.random()*game.participants.length);
    game.current_message = config.TEAM_QUESTING + "\n " + game.team;
}

// Pairs each command with it's result
const commandSet = {
    create : (args, message) => {   
        // TODO: NEED TO ADD A CHECK FOR WHETHER THE CHARACTER IS A PLAYER IN AN EXISTING GAME
        if (message.channel.type !== "GUILD_TEXT"){
            message.reply("New games can only be created in a standard text channel");
            return;
        }
        else if (active_games_by_author.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + "> already has a game active. Please +destroy this game before creating a new one!");
            return;
        }
        else if( message.mentions.users.size + 1 < config.MIN_PARTICIPANTS || message.mentions.users.size + 1 > config.MAX_PARTICIPANTS ){
            message.reply("Sorry! This game only supports " + config.MIN_PARTICIPANTS + "-" + config.MAX_PARTICIPANTS + " players");
            return;
        }
        
        let participants = [];
        participants.push(message.author);
        message.mentions.users.forEach(user => {
            if (!participants.includes(user)){
                participants.push(user);
            }
        });
        
        let game = new Game(message.author, participants, message.channel);
        let thread_promise = game.open_thread();
        let players = "";
        game.participants.forEach(player => {
            players += " <@" + (player.id) + ">"; 
        });
        thread_promise.then(() => active_games_by_author[message.author] = game)
            .then(() => active_games_by_channel[game.thread.id] = game);
        message.reply("Joined Game with:" + players);
    },
    destroy : (args, message) => {
        if (!active_games_by_author.hasOwnProperty(message.author)) {
            message.reply("<@" + message.author.id + "> owns no active game!");
            return;
        }
        let game = active_games_by_author[message.author];
        game.close_thread().then(()=>{
            delete active_games_by_author[message.author];
            delete active_games_by_channel[game.channel.id];
        });
        message.reply("<@" + message.author.id + ">'s game has been destroyed!");
    },
    start : (args, message) => {
        if (!active_games_by_channel.hasOwnProperty(message.channel.id)) {
            message.reply("Games can only be started from their own thread. Create a new game and thread using +create and @ing your desired players")

        } else if (active_games_by_channel[message.channel.id].author !== message.author) {
            SendThreadMessage("Only the owner <@" + active_games_by_channel[message.channel.id].author + "> can start the game!", active_games_by_channel[message.channel.id].thread)

        } else {
            // ASSIGN PLAYER ROLES
            let game = active_games_by_channel[message.channel.id];
            
            SendThreadMessage("" + game.participants.length + " ", game.thread);
            let num_players = game.participants.length;
            let team_table = tables.NET_TEAM_SIZE[num_players.toString()];
            let quest_table = tables.QUEST_TEAM_SIZE[num_players.toString()];
            let participants_indices = Array.from({length: num_players}, (_, i) => i)

            let allies = [];
            for (let i = 0; i < team_table.GOOD ; i++){
                var randomIndex = Math.floor(Math.random()*participants_indices.length);
                allies.push(participants_indices.splice(randomIndex, 1)[0]);
            }

            let minions = participants_indices;
            let merlin = allies[0];
            let assassin = minions[0];

            game.populate_characters(quest_table, allies, minions, merlin, assassin);

            SendThreadMessage("Started game: " + game.thread_name + " " + allies.length + "Allies vs " + minions.length + " Minions", game.thread)
            .then(()=> {
                // SEND ROLE MESSAGES
                let minion_names = ""
                minions.forEach(minion => {
                    minion_names += game.participants[minion].username + " "
                })
                
                minions.forEach(minion => {
                    message = ""
                    if (game.participants[minion].id != game.participants[assassin].id){
                        message += "Hello Minion<@" + game.participants[minion].id + ">!\n" + config.MINION_ROLE_TEXT
                    } else {
                        message += "Hello Assassin <@" + game.participants[minion].id + ">!\n" + config.ASSASSIN_ROLE_TEXT
                    }
                    message += "Your fellow Minions of Mordred are: " + minion_names;
                    SendDirectMessage(message, game.participants[minion]);
                    //SendThreadMessage(message, game.thread);
                })

                
                allies.forEach(ally => {
                    message = ""
                    if (game.participants[ally].id != game.participants[merlin].id){
                        message += "Hello Ally <@" + game.participants[ally].id + ">!\n";
                        message += config.ALLY_ROLE_TEXT;
                        message += "You must use your wit and will to complete three quests without revealing Merlin's Identity!\n";
                    }
                    else {
                        message += "Hello Merlin <@" + game.participants[ally].id + ">!\n";
                        message += "You must use your wit and will to complete three quests without revealing your Identity!\n";
                        message += "Be cautious, enemies are abound. Look out for the Minions of Mordred: " + minion_names;
                    }
                    SendDirectMessage(message, game.participants[ally]);
                    //SendThreadMessage(message, game.thread);
                })

                SendThreadMessage("Please check your DMs to see your role!", game.thread);
            }).then(() => { 
                game.init_game();
                SetupProposalState(game);
                game.print_state();
            });
            
            // Need to initiate game
            message.reply("<@" + message.author + "> Started the game!.");
        }
    },
    propose : (args, message) => {     // TESTER FUNCTION
        if (!active_games_by_channel.hasOwnProperty(message.channel.id)) {
            message.reply("Proposals must be made within a Game's thread")
        } else if (active_games_by_channel[message.channel.id].participants.includes(message.author)){
            let game = active_games_by_channel[message.channel.id];
            if (game.state !== config.TEAM_PROPOSAL){
                message.reply("This game is currently not in the proposal phase \n" + game.get_state());
            }
            else if (message.author !== game.participants[game.leader]){
                SendThreadMessage("Only the current leader <@" + game.participants[game.leader].id + "> can make a proposal!", game.thread);
            } 
            else {
                let proposed = []
                let proposed_string = ""
                message.mentions.users.forEach(user => {
                    if (game.participants.includes(user) && !proposed.includes(user)){
                        proposed.push(user);
                        proposed_string += "" + user.username + ", ";
                    }
                });
                if (proposed.length !== game.table[game.current_quest]){
                    message.reply("The current quest has a required party size of: " + game.table[game.current_quest] + "\nPlease ensure your proposal meets this requirement!" );
                }
                else {
                    game.team = proposed;
                    message.reply("<@" + message.author.id + "> proposed: " + proposed_string + " for this quest.\nPlease check your dms and respond with +vote pass or +vote fail");
                    game.participants.forEach(player => {
                        SendDirectMessage("Proposed team is: " + args + "\n use +vote pass or +vote fail to approve or deny the combination.\nA MAJORITY VOTE IS REQUIRED, IF NOT SECURED, TRY AGAIN.\n IF FIVE COMBINATIONS ARE REJECTED, THE VILLAINS WIN!", player);
                    });
                }
            }
        } 
    },
    vote : (args, message) => {
        if (!message.guild == null && !message.author.bot) {
            message.reply("Votes must be cast in a DM to the bot")
            return;
        }
        console.log();
        let game = false;
        Object.keys(active_games_by_author).forEach(key => {
            if (active_games_by_author[key].participants.includes(message.author)) {
                game = active_games_by_author[key];
            }
        });
        if (game === false){
            message.reply("You must join and start a game to participate in a vote");
            return;
        } else if (!game.voters.includes(message.author)){
            message.reply("Either you have already submitted a vote, or you are not required to vote!");
            return;
        } else {
            let i = game.participants.indexOf(message.author);
            let vote = args.shift().toLowerCase();
            if (vote === "pass") {
                game.passes += 1;
                game.votes_remaining -= 1;
                message.reply("Vote logged!");
            }
            else if (vote === "fail") {
                if (game.minions.includes(i)){
                    game.passes -= 1;
                    game.votes_remaining -= 1;
                    message.reply("Vote logged!");
                }
                message.reply("You're a hero, not a minion! Please use +vote pass");
                return;
            }
            else {
                message.reply("Please vote with either +vote pass or +vote fail");
                return;
            }
            
            SendThreadMessage("<@" + message.author + "> Voted! " + game.votes_remaining + " Votes Remaining!", game.thread);
            if (game.votes_remaining === 0) {
                if (game.state === config.TEAM_PROPOSAL) {
                    if (game.passes > game.participants.length / 2) {
                        SetupQuestState(game);
                        game.print_state();
                        SendThreadMessage("it is time! Use +vote pass or +vote fail to sway the outcome of the quest!", game.thread);
                    } 
                    else {
                        game.attempts += 1;
                        if (game.attempts >= 5) {
                            let text = "GAME OVER!!!\nThe villanous minions of Mordred successfully foiled your quest! The infighting they caused threw the party into turmoil.\n\nCongratulations:";
                            game.minions.forEach(minion => {
                                text += " <@" + game.participants[minion].id + ">,";
                            });
                            text += "\n\n Nice try: "
                            game.allies.forEach(ally => {
                                text += " <@" + game.participants[ally].id + ">,";
                            });

                            SendThreadMessage(text, game.thread)
                        } else {
                            SetupProposalState(game);
                            SendThreadMessage("How unfortunate, you did not get enough votes to move on.\nTry again, perhaps this new leader will help you more: <@" +game.participants[game.leader].id +">", game.thread);
                        }
                    }
                } else if (game.state === config.QUESTING){
                    let success = game.passes >= game.table[game.current_quest.toString()];
                    if (success) {
                        game.ally_wins += 1;
                    } else {
                        game.minion_wins += 1;
                    }

                    game.current_quest += 1;
                    if (game.ally_wins >= 3){
                        game.print_state()
                        let text = "Egads! The heroes have made it back with the Holy Grail! \n@Assassin, you have one final shot. If you can correctly +assassinate Merlin you will take the victory!";
                        SendThreadMessage(text, game.thread);
                        SendDirectMessage("use +assassinate @target to try and defeat Merlin once and for all!", game.participants[game.assassin]);
                    } else if (game.minion_wins >= 3){
                        let text = "GAME OVER!!!\nThe villanous minions of Mordred successfully foiled your quest! The infighting they caused threw the party into turmoil.\n\nCongratulations:";
                        game.minions.forEach(minion => {
                            text += " <@" + game.participants[minion].id + ">,";
                        });
                        text += "\n\n Nice try: "
                        game.allies.forEach(ally => {
                            text += " <@" + game.participants[ally].id + ">,";
                        });
                        SendThreadMessage(text, game.thread)
                    }
                }
            }
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
        console.log(command + " | " + args);
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
