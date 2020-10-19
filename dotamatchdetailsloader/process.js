async function getDb(){
    const MongoClient = require("mongodb").MongoClient;
    const mongoClient = new MongoClient("mongodb+srv://e1ektr0:Tn7O5zyFdgZ0t3nP@cluster0.ncrvw.gcp.mongodb.net/dota2?retryWrites=true&w=majority", { useUnifiedTopology: true });
    var connection = await mongoClient.connect();
    return connection.db("dota2");
}
async function start(){
    await insertAccounts();
    var requestLimit = 100;
    const db = await getDb();
    const accountsCollection = db.collection("accounts");
    var accounts = await accountsCollection.find().toArray();

    const collection = db.collection("matches");
    var mysort = { match_seq_num: 1 };  
    for (let index = 0; index < accounts.length; index++) {
        const account = accounts[accounts.length-index-1];

        if(accounts.requestCount>=requestLimit){
            if(Math.round(+new Date()/1000)- acounts.lastRequestTime>86400){
                accounts.requestCount = 0;
                await accountsCollection.updateOne({_id: account._id}, {set:{
                    requestCount: 0,
                    lastRequestTime:Math.round(+new Date()/1000)
                }});
            }
            else{
                continue;
            }
        }

        account.steam_name = account.steam_user;
        var loader = await require("./privateMatchLoader.js")(account);
        console.log("loader created")
        if(loader == null)
        {
            console.log("loader fail");
            continue;
        }
        while(true){
            var array = await (collection.find({ private_data_loaded: {$ne: true} }).sort(mysort).limit(1).toArray()); 
            if(array.length>0) {
                var dbMatch = array[0];
                
                var match = await load(loader, dbMatch.match_id);
                if(match == null){
                    match = await load(loader, dbMatch.match_id);
                    if(match == null)
                        break;
                }
                account.requestCount = account.requestCount|0+1;
                await accountsCollection.updateOne({_id: account._id}, {set:{
                    requestCount: account.requestCount,
                    lastRequestTime:Math.round(+new Date()/1000)
                }});
                if(account.requestCount>requestLimit)
                    break;  
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
    }
}
async function insertAccounts(){
    console.log("start account loads")

    const db = await getDb();
    const accountsCollection = db.collection("accounts");
    var accounts = await accountsCollection.find().toArray();

    const fs = require('fs')
    var data = await fs.readFileSync('raw_accounts.txt', 'utf8')
    var dataArray = data.split(/\r?\n/); 
    for (let index = 0; index < dataArray.length; index++) {
        const row = dataArray[index];
        var info = row.split(':');
       if( !accounts.some(n=>n.steam_user == info[0])){
            await accountsCollection.insertOne({
                steam_user: info[0],
                steam_pass: info[1]
            });
            console.log("inset "+row)
       }
    }
    console.log("complete account loads")
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