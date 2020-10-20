let accountRepositry;
let matchRepository;
const requestLimit = 100;
const { v4: uuidv4 } = require('uuid');
const id = uuidv4();
console.log(id);

async function start(){
    accountRepositry = await require("./repositories/accounts_repository")();
    matchRepository = await require("./repositories/match_repository")();

    await accountRepositry.insertAccounts();

    var account = (await accountRepositry.reserve(id)).value;
    if(account)
        await connectAndLoad(account);
    else{
        console.log('account not found')
    }
}

// async function checkAccount(account) {
//     await assignProxyToAcc(account);
//     var loader = await require("./privateMatchLoader.js")(account);
//     if(loader == 'login fail')
//     {
//         await accountRepositry.updateFailLogin(account);
//         return;
//     }else{
//         if(loader.socket)
//             loader.socket.destroy();
//     }
// }

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
        var time = Math.round(+new Date()/1000)- account.lastRequestTime;
        if(time>86400){
            account.requestCount = 0;
            await accountRepositry.update(account);
        }
        else {
            console.log("account skipped, rate limit"+time)
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

    await loadMatches(account, loader);
}

async function loadMatches(account,loader){
    while(true) {
        var matches = await matchRepository.getNotProcessed()
        if(matches.length==0){
            console.log('no matches')
        }
        deleteIds = [];
        for (let index = 0; index < matches.length; index++) {
            const dbMatch = matches[index];
            var player = dbMatch.players.find(x=>x.account_id != 4294967295);
            if(player){
                var rank =await loader.loadPlayerRank(player.account_id);
                if(rank && rank>=70){//80+ imortals, 70+ divene
                    console.log('start match load'+rank+"s"+player.account_id)

                    var match = await loader.loadMatch(dbMatch.match_id);
                    account.requestCount = (account.requestCount|0)+1;
                    await accountRepositry.update(account);
                    if(account.requestCount > requestLimit)
                        break;  
                    matchRepository.update(match, dbMatch);

                    console.log('match loaded'+dbMatch.match_id)
                } else {
                    deleteIds.push(dbMatch._id);
                }
            } else {
                deleteIds.push(dbMatch._id);
            }
        }
        await matchRepository.delete(deleteIds);
        console.log("deleted:"+deleteIds.length)
    }
}
start();