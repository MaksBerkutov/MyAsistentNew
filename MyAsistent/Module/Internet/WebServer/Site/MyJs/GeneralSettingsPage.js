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




    

function Change(){
    DenfToJson(new MainSettingsModule(
	document.getElementById('checkbox-171c').checked,
	document.getElementById('checkbox-f54b').checked,
	document.getElementById('select-6cb0').value,
	document.getElementById('select-cc65').value,
	document.getElementById('number-6ca4').value,
	document.getElementById('number-86a0').value,
        Array.from(document.getElementById('select-6cb0').options).map(function(option) {
			return option.value; // Возвращаем значение каждой опции
		}),
		 Array.from(document.getElementById('select-cc65').options).map(function(option) {
			return option.value; // Возвращаем значение каждой опции
		}),
       
    ), 'MainSettingsModule', 'update');
}
function Load(){
    DenfToJson(new MainSettingsModule(), "MainSettingsModule", "get");
}