async function start(){
    const MongoClient = require("mongodb").MongoClient;

    const mongoClient = new MongoClient("mongodb+srv://e1ektr0:Tn7O5zyFdgZ0t3nP@cluster0.ncrvw.gcp.mongodb.net/dota2?retryWrites=true&w=majority", { useUnifiedTopology: true });
    
    var connection = await mongoClient.connect();
    const db = connection.db("dota2");
    const collection = db.collection("matches");

    var mysort = { match_seq_num: 1 };  
    var loader = await require("./privateMatchLoader.js")();
    console.log("loader created")
    var array = await (collection.find({ private_data_loaded: {$ne: true} }).sort(mysort).limit(1).toArray()); 
    if(array.length>0) {
        var dbMatch = array[0];
        
        var match = await load(loader, dbMatch.match_id)
        var setObject = {
            private_data_loaded: true
        };
        for (let index = 0; index < match.players.length; index++) {
            if(dbMatch.players[index].account_id == 4294967295){
                setObject['players.'+index+'.account_id']=  match.players[index].account_id;
            }
            setObject['players.'+index+'.hero_pick_order']=  match.players[index].hero_pick_order;
        }
        match.match_id = dbMatch.match_id;
        await collection.updateOne({_id: array[0]._id}, { $set : setObject })
        console.log('done')
    }
    console.log('complete')
}

async function load(loader, matchId){
    return new Promise((resolve, reject) => {
        loader.Dota2.requestMatchDetails(matchId, function(err, body) {
            if (err) throw err;
            resolve(body.match);
        });
    });
}

start();