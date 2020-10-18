var steam = require("steam-with-proxy"),
    util = require("util"),
    fs = require("fs"),
    crypto = require("crypto"),
    dota2 = require("dota2-with-proxy"),
    steamClient = new steam.SteamClient(),
    steamUser = new steam.SteamUser(steamClient),
    steamFriends = new steam.SteamFriends(steamClient),
    Dota2 = new dota2.Dota2Client(steamClient, true);

const SocksClient = require('socks').SocksClient;
// Load config
global.config = require("./config");

// Load in server list if we've saved one before
if (fs.existsSync('servers')) {
  steam.servers = JSON.parse(fs.readFileSync('servers'));
}

/* Steam logic */
var onSteamLogOn = function onSteamLogOn(logonResp) {
        if (logonResp.eresult == steam.EResult.OK) {
            steamFriends.setPersonaState(steam.EPersonaState.Busy); // to display your steamClient's status as "Online"
            steamFriends.setPersonaName(global.config.steam_name); // to change its nickname
            util.log("Logged on.");
            Dota2.launch();
            Dota2.on("ready", function() {
                console.log("Node-dota2 ready.");                
                Dota2.requestMatchDetails(5658934599, function(err, body) {
                      if (err) throw err;
                      console.log(JSON.stringify(body));
                  });
            });
            Dota2.on("unready", function onUnready() {
                console.log("Node-dota2 unready.");
            });
            Dota2.on("chatMessage", function(channel, personaName, message) {
                // util.log([channel, personaName, message].join(", "));
            });
            Dota2.on("guildInvite", function(guildId, guildName, inviter) {
                // Dota2.setGuildAccountRole(guildId, 75028261, 3);
            });
            Dota2.on("unhandled", function(kMsg) {
                util.log("UNHANDLED MESSAGE " + dota2._getMessageName(kMsg));
            });
            Dota2.on("matchDetailsData", function (matchId, matchData) {
                //console.log(JSON.stringify(matchData, null, 2));
                Dota2.requestMatchDetails(5658934599, function(err, body) {
                    if (err) throw err;
                    console.log(JSON.stringify(body));
                });
            });
            // setTimeout(function(){ Dota2.exit(); }, 5000);
        }
    },
    onSteamServers = function onSteamServers(servers) {
        util.log("Received servers.");
        fs.writeFile('servers', JSON.stringify(servers), (err)=>{
            if (err) {if (this.debug) util.log("Error writing ");}
            else {if (this.debug) util.log("");}
        });
    },
    onSteamLogOff = function onSteamLogOff(eresult) {
        util.log("Logged off from Steam.");
    },
    onSteamError = function onSteamError(error) {
        util.log("Connection closed by server: "+error);
    };

steamUser.on('updateMachineAuth', function(sentry, callback) {
    var hashedSentry = crypto.createHash('sha1').update(sentry.bytes).digest();
    fs.writeFileSync('sentry', hashedSentry)
    util.log("sentryfile saved");
    callback({
        sha_file: hashedSentry
    });
});


// Login, only passing authCode if it exists
var logOnDetails = {
    "account_name": global.config.steam_user,
    "password": global.config.steam_pass,
};
if (global.config.steam_guard_code) logOnDetails.auth_code = global.config.steam_guard_code;
if (global.config.two_factor_code) logOnDetails.two_factor_code = global.config.two_factor_code;

try {
    var sentry = fs.readFileSync('sentry');
    if (sentry.length) logOnDetails.sha_sentryfile = sentry;
} catch (beef) {
    util.log("Cannae load the sentry. " + beef);
}


var server = steam.servers[Math.floor(Math.random() * steam.servers.length)];

const options = {
    proxy: {
      host: '98.185.94.76', // ipv4 or ipv6 or hostname
      port: 4145,
      type: 5 // Proxy version (4 or 5)
    },
   
    command: 'connect', // SOCKS command (createConnection factory function only supports the connect command)
   
    destination: server
  };
console.log("connect to proxy")
SocksClient.createConnection(options, (err, info) => {
    if (err)
        console.log(err);
    else {
        console.log("connected to proxy")

        // Connection has been established, we can start sending data now: 
        steamClient.connect( {customSocket : info.socket});
        steamClient.on('connected', function() {
            steamUser.logOn(logOnDetails);
        });
        steamClient.on('logOnResponse', onSteamLogOn);
        steamClient.on('loggedOff', onSteamLogOff);
        steamClient.on('error', onSteamError);
        steamClient.on('servers', onSteamServers);
    }
});

