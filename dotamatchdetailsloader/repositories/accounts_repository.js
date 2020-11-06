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
                var account = accounts.find(n=>n.steam_user == info[0])
               if( !account){
                    await accountsCollection.insertOne({
                        steam_user: info[0],
                        steam_pass: info[1]
                    });
                    console.log("inset "+row)
               }else{
                   if(!account.email){
                    await accountsCollection.updateOne({_id: account._id},{$set: {
                        email: info[2],
                        email_pass: info[3]
                    }});
                   }
               }
            }
            console.log("complete account loads")
        },
        update: async function(account){
            await accountsCollection.updateOne({_id: account._id}, {$set:{
                requestCount: account.requestCount,
                lastRequestTime:Math.round(+new Date()/1000)
            }});
        },
        
        updateSentry: async function(account){
            await accountsCollection.updateOne({_id: account._id}, {$set:{
                sentry: account.sentry
            }});
        },
        getAll: async function() {
            return await accountsCollection.find().toArray()
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
        },
        reserve: async function(id){
            var now =Math.round(+new Date()/1000);
            var expire = now - 60*1;//1 min
            var rateLimitExpire = now - 86400;
            var failExpire = now - 60*60*3;//3h
            var filter = {$and:
                [
                   {$or:[{reserve_instance_id: null},{$or:[{lastRequestTime: { $lt: expire } }, {lastRequestTime:null}]}]}, 
                   {
                       $or:[ 
                           {lastRequestTime: null},
                           {lastRequestTime:{ $lt: rateLimitExpire }}, 
                           {requestCount:{ $lt: 94 }},
                           {$and:[{requestCount:100}, {lastRequestTime: { $lt: failExpire } }]}
                       ]
                   }
               ]};
            let result = await accountsCollection.findOneAndUpdate( filter,
                {$set:{
                    lastRequestTime: now,
                    reserve_instance_id: id
            }}, { sort: { 'lastRequestTime': 1 } });
            if(result&&result.value&&result.value.requestCount == 100)
                result.value.requestCount = 0;
            return result;
        }
    }
    obj.accountsCollection = accountsCollection;
    return obj;
}

