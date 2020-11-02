module.exports = async function (account, sentryCallBack) {
    var loader = {
        loadMatch: async function (matchId) {
            return new Promise((resolve, reject) => {
                var done = false;

                setTimeout(() => {
                    if (!done) {
                        console.log("timeout match loading")
                        resolve(-1);
                    }
                }, 20 * 1000);

                this.Dota2.requestMatchDetails(matchId, function (err, body) {

                    if (err) {
                        console.log(matchId)
                        console.log(err)

                        resolve(-1);
                    }
                    done = true;
                    resolve(body.match);
                });
            });
        },
        loadPlayerRank: async function (accountId) {
            var done = false;
            return new Promise((resolve, reject) => {
                setTimeout(() => {
                    if (!done) {
                        console.log("timeout account loading");
                        done = true;
                        resolve(-1);
                    }
                }, 20 * 1000);

                this.Dota2.requestProfileCard(accountId, function (error, profileData) {
                    if(!profileData){
                        resolve(-1);
                    }
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
    // Load in server list if we've saved one before
    if (fs.existsSync('servers')) {
        steam.servers = JSON.parse(fs.readFileSync('servers'));
    }
    let resolver;
    var retry = false;
    /* Steam logic */
    var onSteamLogOn = function onSteamLogOn(logonResp) {
     
        if (logonResp.eresult == steam.EResult.OK) {
            util.log("Logged on.");
            if(!loader.connected)
                Dota2.launch();
            loader.connected = true;
        }
        else {
            if(retry)
                return;
            sleep(5000).then(() => {
                require("./rambler_pop3")(account.email, account.email_pass).then(code => {
                    logOnDetails.auth_code = code;
                    if (code) {
                        retry = true;
                        console.log('retry with code'+code)
                        connect(options, loader, steamClient);
                    } else {
                        resolver('login fail');
                    }
                })
            });
        }
    },
        onSteamServers = function onSteamServers(servers) {
            // util.log("Received servers.");
            // fs.writeFile('servers', JSON.stringify(servers), (err) => {
            //     if (err) { if (this.debug) util.log("Error writing "); }
            //     else { if (this.debug) util.log(""); }
            // });
        },
        onSteamLogOff = function onSteamLogOff(eresult) {
            if (retry) {
                if (resolver) {
                    resolver('login fail');
                }
            }
            loader.connected = false;
            util.log("Logged off from Steam.");
        },
        onSteamError = function onSteamError(error) {
            if (retry) {
                if (resolver) {
                    resolver('login fail');
                }
            }
            loader.connected = false;
        };

    steamClient.on('connected', function () {
        steamUser.logOn(logOnDetails);
    });
    steamClient.on('logOnResponse', onSteamLogOn);
    steamClient.on('loggedOff', onSteamLogOff);
    steamClient.on('error', onSteamError);
    steamClient.on('servers', onSteamServers);
    steamUser.on('updateMachineAuth', function (sentry, callback) {
        var hashedSentry = crypto.createHash('sha1').update(sentry.bytes).digest();
        loader.sentry = hashedSentry;
        if(sentryCallBack)
            sentryCallBack(hashedSentry);
        callback({
            sha_file: hashedSentry
        });
    });


    // Login, only passing authCode if it exists
    var logOnDetails = {
        "account_name": global.config.steam_user,
        "password": global.config.steam_pass
    };
    if (global.config.steam_guard_code) logOnDetails.auth_code = global.config.steam_guard_code;
    if (global.config.two_factor_code) logOnDetails.two_factor_code = global.config.two_factor_code;

    try {
        var sentry = account.sentry | '';
        if (sentry.length) logOnDetails.sha_sentryfile = sentry;
    } catch (beef) {
        util.log("Cannae load the sentry. " + beef);
    }

    var server = steam.servers[Math.floor(Math.random() * steam.servers.length)];
    server  ={
        host: server.host,
        port: parseInt(server.port)
    }
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
    connect(options, loader, steamClient);
    
    return new Promise((resolve, reject) => {
        var done = false;
        resolver = resolve;
        setTimeout(() => {
            if (!done) {
                console.log("timeout dota2 loading")
                resolve(null);
            }
        }, 120 * 1000);
        Dota2.on("ready", function () {
            done = true;
            resolve(loader);
        });
    });
}


function sleep(ms) {
    return new Promise((resolve) => {
      setTimeout(resolve, ms);
    });
  } 

function connect(options, loader, steamClient){
    console.log("connect to proxy" +options.proxy.host)
    const SocksClient = require('socks').SocksClient;
    SocksClient.createConnection(options, (err, info) => {
        if (err)
            console.log(err);
        else {
            console.log("connected to proxy")
            if (!info.socket) {
                console.log("no socket")
                return;
            }
            loader.socket = info.socket;
            // Connection has been established, we can start sending data now: 
            steamClient.connect({ customSocket: info.socket });
        }
    });
}