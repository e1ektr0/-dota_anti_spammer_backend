let accountRepositry;
let matchRepository;
let db;
const requestLimit = 100;

async function start(){
    db = await require("./db")();
    accountRepositry = await require("./repositories/accounts_repository")();
    matchRepository = await require("./repositories/match_repository")();

    await accountRepositry.insertAccounts();

    var accounts = await accountRepositry.getAll();
    for (let index = 0; index < accounts.length; index++) {
        const account = accounts[index];
        await connectAndLoad(account);
    }
}

async function connectAndLoad(account){
    if(account.requestCount>=requestLimit){
        if(Math.round(+new Date()/1000)- account.lastRequestTime>86400){
            account.requestCount = 0;
            await accountRepositry.update(account);
        }
        else {
            return;
        }
    }
    if(account.failLogin)
        return;

    account.steam_name = account.steam_user;
    if(!account.proxy){
        if(!await accountRepositry.updateProxy(account)){
            console.log('no proxy for this acc')
            return;
        }
    }
    console.log(account)
    var loader = await require("./privateMatchLoader.js")(account);
    console.log("loader created")
    if(loader == 'login fail')
    {
        await accountRepositry.updateFailLogin(account);
        return;
    }
    if(loader == null)
    {
        await accountRepositry.updateProxy(account);
        console.log("loader fail");
        return;
    }

    await loadMatches(account,loader);
}

async function loadMatches(account,loader){
    while(true) {
        var matches = await matchRepository.getNotProcessed()
        if(matches.length > 0) {
            var dbMatch = matches[0];
            var match = await load(loader, dbMatch.match_id);
            if(match == null) {
                match = await load(loader, dbMatch.match_id);
                if(match == null)
                {
                    break;
                }
            }
            account.requestCount = (account.requestCount|0)+1;
            await accountRepositry.update(account);
            if(account.requestCount > requestLimit)
                break;  

            matchRepository.update(match, dbMatch);
            console.log('match loaded')
        }
    }
}

async function load(loader, matchId){
    return new Promise((resolve, reject) => {
        var done = false;
        
        setTimeout(() => {
            if(!done)
            {
                console.log("timeout match loading")
                resolve(null);
            }
          }, 20*1000);
        loader.Dota2.requestMatchDetails(matchId, function(err, body) {
            if (err) throw err;
            done = true;
            resolve(body.match);
        });
    });
}

start();