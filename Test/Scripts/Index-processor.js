	document.getElementById('indexCatSelect').onclick = function () { //Выбор другой категории новостей
		var category = document.getElementById('minp').value;
        var catId = window.event.toElement.id;
		if (catId != category)
		{
			var catText = window.event.toElement.text;
			window.event.currentTarget.children[0].value = catId;
			document.getElementById('catBtn').innerText = "Категория: " + catText;
			document.getElementById('index-posts').innerHTML = "";
			
			getPosts.startFrom = 0;
			getPosts.inProgress = true;
			getPosts.sendRequest();
			getPosts.startFrom += 3;
			getPosts.inProgress = false;
		}
    }
	
	function postLoad() {
		this.inProgress = false;
		this.startFrom = 0;
		this.sendRequest = sendRequest;
		this.createPostContent = createPostContent;
		this.DataProc = DataProc;
		
	}

	DataProc = function(data) //Обработка принятых данных
	{
		if (data.length > 0) {

			for(var index in data)
			{
				createPostContent(data[index]);
			}
		}
	}

	createPostContent = function(data) //Формирование контента поста
	{
		var top = '<br /> <div class="divider"></div>            <div class="post">                <div class="row">                    <div class="col-md-12 col-lg-12">                        <div class="posttitle"><h4>';
		var title_text = '</h4></div>                    </div>                </div>                <br />                <div class="row">                    <div class="col-md-12 col-lg-12">                        <div class="posttext">';
		var text_userName =  '</div>                    </div>                </div>                <div class="row">                    <div class="col-md-12 col-lg-12">                        <div class="postuser">                            <a href="/Account/userProfile?user=' + data.user_id + '">';
		var userLastName_postImage = '</a>                        </div>                    </div>                </div>                <div class="row">                    <div class="col-md-12 col-lg-12">';
		var postImage = '';
		if (data.image_name != "")
			postImage = '<img src="http://' + location.host + '/images/posts/' + data.image_name + '" class="img-rounded img-responsive" alt="' + data.title + '">';
		var postImage_category = '</div>                </div>                <div class="tdivider"> </div>                <div class="row">                    <div class="col-md-6 col-lg-6"><h4>Категория: ';
		var category_detalBtn = '</h4></div>                    <div class="col-md-2 col-lg-2">                        <p><a class="btn btn-default" href="/Post/PostPage?post=';
		var detalBtn_editBtn = '">Подробнее &raquo;</a></p>                    </div>                    <div class="col-md-2 col-lg-2">';
		var editBtn = '';
		if (data.currUser_id == data.user_id)
			editBtn = '<p><a class="btn btn-info" href="Post/EditPost?post=' + data.id + '">Редактировать</a></p>';
		var editBtn_dellBtn = '</div>                    <div class="col-md-2 col-lg-2">';
		var dellBtn = '';
		if (data.currUser_id == data.user_id)
			dellBtn = '<button id="' + data.id + '" type="button" class="btn btn-danger" onclick="dellPost()">Удалить</button>';
		var bottom = '</div>                </div>            </div>';
		
		$("#index-posts").append(top + data.title + title_text + data.text + text_userName + data.userName + " " + data.userLastName + userLastName_postImage + postImage + postImage_category + data.category + category_detalBtn + data.id
		+ detalBtn_editBtn + editBtn + editBtn_dellBtn + dellBtn + bottom);
	}
	
	sendRequest = function() //Отправка запроса на получение постов
	{
		var startFrom = this.startFrom;
		var categoryInp = document.getElementById('minp');
		var category = categoryInp.value;
		var rootUrl = location.host;
		var url = 'http://' + rootUrl + '/Post/GetPosts';
		var data = {"startFrom" : startFrom, "category" : category};
		$.ajax({
			/* адрес файла-обработчика запроса */
		    url: '/Post/GetPosts',
			/* метод отправки данных */
			method: 'POST',
			/* данные, которые мы передаем в*/
			data: {"startFrom" : startFrom, "category" : category},
			/* выполняем после приёма данных */
			success: function(data)
			{
				DataProc(data);
				
			}
		});
	}
	
	var count = 0;
	var getPosts = new postLoad();
	
	window.onload = function() //Запрашиваем первые новости на страницу
	{
	
		getPosts.inProgress = true;
		getPosts.sendRequest();
		getPosts.startFrom += 3;
		getPosts.inProgress = false;
	}
	
	document.ready = function(){
	
    window.onscroll = function() { //Запрашиваем новости при прокрутке
		
        if(window.pageYOffset >= getDocumentHeight() - window.outerHeight - 200 && !getPosts.inProgress) {
			getPosts.inProgress = true;
			getPosts.sendRequest();
			getPosts.startFrom += 3;
			getPosts.inProgress = false;
        }
    };
};

function getDocumentHeight() {
	return Math.max(document.compatMode != 'CSS1Compat' ? document.body.scrollHeight : document.documentElement.scrollHeight, getViewportHeight());
}

function getViewportHeight() {
	var ua = navigator.userAgent.toLowerCase();
	var isOpera = (ua.indexOf('opera')  > -1);
	var isIE = (!isOpera && ua.indexOf('msie') > -1);
	return ((document.compatMode || isIE) && !isOpera) ? (document.compatMode == 'CSS1Compat') ? document.documentElement.clientHeight : document.body.clientHeight : (document.parentWindow || document.defaultView).innerHeight;
}