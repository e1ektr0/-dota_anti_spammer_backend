module.exports  =  async function (){
    const MongoClient = require("mongodb").MongoClient;
    const mongoClient = new MongoClient("mongodb+srv://e1ektr0:Tn7O5zyFdgZ0t3nP@cluster0.ncrvw.gcp.mongodb.net/dota2?retryWrites=true&w=majority", { useUnifiedTopology: true });
    var connection = await mongoClient.connect();
    return connection.db("dota2");
}