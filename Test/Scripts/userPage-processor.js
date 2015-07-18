	document.getElementById('mitem').onclick = function () 
	{
		var catId = window.event.toElement.id;
		var catText = window.event.toElement.text;
		window.event.currentTarget.children[0].value = catId;
		//var btnValue = document.getElementById('catBtn');
		document.getElementById('catBtn').innerText = "Категория: " + catText;
		if(catId == 'newCat') document.getElementById('catText').type = "text";
	}