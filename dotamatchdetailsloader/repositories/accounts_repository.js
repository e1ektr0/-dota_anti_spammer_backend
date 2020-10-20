module.exports = async function(){
    var db = await require("../db")();
    const fs = require('fs');
    const accountsCollection = db.collection("accounts");
    var obj = {
        insertAccounts: async function(){
            var accounts = await accountsCollection.find().toArray();
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
        },
        update: async function(account){
            console.log("update a cc"+account.requestCount)
            console.log(account)
            await accountsCollection.updateOne({_id: account._id}, {$set:{
                requestCount: account.requestCount,
                lastRequestTime:Math.round(+new Date()/1000)
            }});
        },
        getAll: async function() {
            return await accountsCollection.find({guarded:{$ne: true}}).toArray()
        },
        updateProxy: async function (account) {
            var accounts = await this.getAll();
            var data = await fs.readFileSync('raw_proxy.txt', 'utf8')
            var dataArray = data.split(/\r?\n/); 
            for (let index = 0; index < dataArray.length; index++) {
                const row = dataArray[index];
                var info = row.split(':');
               if( !accounts.some(n=>n.proxy && n.proxy.host ==info[0])){
                    account.proxy = {
                        host: info[0],
                        port: info[1]
                    };
                    await accountsCollection.replaceOne({_id: account._id}, account);
                    return true;
               }
            }
            return false;
        },
        updateFailLogin: async function(account){
            account.failLogin = Math.round(+new Date()/1000);
            var set =  {$set:{
                failLogin: account.failLogin,
                guarded: true
            }};
            await accountsCollection.updateOne({_id: account._id} ,set);
        }
    }
    obj.accountsCollection = accountsCollection;
    return obj;
}

