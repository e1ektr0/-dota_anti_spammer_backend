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
        getNotProcessed: async function () {
            return await (collection.find({ private_data_loaded: {$ne: true}}).limit(10).toArray());
        },
        getNotFiltered: async function () {
            return await (collection.find({ skill_filtered: {$ne: true} }).limit(30).toArray());
        },
        skillFiltered: async function(matchIds){
            await collection.updateMany({_id: { $in: matchIds }}, { $set : {skill_filtered:true} })
        },
        delete: async function(matchIds){
            await collection.deleteMany({_id: { $in: matchIds }});
        }
    }
    return obj;
}