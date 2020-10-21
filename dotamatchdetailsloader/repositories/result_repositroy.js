module.exports = async function(){
    var db = await require("../db")();
    try{
        if(!(await db.listCollections().toArray()).some(x=>x.name == "results"))
            db.createCollection("results")
    }
    catch(e){
        console.log(e);
    }
    
    const collection = db.collection("results");
    var obj = {
        add: async function(match,dbMatch){
            var bans = match.picks_bans.filter(x=>!x.is_pick).map(x=>x.hero_id)
            var results = match.players.map(p=>({
                account_id: p.account_id,
                startTime: match.startTime,
                hero_id: p.hero_id,
                hero_pick_order: p.hero_pick_order,
                bans:bans,
                match_id:dbMatch.match_id,
                match_seq_num: dbMatch.match_seq_num,
                win : p.player_slot>100?!dbMatch.radiant_win:dbMatch.radiant_win
            }))
            await collection.insertMany(results);
        }
    }
    return obj;
}