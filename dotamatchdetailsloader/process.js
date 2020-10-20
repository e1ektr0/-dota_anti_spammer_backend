let accountRepositry;
let matchRepository;
const requestLimit = 100;

async function start(){
    accountRepositry = await require("./repositories/accounts_repository")();
    matchRepository = await require("./repositories/match_repository")();

    await accountRepositry.insertAccounts();

    var accounts = await accountRepositry.getAll();
    for (let index = 0; index < accounts.length; index++) {
        const account = accounts[index];
        await connectAndLoad(account);
        // if(!account.proxy)
        //     await checkAccount(account);
    }
}

async function checkAccount(account) {
    await assignProxyToAcc(account);
    var loader = await require("./privateMatchLoader.js")(account);
    if(loader == 'login fail')
    {
        await accountRepositry.updateFailLogin(account);
        return;
    }else{
        if(loader.socket)
            loader.socket.destroy();
    }
}

async function assignProxyToAcc(account){
    if(!account.proxy){
        if(!await accountRepositry.updateProxy(account)){
            console.log('no proxy for this acc')
            return;
        }
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

    await assignProxyToAcc(account);
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
            console.log(match);
            account.requestCount = (account.requestCount|0)+1;
            await accountRepositry.update(account);
            if(account.requestCount > requestLimit)
                break;  

            matchRepository.update(match, dbMatch);
            console.log('match loaded')
        }else{
            console.log('no matches')
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