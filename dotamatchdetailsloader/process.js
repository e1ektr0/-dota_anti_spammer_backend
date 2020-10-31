let accountRepositry;
let matchRepository;
const requestLimit = 95;
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
        await accountRepositry.updateProxy(account);
        console.log("loader fail");
        return;
    }

    return loader;
}

async function assignProxyToAcc(account) {
    if (!account.proxy) {
        if (!await accountRepositry.updateProxy(account)) {
            console.log('no proxy for this acc')
            return;
        }
    }
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

        // var i = 0;
        // while (true) {
        //     let results = await resultRepository.getNoSteam();
        //     if (results.length == 0) break;
        //     for (let index = 0; index < results.length; index++) {
        //         const result = results[index];
        //         await resultRepository.update({ _id: result._id }, { steam_id: loader.Dota2.ToSteamID(result.account_id).toString() })
        //     }
        //     console.log(i+=results.length);
        // }
        var dbMatch = (await matchRepository.getNotProcessed(id)).value;
        if (!dbMatch) {
            await sleep(1000);
            continue;
        }
        var player = dbMatch.players.find(x => x.account_id != 4294967295);
        if (player) {
            var rank = await loader.loadPlayerRank(player.account_id);
            if (rank != -1 && rank >= 70) {//80+ imortals, 70+ divene
                console.log('start match load ' + dbMatch.match_id + ' for rank' + rank + " by player account id " + player.account_id)
                var match = await loader.loadMatch(dbMatch.match_id);
                if (match != -1) {
                    await resultRepository.add(match, dbMatch, loader);
                    await matchRepository.delete(dbMatch._id);
                    loaded++;
                    totalLoaded++;
                } else {
                    console.log("fail load match info" + match)
                    account.requestCount = 100;
                }

                account.requestCount = (account.requestCount | 0) + 1;
                await accountRepositry.update(account);
                if (account.requestCount > requestLimit)
                    break;
                console.log('match loaded' + dbMatch.match_id)
            } else {
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
