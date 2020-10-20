async function start(){
    console.log('start');
    let matchRepository = await require("./repositories/match_repository")();
    var config = require("./config");
    var rankLoader =await require("./matchRankLoader")(config);
    let deleteIds = [];
    let filtered = [];
    while(true){
        let matches = await matchRepository.getNotFiltered();
        deleteIds = [];
        filtered = [];

        for (let index = 0; index < matches.length; index++) {
            const match = matches[index];
            var player = match.players.find(x=>x.account_id != 4294967295);
            if(player){
    
                var rank =await load(rankLoader, player.account_id);
                if(rank && rank>=60){//80+ imortals, 70+ divene
                    filtered.push(match._id)
                } else {
                    deleteIds.push(match._id);
                }
            }
            else {
                deleteIds.push(match._id);
            }
        }
        await matchRepository.delete(deleteIds);
        await matchRepository.skillFiltered(filtered);
        console.log('deleted'+deleteIds.length)
        console.log('filtered'+filtered.length)
    }
}

async function load(loader, accountId){
    return new Promise((resolve, reject) => {
        var done = false;
        setTimeout(() => {
            if(!done)
            {
                console.log("timeout account loading")
                resolve(null);
            }
          }, 20*1000);
        
          loader.Dota2.requestProfileCard(accountId,function (error, profileData) {
            resolve(profileData.rank_tier);
            done = true;
        });
    });
}
start();
