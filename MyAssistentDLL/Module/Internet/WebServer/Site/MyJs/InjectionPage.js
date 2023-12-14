class InjectionModule{
    check;
    login;
    pass;


    constructor(check,login,pass){
        this.pass = pass;
        this.login = login;
        this.check = check;
    }
    update() {
        document.getElementById('name-654f').value = this.login;
        document.getElementById('email-654f').value = this.pass;
        document.getElementById("checkbox-171c").checked = this.check;
    }
}
function Change(){
    DenfToJson(new InjectionModule(
        document.getElementById("checkbox-171c").checked,
        document.getElementById("name-654f").value,
        document.getElementById('email-654f').value
    ), 'InjectionModule', 'update');
}
function Load(){
    DenfToJson(new InjectionModule(), "InjectionModule", "get");
}