![LOGO](https://github.com/MaksBerkutov/MyAsistentNew/blob/master/ImageGit/Logo/Logo_Blue.svg)

# My Asisstent гайд до написання сайту
---

## З чого почати?
---

Почніть з дизайну сайта і проробіть його. Далі його потрібно зверстати. Потім перейдіть до підключення логіки.

Усе працює за допомогою `POST/GET` запитів до серверу. Сервер відповідає на запити клієнта.

На головній сторінці немає коду для обробки логіки, всі дії перенаправляються на сторінки з налаштуваннями.

Давайте розглянемо підключення вашої сторінки.

Спочатку створіть клас для цієї сторінки (наприклад, `WebServerSettings`):

```javascript
class WebServerSettings {
    check;
    path;
    Url;

    constructor(check, path, Url) {
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
```
В класі WebServerSettings є метод update(), в якому ви можете реалізувати свою унікальну функціональність.

Далі, вам потрібно створити функцію, яка буде викликатися при завантаженні сторінки. Ось приклад коду для цієї функції:

```javascript
    function Load() {
        DenfToJson(new WebServerSettings(), "WebServerSettings", "get");
    }

}
```

Будьте обережні, не змінюйте аргументи функції DenfToJson, а також назву класу та змінних у ньому.

Також вам знадобиться функція, яка буде оновлювати дані (змінювати налаштування) на сервері. Ось приклад такої функції:

```javascript
function Change() {
    DenfToJson(
        new WebServerSettings(
            document.getElementById("checkbox-171c").checked,
            document.getElementById("name-654f").value,
            document.getElementById('email-654f').value
        ),
        'WebServerSettings',
        'update'
    );
}
```

Це всі вказівки. Якщо у вас є будь-які незрозумілості, перегляньте вихідний код сайту, який є вільнодоступним.

## Коди всіх сторінок
У проекті є різні сторінки з різними налаштуваннями. Ось приклади класів для всіх сторінок:
### Main

```js
   
class MainSettingsModule{	
	VoiceMessage;
    VoiceLog;
	
    CultureSpeech;
    CultureRecognition;
	
    timeAutoSave;
    timeWaitIniit;
	
    AllSpeech;
    AllRecognition;


    constructor(VoiceMessage,VoiceLog,CultureSpeech,CultureRecognition,timeAutoSave,timeWaitIniit,AllSpeech,AllRecognition){
        this.VoiceMessage = VoiceMessage;
        this.VoiceLog = VoiceLog;
        this.CultureSpeech = CultureSpeech;
        this.CultureRecognition = CultureRecognition;
        this.timeAutoSave = timeAutoSave;
        this.timeWaitIniit = timeWaitIniit;
        this.AllSpeech = AllSpeech;
        this.AllRecognition = AllRecognition;

		
	}
	update() {
        document.getElementById('checkbox-171c').checked = this.VoiceMessage;
        document.getElementById('checkbox-f54b').checked = this.VoiceLog;
        document.getElementById('number-6ca4').value = this.timeAutoSave;
        document.getElementById('number-86a0').value = this.timeWaitIniit;
		
        var dropdown = document.getElementById('select-cc65'); 
		dropdown.options.length = 0;
		for (var i = 0; i < this.AllRecognition.length; i++) {
			var option = document.createElement('option');
			option.text = this.AllRecognition[i];
			dropdown.add(option);
		}
		dropdown.value = this.CultureSpeech;
		
		var dropdownCultureRecognition = document.getElementById('select-6cb0'); 
		dropdownCultureRecognition.options.length = 0;
		for (var i = 0; i < this.AllSpeech.length; i++) {
			var option = document.createElement('option');
			option.text = this.AllSpeech[i];
			dropdownCultureRecognition.add(option);
		}
		dropdownCultureRecognition.value = this.CultureRecognition;
    }
}
```
---
### Network

```js
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
        var dropdown = document.getElementById('select-0f6d');
		dropdown.options.length = 0;
		for (var i = 0; i < this.ips.length; i++) {
			var option = document.createElement('option');
			option.text = this.ips[i];
			dropdown.add(option);
		}
		dropdown.value = this.SelectedIps;
    }
}

```
### Telegram Bot

```js
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

```
### Injection Code

```js
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

```
### Local Web Server

```js
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

```
