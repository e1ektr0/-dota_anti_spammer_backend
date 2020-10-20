module.exports = function(account){
    var loader = {

    }
    var steam = require("steam"),
        util = require("util"),
        fs = require("fs"),
        crypto = require("crypto"),
        dota2 = require("dota2"),
        steamClient = new steam.SteamClient(),
        steamUser = new steam.SteamUser(steamClient),
        Dota2 = new dota2.Dota2Client(steamClient, false);
    loader.Dota2 = Dota2;
    // Load config
    global.config = account;
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


    var logOnDetails = {
        "account_name": global.config.steam_user,
        "password": global.config.steam_pass,
    };

    try {
        var sentry = account.sentry|'';
        if (sentry.length) logOnDetails.sha_sentryfile = sentry;
    } catch (beef) {
        util.log("Cannae load the sentry. " + beef);
    }

    steamClient.connect( );
    steamClient.on('connected', function() {
        steamUser.logOn(logOnDetails);
    });
    steamClient.on('logOnResponse', onSteamLogOn);
    steamClient.on('loggedOff', onSteamLogOff);
    steamClient.on('error', onSteamError);
    steamClient.on('servers', onSteamServers);
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
              
                resolve(loader);
            });
        });
}