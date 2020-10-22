module.exports = async function(user, password, markSeen){
    var imaps = require('imap-simple');
    const _ = require('lodash');
    var config = {
        imap: {
        user: user,
        password: password,
        host: 'imap.rambler.ru',
            port: 993,
            tls: true,
            authTimeout: 3000
        }
    };
    var connection = await imaps.connect(config);
    await connection.openBox('INBOX');
    var searchCriteria = [
        'UNSEEN'
    ];

    var fetchOptions = {
        bodies: ['HEADER', 'TEXT'],
        markSeen: markSeen
    };

    var results = await connection.search(searchCriteria, fetchOptions);
    var emails =  codes = results.filter(function (res) {
        return res.parts.filter(function (part) {
            return part.which === 'HEADER';
        })[0].body.subject[0] == 'Your Steam account: Access from new computer';
    });
    if(emails.length>0){
        var item =  emails[emails.length-1];
        var all = _.find(item.parts, { "which": "TEXT" })
        var pattern = '<span style="font-size: 24px; color: #66c0f4; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">';
        var start = all.body.indexOf(pattern);
        return all.body.substring(start+pattern.length, start+pattern.length+7).trim();
    }
}