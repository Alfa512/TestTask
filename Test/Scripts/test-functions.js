	function dellComment()
	{
		var comment = window.event.currentTarget;
		var commentId = comment.id;
		commentId = Number(commentId.replace(/\D+/g,""));
		comment = comment.parentElement;
		comment = comment.parentElement;
		comment = comment.parentElement;
		comment.remove();
		
		var xhr = new XMLHttpRequest();

		var body = 'commentId=' + commentId;
		
		var rootUrl = location.host;
		var url = 'http://' + rootUrl + '/Post/DellComment';

		xhr.open('POST', url, true)
		xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded')

		xhr.send(body);
	}
	
	function dellPost()
	{
		var post = window.event.currentTarget;
		var postId = post.id;
		postId = Number(postId.replace(/\D+/g,""));
		post = post.parentElement;
		post = post.parentElement;
		post = post.parentElement;
		post.remove();
		
		var xhr = new XMLHttpRequest();

		var body = 'postId=' + postId;
		
		var rootUrl = location.host;
		var url = 'http://' + rootUrl + '/Post/DellPost';

		xhr.open('POST', url, true)
		xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded')

		xhr.send(body);
	}

	document.getElementById('mitem').onclick = function () {
	    var catId = window.event.toElement.id;
	    var catText = window.event.toElement.text;
	    window.event.currentTarget.children[0].value = catId;
	    //var btnValue = document.getElementById('catBtn');
	    document.getElementById('catBtn').innerText = "Категория: " + catText;
	    if (catId == 'newCat') document.getElementById('catText').type = "text";
	}