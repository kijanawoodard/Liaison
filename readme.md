#What is Liaison?

> a binding ... agent

>  a close bond or connection

>one that establishes and maintains communication for mutual understanding and cooperation

> <cite>http://www.merriam-webster.com/dictionary/liaison</cite>



> ... a means of communication between different ... units ...

> One that maintains communication

> the relationship ... to ensure unity of purpose

>	<cite>http://www.thefreedictionary.com/Liason<cite>

##Summary
An in-memory mediator written in c#. Inspired by [nimbus].

Nimbus is bloated and lacks clarity.

##Usage

	//app start
	mediator.Subscribe<PostRequest, PostGetViewModel>(message =>
	{
		var result = new PostGetViewModel();
		result = new FilteredPostVault().Handle(message, result);
		result = new MarkdownContentStorage(root).Handle(message, result);
		return result;
	});
	
	//somewhere else
	var result = _mediator.Send<PostRequest, PostGetViewModel>(message);

[nimbus]: https://github.com/kijanawoodard/nimbus