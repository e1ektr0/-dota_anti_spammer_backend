module.exports = async function(){
    var db = await require("../db")();
    const collection = db.collection("matches");
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
        }
    }
    return obj;
}