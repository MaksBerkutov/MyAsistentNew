class TelegramBotModule{
    check;
    api;
    id;


    constructor(check,api,id){
        this.id = id;
        this.api = api;
        this.check = check;
    }
    update() {
        document.getElementById('name-654f').value = this.api;
        document.getElementById('email-654f').value = this.id;
        document.getElementById("checkbox-171c").checked = this.check;
    }
}
function Change(){
    DenfToJson(new TelegramBotModule(
        document.getElementById("checkbox-171c").checked,
        document.getElementById("name-654f").value,
        document.getElementById('email-654f').value
    ), 'TelegramBotModule', 'update');
}
function Load(){
    DenfToJson(new TelegramBotModule(), "TelegramBotModule", "get");
}