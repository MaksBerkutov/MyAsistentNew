
class Send {
    name;
    action;
    date;
    constructor(date, action, name) {
        this.name = name;
        this.action = action;
        this.date = date;
    }
}
function clearDropdownList(dropdownId) {
    var dropdown = document.getElementById(dropdownId);
    
    while (dropdown.options.length > 0) {
      dropdown.remove(0);
    }
}
async function DenfToJson(classes,name,acton) {
    const obj = new Send(JSON.stringify(classes), acton, name);
    console.log('------------');
    console.log(obj);
    (async () => {
        const rawResponse = await fetch('http://'+window.location.host+'/json/', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(obj)
        });
        const content = await rawResponse.json();
        console.log(content);
        Object.assign(classes, JSON.parse(content));
        console.log(classes);
        if (acton=='get')
            classes.update();
       

    })();



}
//Helper Not Main Work | Karoche ono tyt nahui ne nado no pyst poka bydet =) |
function removeOptions(selectElement) {
    var i, L = selectElement.options.length - 1;
    for (i = L; i >= 0; i--) {
        selectElement.remove(i);
    }
}
function OnChangeSelcted() {
    
    settings.text = settings.stringarr[document.getElementById('mainselect').selectedIndex];
    alert(document.getElementById('mainselect').selectedIndex);
    (async () => {
        const rawResponse = await fetch('http://localhost:7878/json/', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(settings)
        });
        const content = await rawResponse.json();
        settings = Object.assign(new Settings(), content);
        console.log(settings);
        console.log(content);
    })();
}
function UpdateDate(){
    let element = document.getElementById('mainselect');
    for (let i = 0; i < settings.stringarr.length; i++) {
        const opt1 = document.createElement("option");
        opt1.value = i;
        opt1.text = settings.stringarr[i];
        element.add(opt1, null);
    }
}