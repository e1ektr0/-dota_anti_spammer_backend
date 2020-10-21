module.exports  =  async function (){
    const MongoClient = require("mongodb").MongoClient;
    const mongoClient = new MongoClient("mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false", { useUnifiedTopology: true });
    var connection = await mongoClient.connect();
    return connection.db("dota2");
}