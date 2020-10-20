module.exports = function(account){
    var loader = {
        loadMatch: async function (matchId){
            return new Promise((resolve, reject) => {
                var done = false;
                
                setTimeout(() => {
                    if(!done)
                    {
                        console.log("timeout match loading")
                        resolve(null);
                    }
                  }, 20*1000);
                
                this.Dota2.requestMatchDetails(matchId, function(err, body) {
                    
                    if (err) {
                        console.log(matchId)
                        console.log(err)

                        throw err;
                    }
                    done = true;
                    resolve(body.match);
                });
            });
        },
        loadPlayerRank: async function(accountId){
            return new Promise((resolve, reject) => {
                var done = false;
                setTimeout(() => {
                    if(!done)
                    {
                        console.log("timeout account loading")
                        resolve(null);
                    }
                  }, 20*1000);
                
                  this.Dota2.requestProfileCard(accountId,function (error, profileData) {
                    resolve(profileData.rank_tier);
                    done = true;
                });
            });
        }
    }
    var steam = require("steam-with-proxy"),
        util = require("util"),
        fs = require("fs"),
        crypto = require("crypto"),
        dota2 = require("dota2-with-proxy"),
        steamClient = new steam.SteamClient(),
        steamUser = new steam.SteamUser(steamClient),
        Dota2 = new dota2.Dota2Client(steamClient, false);
    loader.Dota2 = Dota2;
    // Load config
    global.config = account;
    console.log(account)
    // Load in server list if we've saved one before
    if (fs.existsSync('servers')) {
        steam.servers = JSON.parse(fs.readFileSync('servers'));
    }
    let resolver;

    /* Steam logic */
    var onSteamLogOn = function onSteamLogOn(logonResp) {
            if (logonResp.eresult == steam.EResult.OK) {
                util.log("Logged on.");
                Dota2.launch();
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
            if(resolver){
                resolver('login fail');
            }
            util.log("Logged off from Steam.");
        },
        onSteamError = function onSteamError(error) {
            if(resolver){
                resolver('login fail');
            }
            util.log("Connection closed by server: "+error);
        };

    steamUser.on('updateMachineAuth', function(sentry, callback) {
        var hashedSentry = crypto.createHash('sha1').update(sentry.bytes).digest();
        loader.sentry = hashedSentry;
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
        var sentry = account.sentry|'';
        if (sentry.length) logOnDetails.sha_sentryfile = sentry;
    } catch (beef) {
        util.log("Cannae load the sentry. " + beef);
    }

    var server = steam.servers[Math.floor(Math.random() * steam.servers.length)];

    const options = {
        proxy: {
            host: account.proxy.host, // ipv4 or ipv6 or hostname174.77.111.197:41455
            port: parseInt(account.proxy.port),
            type: 5 // Proxy version (4 or 5)
        },
    
        command: 'connect', // SOCKS command (createConnection factory function only supports the connect command)
    
        destination: server
    };
    console.log(options)

    console.log("connect to proxy")
    const SocksClient = require('socks').SocksClient;
    SocksClient.createConnection(options, (err, info) => {
        if (err)
            console.log(err);
        else {
            console.log("connected to proxy")
            if(!info.socket){
                console.log("no socket")
            }
            loader.socket = info.socket;
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
    return new Promise((resolve, reject) => {
            var done = false;
            resolver = resolve;
            setTimeout(() => {
                if(!done)
                {
                    console.log("timeout dota2 loading")
                    resolve(null);
                }
            }, 80*1000);
            Dota2.on("ready", function() {
                done = true;
                // Dota2.requestProfileCard(72890132);
                // Dota2.on("profileCardData", function (accountId, profileData) {
                //     console.log(JSON.stringify(profileData, null, 2));
                // });
              
               
                resolve(loader);
            });
        });
}