module.exports = async function (username, password) {//user, password, markSeen){

    var POP3Client = require("mailpop3");
    var client = new POP3Client(995, 'pop3.rambler.ru', {
        tlserrs: false,
        enabletls: true,
        debug: false
    });
    var debug = false;
    client.on("error", function (err) {

        if (err.errno === 111) console.log("Unable to connect to server");
        else console.log("Server error occurred");

        console.log(err);

    });

    client.on("connect", function (rawdata) {

        client.login(username, password);

    });

    client.on("invalid-state", function (cmd) {
        console.log("Invalid state. You tried calling " + cmd);
    });

    client.on("locked", function (cmd) {
        console.log("Current command has not finished yet. You tried calling " + cmd);
    });


    client.on("list", function (status, msgcount, msgnumber, data, rawdata) {

        if (status === false) {

            console.log("LIST failed");
            client.quit();

        } else if (msgcount > 0) {

            totalmsgcount = msgcount;
            currentmsg = msgcount;
            client.retr(msgcount);

        } else {

            console.log("LIST success with 0 message(s)");
            client.quit();

        }
    });


    client.on("dele", function (status, msgnumber, data, rawdata) {

        if (status === true) {

            console.log("DELE success for msgnumber " + msgnumber);

            if (currentmsg > totalmsgcount)
                client.quit();
            else
                client.retr(currentmsg);

        } else {

            console.log("DELE failed for msgnumber " + msgnumber);
            client.rset();

        }
    });

    client.on("rset", function (status, rawdata) {
        client.quit();
    });

    client.on("quit", function (status, rawdata) {

        if (status === true) console.log("QUIT success");
        else console.log("QUIT failed");

    });
    return new Promise((resolve, reject) => {
        var done = false;
        resolver = resolve;
        setTimeout(() => {
            if (!done) {
                console.log("timeout dota2 loading")
                resolve(null);
            }
        }, 20 * 1000);

        client.on("login", function (status, rawdata) {

            if (status) {

                client.list();

            } else {

                console.log("LOGIN/PASS failed");
                client.quit();
                resolve(null);

            }

        });
        client.on("retr", function (status, msgnumber, data, rawdata) {

            if (status === true) {

                var pattern = '<span style="font-size: 24px; color: #66c0f4; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">';
                var start = data.indexOf(pattern);
                done = true;
                resolve(data.substring(start + pattern.length, start + pattern.length + 7).trim());

            } else {

                console.log("RETR failed for msgnumber " + msgnumber);
                client.rset();

            }
        });
    });
}