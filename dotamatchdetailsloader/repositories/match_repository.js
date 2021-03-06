module.exports = async function(){
    var db = await require("../db")();

    if(!(await db.listCollections().toArray()).some(x=>x.name == "matches_analyze"))
        db.createCollection("matches_analyze");

    const collection = db.collection("matches");
    const collectionAnalyze = db.collection("matches_analyze");

    var obj = {
        update: async function (match, dbMatch) {
            var setObject = {
                private_data_loaded: true
            };
            for (let index = 0; index < match.players.length; index++) {
                if(dbMatch.players[index].account_id == 4294967295){
                    setObject['players.'+index+'.account_id']=  match.players[index].account_id;
                }
            }
            match.match_id = dbMatch.match_id;
            await collection.updateOne({_id: dbMatch._id}, { $set : setObject })
        },
        getNotProcessed: async function (id) {
            var now =Math.round(+new Date()/1000);
            var expire = now - 60*1*10;//10 min
            var filter=   {$or:[{reserve_instance_id: null},{reserve_update_time: { $lt: expire } }]};
            return await (collection.findOneAndUpdate(filter,
                {
                    $set:{
                        reserve_update_time: now,
                        reserve_instance_id: id
                    }
                }));
        },
        delete: async function(matchId){
            await collection.deleteMany({_id: matchId});
        },
        addSalt: async function addSalt(match_id, replay_salt, cluster, match){
            var players = match.players.map(p=>({
                account_id: p.account_id,
                hero_id: p.hero_id,
            }))
            await collectionAnalyze.insertOne({match_id, replay_salt, cluster, players});
        }
    }
    return obj;
}