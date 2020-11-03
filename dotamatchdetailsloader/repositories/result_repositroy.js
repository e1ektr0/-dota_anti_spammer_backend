module.exports = async function(){
    var db = await require("../db")();
    try{
        if(!(await db.listCollections().toArray()).some(x=>x.name == "results"))
            db.createCollection("results");
        if(!(await db.listCollections().toArray()).some(x=>x.name == "player_accounts"))
            db.createCollection("player_accounts");
    }
    catch(e){
        console.log(e);
    }
    
    const collection = db.collection("results");
    const collectionAccounts = db.collection("player_accounts");
    var obj = {
        add: async function(match,dbMatch, loader){
            var bans = match.picks_bans.filter(x=>!x.is_pick).map(x=>x.hero_id)
            var results = match.players.map(p=>({
                account_id: p.account_id,
                steam_id: loader.Dota2.ToSteamID(p.account_id).toString(),
                startTime: match.startTime,
                hero_id: p.hero_id,
                hero_pick_order: p.hero_pick_order,
                bans:bans,
                match_id:dbMatch.match_id,
                match_seq_num: dbMatch.match_seq_num,
                win : p.player_slot>100?!dbMatch.radiant_win:dbMatch.radiant_win,
                rank: p.lrank,
                party_id:p.party_id
            }))
            await collection.insertMany(results);
            for (let index = 0; index < results.length; index++) {
                const result = results[index];
                await collectionAccounts.updateOne({account_id: result.account_id},{ $set:{account_id: result.account_id,last_match_seq_num: result.match_seq_num }}, { upsert: true })
            }
        },
        updateRank: async function(account_id, rank){
            await collectionAccounts.updateOne({account_id: account_id},{ $set:{account_id: account_id, 'stats.rank': rank}}, { upsert: true })
        },
        getNoSteam: async function(){
            return await collection.find({steam_id: null}).limit(100).toArray()
        },
        update: async function(filter, change){
            await collection.findOneAndUpdate( filter, {$set:change});
        }
    }
    return obj;
}