let accountRepositry;
let matchRepository;
const requestLimit = 95;
const e = require('express');
const { v4: uuidv4 } = require('uuid');
const id = uuidv4();
console.log(id);

async function start() {
    accountRepositry = await require("./repositories/accounts_repository")();
    matchRepository = await require("./repositories/match_repository")();
    resultRepository = await require("./repositories/result_repositroy")();
    await accountRepositry.insertAccounts();
    while (true) {
        var account = (await accountRepositry.reserve(id)).value;
        if (account) {
            var loader = await createLoader(account);
            if (loader)
                await loadMatches(account, loader);
        }
        else {
            console.log('account not found')
            await sleep(1000);
        }
    }

}

async function createLoader(account) {
    if (account.requestCount >= requestLimit) {
        var time = Math.round(+new Date() / 1000) - account.lastRequestTime;
        if (time > 86400) {
            account.requestCount = 0;
            await accountRepositry.update(account);
        }
        else {
            console.log("account skipped, rate limit" + time)
            return;
        }
    }

    await assignProxyToAcc(account);
    var loader = await require("./privateMatchLoader.js")(account,async function(sentry){
        account.sentry = sentry; 
        await accountRepositry.updateSentry(account);
    });
    console.log("loader created")
    if (loader == 'login fail') {
        await accountRepositry.updateFailLogin(account);
        return;
    }
    if (loader == null) {
        console.log("loader fail");
        console.log(account)
        account.requestCount =100;
        await accountRepositry.update(account);
        
        return;
    }

    return loader;
}

async function assignProxyToAcc(account) {
    if (!account.proxy) {
        if (!await accountRepositry.updateProxy(account)) {
            console.log('no proxy for this acc')
            return false;;
        }
        return false;;
    }
    return true;
}


let totalDeleted = 0;
let totalLoaded = 0;
function sleep(ms) {
    return new Promise((resolve) => {
        setTimeout(resolve, ms);
    });
}
async function loadMatches(account, loader) {
    let startTime = Math.round(+new Date() / 1000);
    let deleted = 0;
    let loaded = 0;
    while (true) {
        
        if (!loader.connected)
            return;

        var dbMatch = (await matchRepository.getNotProcessed(id)).value;
        if (!dbMatch) {
            await sleep(1000);
            continue;
        }
        var player = dbMatch.players.find(x => x.account_id != 4294967295);
        if (player) {
            if(dbMatch.HightRankProof || (await loader.loadPlayerRank(player.account_id)) >= 70){
                var rank = 0;
                for (let index = 0; index < dbMatch.players.length; index++) {
                    const currentPlayer = dbMatch.players[index];
                    if(currentPlayer.account_id == 4294967295)
                        continue;
                    dbMatch.players[index].lrank = await loader.loadPlayerRank(currentPlayer.account_id);
                    if(rank< dbMatch.players[index].lrank)
                        rank = dbMatch.players[index].lrank;
                    if(dbMatch.players[index].lrank>0)
                        await resultRepository.updateRank(dbMatch.players[index].account_id, dbMatch.players[index].lrank);
                }
                if(rank == 0 || rank >70){
                    var match = await loader.loadMatch(dbMatch.match_id);
                    if (match != -1) {
                        console.log('start match load ' + dbMatch.match_id+ 'with rank'+rank)
                        await resultRepository.add(match, dbMatch, loader);
                        await matchRepository.delete(dbMatch._id);
                        await matchRepository.addSalt(dbMatch.match_id, match.replay_salt, match.cluster, match);
                        loaded++;
                        totalLoaded++;
                        
                        console.log('match loaded' + dbMatch.match_id)
                        account.requestCount = (account.requestCount | 0) + 1;
                        account.totalRequestCount = (account.totalRequestCount | 0) + 1;
                        
                        await accountRepositry.update(account);
                    } else {
                        console.log("fail load match info" + match)
                        account.requestCount = 100;
                    }
                }
                else{
                    await matchRepository.delete(dbMatch._id);
                    deleted++;
                    totalDeleted++;
                }  
               
            }else{
                await matchRepository.delete(dbMatch._id);
                deleted++;
                totalDeleted++;
            }
        } else {
            await matchRepository.delete(dbMatch._id);
            deleted++;
            totalDeleted++;
        }
        await accountRepositry.update(account);
        if (account.requestCount > requestLimit)
            break;
        let now = Math.round(+new Date() / 1000);
        if (now - startTime > 60) {
            startTime = now;

            console.log("Deleted:" + deleted + "(" + totalDeleted + ") Loaded:" + loaded + "(" + totalLoaded + ")")
            console.log("Loaded" + loaded)
            deleted = 0;
            loaded = 0;
        }
    }
}

start();
