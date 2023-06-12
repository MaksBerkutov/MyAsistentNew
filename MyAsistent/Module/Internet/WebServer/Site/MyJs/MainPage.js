class WebServerSettings{
    check;
    path;
    Url;


    constructor(check,path,Url){
        this.Url = Url;
        this.path = path;
        this.check = check;
    }
    update() {
        document.getElementById('name-654f').value = this.path;
        document.getElementById('email-654f').value = this.Url;
        document.getElementById("checkbox-171c").checked = this.check;
    }
}
function Change(){
    DenfToJson(new WebServerSettings(
        document.getElementById("checkbox-171c").checked,
        document.getElementById("name-654f").value,
        document.getElementById('email-654f').value
    ), 'WebServerSettings', 'update');
}
function Load(){
    DenfToJson(new WebServerSettings(), "WebServerSettings", "get");
}