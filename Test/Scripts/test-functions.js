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
		var url = 'http://' + rootUrl + '/Post/dellComment';

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
		var url = 'http://' + rootUrl + '/Post/dellPost';

		xhr.open('POST', url, true)
		xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded')

		xhr.send(body);
	}