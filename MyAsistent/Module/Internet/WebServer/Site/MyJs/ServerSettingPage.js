class ServerSettings{
    ips;
    ip;
    port;
	SelectedIps;


    constructor(ips,ip,port,SelectedIps){
        this.port = port;
		this.SelectedIps = SelectedIps;
        this.ip = ip;
        this.ips = ips;
		
	}
	update() {
        document.getElementById('name-654f').value = this.ip;
        document.getElementById('email-654f').value = this.port;
        var dropdown = document.getElementById('select-0f6d'); // Получаем элемент выпадающего списка по его идентификатору
		dropdown.options.length = 0;
		for (var i = 0; i < this.ips.length; i++) {
			var option = document.createElement('option');
			option.text = this.ips[i];
			dropdown.add(option);
		}
		dropdown.value = this.SelectedIps;
    }
}




    

function Change(){
	console.log( Array.from(document.getElementById('select-0f6d').options));
    DenfToJson(new ServerSettings(
	
        Array.from(document.getElementById('select-0f6d').options).map(function(option) {
			return option.value; // Возвращаем значение каждой опции
		}),
        document.getElementById("name-654f").value,
        document.getElementById('email-654f').value,
		document.getElementById('select-0f6d').value,
    ), 'ServerSettings', 'update');
}
function Load(){
    DenfToJson(new ServerSettings(), "ServerSettings", "get");
	document.getElementById('name-654f').addEventListener('blur', function(event) {
    Change();
	location.reload(); 
    
  });
}