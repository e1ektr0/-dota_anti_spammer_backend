module.exports  =  async function (){
    const MongoClient = require("mongodb").MongoClient;
    const mongoClient = new MongoClient("mongodb://e1ekt0:secretPassword@194.87.103.72:27017/?authSource=dota2&readPreference=primary&appname=MongoDB%20Compass&ssl=false", { useUnifiedTopology: true });
    var connection = await mongoClient.connect();
    return connection.db("dota2");
}